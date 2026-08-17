using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// タワーの攻撃・レベル・強化・売却・見た目を管理する。
///
/// ・CircleCollider2D → 攻撃範囲・敵検知専用
/// ・TowerClickArea → クリック専用
/// ・レベル別Prefab → 見た目専用
/// </summary>
public class Tower : MonoBehaviour
{
    // =====================================================
    // タワーデータ
    // =====================================================

    [Header("タワーデータ")]
    public TowerData towerData;


    // =====================================================
    // レベル
    // =====================================================

    [Header("レベル")]
    [SerializeField]
    private int level = 1;


    // =====================================================
    // 現在のステータス
    // =====================================================

    private int currentAttackDamage;

    private float currentAttackInterval;

    private float currentAttackRange;


    // =====================================================
    // 敵
    // =====================================================

    private readonly List<Enemy> enemiesInRange =
        new List<Enemy>();

    private Enemy currentTarget;


    // =====================================================
    // 攻撃
    // =====================================================

    private float nextAttackTime;


    // =====================================================
    // Collider
    // =====================================================

    // 攻撃範囲専用
    private CircleCollider2D rangeCollider;


    // =====================================================
    // 見た目
    // =====================================================

    [Header("見た目")]
    [Tooltip("レベル別Prefabを生成する親")]
    public Transform visualRoot;

    // 現在表示している見た目
    private GameObject currentVisual;


    // =====================================================
    // プロパティ
    // =====================================================

    public int Level => level;

    public int CurrentAttackDamage =>
        currentAttackDamage;

    public float CurrentAttackInterval =>
        currentAttackInterval;

    public float CurrentAttackRange =>
        currentAttackRange;


    // =====================================================
    // Unity
    // =====================================================

    private void Awake()
    {
        // ---------------------------------------------
        // 攻撃範囲Collider取得
        // ---------------------------------------------

        rangeCollider =
            GetComponent<CircleCollider2D>();


        if (rangeCollider == null)
        {
            Debug.LogError(
                $"{gameObject.name}: " +
                "Tower本体にCircle Collider 2Dがありません。",
                this
            );

            return;
        }


        // 攻撃範囲なのでTrigger
        rangeCollider.isTrigger = true;


        // visualRootが設定されていない場合
        if (visualRoot == null)
        {
            Debug.LogWarning(
                $"{gameObject.name}: " +
                "Visual Rootが設定されていません。",
                this
            );
        }


        // towerDataがPrefab側に設定されている場合のみ
        if (towerData != null)
        {
            InitializeTower();
        }
    }


    private void Update()
    {
        if (towerData == null)
            return;


        RemoveInvalidEnemies();


        currentTarget =
            FindMostAdvancedEnemy();


        if (currentTarget == null)
            return;


        if (Time.time >= nextAttackTime)
        {
            Attack();
        }
    }


    // =====================================================
    // 初期化
    // =====================================================

    /// <summary>
    /// TowerPlacementManagerからTowerDataを設定した後に呼び出す。
    /// </summary>
    public void InitializeTower()
    {
        if (towerData == null)
        {
            Debug.LogError(
                $"{gameObject.name}: TowerDataが設定されていません。",
                this
            );

            return;
        }


        // ステータスを適用
        ApplyStats();


        // レベル1の見た目を生成
        UpdateVisual();
    }


    // =====================================================
    // ステータス
    // =====================================================

    /// <summary>
    /// 現在のレベルに応じてステータスを計算する。
    /// </summary>
    private void ApplyStats()
    {
        if (towerData == null)
            return;


        // 攻撃力
        currentAttackDamage =
            towerData.attackDamage
            + towerData.upgradeDamage
            * (level - 1);


        // 射程
        currentAttackRange =
            towerData.attackRange
            + towerData.upgradeRange
            * (level - 1);


        // 攻撃間隔
        currentAttackInterval =
            towerData.attackInterval
            - towerData.upgradeIntervalReduction
            * (level - 1);


        // 0.1秒未満にはしない
        currentAttackInterval =
            Mathf.Max(
                0.1f,
                currentAttackInterval
            );


        // 攻撃範囲Colliderを更新
        if (rangeCollider != null)
        {
            rangeCollider.radius =
                currentAttackRange;
        }
    }


    // =====================================================
    // 強化
    // =====================================================

    /// <summary>
    /// タワーを1レベル強化する。
    /// </summary>
    public bool Upgrade()
    {
        if (towerData == null)
            return false;


        if (level >= towerData.maxLevel)
            return false;


        // レベルアップ
        level++;


        // ステータス更新
        ApplyStats();


        // 見た目更新
        UpdateVisual();


        return true;
    }


    // =====================================================
    // 次のレベルのステータス
    // =====================================================

