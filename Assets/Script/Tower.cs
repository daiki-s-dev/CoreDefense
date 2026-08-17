using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// タワーの攻撃・レベル・強化・売却を管理する。
/// 範囲内で最も進んでいる敵を攻撃する。
/// </summary>
public class Tower : MonoBehaviour
{
    [Header("タワーデータ")]
    public TowerData towerData;


    // =====================================================
    // レベル
    // =====================================================

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

    private CircleCollider2D rangeCollider;


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
    // 初期化
    // =====================================================

    private void Awake()
    {
        rangeCollider =
            GetComponent<CircleCollider2D>();


        if (rangeCollider == null)
        {
            Debug.LogError(
                $"{gameObject.name}: Circle Collider 2Dがありません。",
                this
            );

            return;
        }


        rangeCollider.isTrigger = true;


        ApplyStats();
    }


    private void Start()
    {
        // 初期レベルの見た目を適用
        UpdateVisual();
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
    // クリック
    // =====================================================

    private void OnMouseDown()
    {
        // UIが開いている間は
        // タワーをクリックできないようにする
        if (TowerBuildUI.IsUIOpen)
            return;


        if (TowerPlacementManager.Instance == null)
            return;


        TowerPlacementManager.Instance.OpenTowerUI(
            this
        );
    }


    // =====================================================
    // ステータス計算
    // =====================================================

    private void ApplyStats()
    {
        if (towerData == null)
            return;


        currentAttackDamage =
            towerData.attackDamage
            + towerData.upgradeDamage * (level - 1);


        currentAttackRange =
            towerData.attackRange
            + towerData.upgradeRange * (level - 1);


        currentAttackInterval =
            towerData.attackInterval
            - towerData.upgradeIntervalReduction
            * (level - 1);


        // 0.1秒未満にならないようにする
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


        // レベルアップ
        level++;


        // ステータス更新
        ApplyStats();


        // 見た目更新
        UpdateVisual();


        return true;
    }


    // =====================================================
    // 次のレベルの攻撃力
    // =====================================================

    public int GetNextAttackDamage()
    {
        if (towerData == null)
            return currentAttackDamage;


        if (level >= towerData.maxLevel)
            return currentAttackDamage;


        return
            towerData.attackDamage
            + towerData.upgradeDamage * level;
    }


    // =====================================================
    // 次のレベルの攻撃間隔
    // =====================================================

    public float GetNextAttackInterval()
    {
        if (towerData == null)
            return currentAttackInterval;


        if (level >= towerData.maxLevel)
            return currentAttackInterval;


        float nextInterval =
            towerData.attackInterval
            - towerData.upgradeIntervalReduction
            * level;


        return Mathf.Max(
            0.1f,
            nextInterval
        );
    }


    // =====================================================
    // 次のレベルの射程
    // =====================================================

    public float GetNextAttackRange()
    {
        if (towerData == null)
            return currentAttackRange;


        if (level >= towerData.maxLevel)
            return currentAttackRange;


        return
            towerData.attackRange
            + towerData.upgradeRange * level;
    }


    // =====================================================
    // 強化費
    // =====================================================

    public int GetUpgradeCost()
    {
        if (towerData == null)
            return 0;


        return towerData.upgradeCost * level;
    }


    // =====================================================
    // 売却価格
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
    // 見た目変更
    // =====================================================

    private void UpdateVisual()
    {
        if (towerData == null)
            return;


        GameObject prefab =
            GetLevelPrefab();


        if (prefab == null)
        {
            Debug.LogWarning(
                $"TowerData「{towerData.towerName}」のLv.{level}用Prefabが設定されていません。",
                this
            );

            return;
        }


        // 現在の見た目を削除
        ClearVisual();


        // 新しいPrefabを生成
        GameObject visual =
            Instantiate(
                prefab,
                transform.position,
                Quaternion.identity,
                transform
            );


        // 見た目PrefabのTransformを調整
        visual.transform.localPosition =
            Vector3.zero;

        visual.transform.localRotation =
            Quaternion.identity;

        visual.transform.localScale =
            Vector3.one;
    }


    // =====================================================
    // レベルPrefab取得
    // =====================================================

    private GameObject GetLevelPrefab()
    {
        switch (level)
        {
            case 1:
                return towerData.level1Prefab;

            case 2:
                return towerData.level2Prefab;

            case 3:
                return towerData.level3Prefab;

            default:
                return towerData.level3Prefab;
        }
    }


    // =====================================================
    // 現在の見た目削除
    // =====================================================

    private void ClearVisual()
    {
        // Tower本体の子オブジェクトを削除
        for (int i = transform.childCount - 1;
             i >= 0;
             i--)
        {
            Transform child =
                transform.GetChild(i);


            Destroy(child.gameObject);
        }
    }


    // =====================================================
    // 敵が範囲内に入った
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


    // =====================================================
    // 敵が範囲外に出た
    // =====================================================

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


    // =====================================================
    // 無効な敵を削除
    // =====================================================

    private void RemoveInvalidEnemies()
    {
        enemiesInRange.RemoveAll(
            enemy =>
                enemy == null ||
                !enemy.gameObject.activeInHierarchy
        );
    }


    // =====================================================
    // 最も進んでいる敵
    // =====================================================

    private Enemy FindMostAdvancedEnemy()
    {
        Enemy bestEnemy = null;


        float bestProgress =
            float.MinValue;


        foreach (
            Enemy enemy
            in enemiesInRange)
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
}