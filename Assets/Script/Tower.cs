using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// タワーの攻撃・レベル・強化・売却を管理する。
///
/// ・攻撃範囲内で最も進んでいる敵を攻撃
/// ・レベルアップ
/// ・レベル別Prefabの切り替え
/// ・攻撃範囲Colliderによる敵検知
/// ・攻撃範囲の常時表示
///
/// クリック判定はTowerClickArea.csで行う。
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
    // 現在ステータス
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

    [Header("攻撃範囲Collider")]
    [SerializeField]
    private CircleCollider2D rangeCollider;


    // =====================================================
    // レベル別見た目
    // =====================================================

    [Header("レベル別見た目")]
    [Tooltip("現在表示しているレベル別見た目")]
    [SerializeField]
    private Transform visualRoot;


    // =====================================================
    // 攻撃範囲表示
    // =====================================================

    [Header("攻撃範囲表示")]
    [Tooltip("攻撃範囲を表示するPrefab")]
    [SerializeField]
    private GameObject attackRangeVisualPrefab;


    private GameObject attackRangeVisualInstance;


    // =====================================================
    // プロパティ
    // =====================================================

    public int Level =>
        level;


    public int CurrentAttackDamage =>
        currentAttackDamage;


    public float CurrentAttackInterval =>
        currentAttackInterval;


    public float CurrentAttackRange =>
        currentAttackRange;


    // =====================================================
    // Awake
    // =====================================================

    private void Awake()
    {
        // -------------------------------------------------
        // CircleCollider2D取得
        // -------------------------------------------------

        if (rangeCollider == null)
        {
            rangeCollider =
                GetComponent<CircleCollider2D>();
        }


        if (rangeCollider == null)
        {
            Debug.LogError(
                $"{gameObject.name}: " +
                "Circle Collider 2Dがありません。",
                this
            );

            return;
        }


        // 攻撃範囲なのでTrigger
        rangeCollider.isTrigger = true;


        // -------------------------------------------------
        // ステータス適用
        // -------------------------------------------------

        ApplyStats();
    }


    // =====================================================
    // Start
    // =====================================================

    private void Start()
    {
        UpdateAttackRangeVisual();
    }


    // =====================================================
    // Update
    // =====================================================

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
    /// TowerPlacementManagerから生成された
    /// タワーを初期化する。
    ///
    /// towerDataはInitializeTower()を呼ぶ前に
    /// 設定しておく。
    /// </summary>
    public void InitializeTower()
    {
        if (towerData == null)
        {
            Debug.LogError(
                $"{gameObject.name}: " +
                "TowerDataが設定されていません。",
                this
            );

            return;
        }


        level = 1;


        ApplyStats();


        nextAttackTime =
            Time.time;


        // Lv1の見た目を生成
        UpdateLevelVisual();


        // 攻撃範囲表示
        UpdateAttackRangeVisual();
    }


    // =====================================================
    // ステータス
    // =====================================================

    private void ApplyStats()
    {
        if (towerData == null)
            return;


        currentAttackDamage =
            towerData.attackDamage
            + towerData.upgradeDamage *
            (level - 1);


        currentAttackRange =
            towerData.attackRange
            + towerData.upgradeRange *
            (level - 1);


        currentAttackInterval =
            towerData.attackInterval
            - towerData.upgradeIntervalReduction *
            (level - 1);


        currentAttackInterval =
            Mathf.Max(
                0.1f,
                currentAttackInterval
            );


        if (rangeCollider != null)
        {
            rangeCollider.radius =
                currentAttackRange;
        }


        UpdateAttackRangeVisual();
    }


    // =====================================================
    // 強化
    // =====================================================

    public bool Upgrade()
    {
        if (towerData == null)
            return false;


        if (level >= towerData.maxLevel)
            return false;


        level++;


        ApplyStats();


        // レベルに合わせて見た目を変更
        UpdateLevelVisual();


        // 攻撃範囲表示を更新
        UpdateAttackRangeVisual();


        return true;
    }


    // =====================================================
    // レベル別Prefab
    // =====================================================

    /// <summary>
    /// 現在のレベルに対応した見た目を生成する。
    /// </summary>
    private void UpdateLevelVisual()
    {
        if (towerData == null)
            return;


        GameObject prefab =
            towerData.GetLevelPrefab(level);


        if (prefab == null)
        {
            Debug.LogWarning(
                $"{gameObject.name}: " +
                $"Lv.{level}の見た目Prefabが設定されていません。",
                this
            );

            return;
        }


        // -------------------------------------------------
        // 既存の見た目を削除
        // -------------------------------------------------

        if (visualRoot != null)
        {
            Destroy(
                visualRoot.gameObject
            );

            visualRoot = null;
        }


        // -------------------------------------------------
        // 新しい見た目を生成
        // -------------------------------------------------

        GameObject visual =
            Instantiate(
                prefab,
                transform
            );


        visual.name =
            "TowerVisual";


        // タワー本体を基準にする
        visual.transform.localPosition =
            Vector3.zero;


        visual.transform.localRotation =
            Quaternion.identity;


        visual.transform.localScale =
            Vector3.one;


        visualRoot =
            visual.transform;
    }


    // =====================================================
    // 次のレベル
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
            + towerData.upgradeDamage *
            (nextLevel - 1);
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
            - towerData.upgradeIntervalReduction *
            (nextLevel - 1);


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
            + towerData.upgradeRange *
            (nextLevel - 1);
    }


    // =====================================================
    // 強化費用
    // =====================================================

    public int GetUpgradeCost()
    {
        if (towerData == null)
            return 0;


        return
            towerData.upgradeCost * level;
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
            totalCost *
            towerData.sellRate
        );
    }


    // =====================================================
    // 敵検知
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
                bestProgress =
                    progress;

                bestEnemy =
                    enemy;
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


    // =====================================================
    // 攻撃範囲表示
    // =====================================================

    private void UpdateAttackRangeVisual()
    {
        if (towerData == null)
            return;


        if (attackRangeVisualPrefab == null)
            return;


        if (attackRangeVisualInstance == null)
        {
            attackRangeVisualInstance =
                Instantiate(
                    attackRangeVisualPrefab,
                    transform
                );


            attackRangeVisualInstance.name =
                "AttackRangeVisual";
        }


        attackRangeVisualInstance.transform.localPosition =
            Vector3.zero;


        attackRangeVisualInstance.transform.localRotation =
            Quaternion.identity;


        float diameter =
            currentAttackRange * 2f;


        attackRangeVisualInstance.transform.localScale =
            new Vector3(
                diameter,
                diameter,
                1f
            );


        attackRangeVisualInstance.SetActive(true);
    }


    public void ShowAttackRange()
    {
        if (attackRangeVisualInstance == null)
        {
            UpdateAttackRangeVisual();
            return;
        }


        attackRangeVisualInstance.SetActive(true);
    }


    public void HideAttackRange()
    {
        if (attackRangeVisualInstance != null)
        {
            attackRangeVisualInstance.SetActive(false);
        }
    }
}