    public int GetNextAttackDamage()
    {
        if (towerData == null)
            return 0;


        int nextLevel =
            Mathf.Min(
                level + 1,
                towerData.maxLevel
            );


        return
            towerData.attackDamage
            + towerData.upgradeDamage
            * (nextLevel - 1);
    }


    public float GetNextAttackInterval()
    {
        if (towerData == null)
            return 0f;


        int nextLevel =
            Mathf.Min(
                level + 1,
                towerData.maxLevel
            );


        float nextInterval =
            towerData.attackInterval
            - towerData.upgradeIntervalReduction
            * (nextLevel - 1);


        return Mathf.Max(
            0.1f,
            nextInterval
        );
    }


    public float GetNextAttackRange()
    {
        if (towerData == null)
            return 0f;


        int nextLevel =
            Mathf.Min(
                level + 1,
                towerData.maxLevel
            );


        return
            towerData.attackRange
            + towerData.upgradeRange
            * (nextLevel - 1);
    }


    // =====================================================
    // レベル別見た目
    // =====================================================

    /// <summary>
    /// 現在のレベルに応じた見た目Prefabを生成する。
    /// </summary>
    private void UpdateVisual()
    {
        if (towerData == null)
            return;


        if (visualRoot == null)
        {
            Debug.LogWarning(
                $"{gameObject.name}: " +
                "VisualRootが設定されていないため、" +
                "レベル別見た目を生成できません。",
                this
            );

            return;
        }


        // ---------------------------------------------
        // 古い見た目を削除
        // ---------------------------------------------

        if (currentVisual != null)
        {
            Destroy(currentVisual);
            currentVisual = null;
        }


        // ---------------------------------------------
        // レベル別Prefab取得
        // ---------------------------------------------

        GameObject visualPrefab =
            towerData.GetLevelPrefab(level);


        if (visualPrefab == null)
        {
            Debug.LogWarning(
                $"{gameObject.name}: " +
                $"Lv.{level}の見た目Prefabが設定されていません。",
                this
            );

            return;
        }


        // ---------------------------------------------
        // 見た目を生成
        // ---------------------------------------------

        currentVisual =
            Instantiate(
                visualPrefab,
                visualRoot
            );


        // Transformを初期化
        currentVisual.transform.localPosition =
            Vector3.zero;

        currentVisual.transform.localRotation =
            Quaternion.identity;

        currentVisual.transform.localScale =
            Vector3.one;
    }


    // =====================================================
    // 強化費用
    // =====================================================

    public int GetUpgradeCost()
    {
        if (towerData == null)
            return 0;


        return towerData.upgradeCost * level;
    }


    // =====================================================
    // 売却
    // =====================================================

    public int GetSellPrice()
    {
        if (towerData == null)
            return 0;


        int totalCost =
            towerData.buildCost;


        for (int i = 1; i < level; i++)
        {
            totalCost +=
                towerData.upgradeCost * i;
        }


        return Mathf.FloorToInt(
            totalCost * towerData.sellRate
        );
    }


    // =====================================================
    // 攻撃範囲
    // =====================================================

    private void OnTriggerEnter2D(
        Collider2D other)
    {
        Enemy enemy =
            other.GetComponentInParent<Enemy>();


        if (enemy == null)
            return;


        if (!enemiesInRange.Contains(enemy))
        {
            enemiesInRange.Add(enemy);
        }
    }


    private void OnTriggerExit2D(
        Collider2D other)
    {
        Enemy enemy =
            other.GetComponentInParent<Enemy>();


        if (enemy == null)
            return;


        enemiesInRange.Remove(enemy);


        if (currentTarget == enemy)
        {
            currentTarget = null;
        }
    }


    private void RemoveInvalidEnemies()
    {
        enemiesInRange.RemoveAll(
            enemy =>
                enemy == null ||
                !enemy.gameObject.activeInHierarchy
        );
    }


    private Enemy FindMostAdvancedEnemy()
    {
        Enemy bestEnemy = null;


        float bestProgress =
            float.MinValue;


        foreach (Enemy enemy in enemiesInRange)
        {
            if (enemy == null)
                continue;


            if (!enemy.gameObject.activeInHierarchy)
                continue;


            float progress =
                enemy.RouteProgress;


            if (progress > bestProgress)
            {
                bestProgress = progress;
                bestEnemy = enemy;
            }
        }


        return bestEnemy;
    }


    // =====================================================
    // 攻撃
    // =====================================================

    private void Attack()
    {
        if (currentTarget == null)
            return;


        currentTarget.TakeDamage(
            currentAttackDamage
        );


        nextAttackTime =
            Time.time +
            currentAttackInterval;
    }
}