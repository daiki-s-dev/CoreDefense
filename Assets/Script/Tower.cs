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

    // 現在のレベル
    [SerializeField]
    private int level = 1;

    // 現在の攻撃力
    private int currentAttackDamage;

    // 現在の攻撃間隔
    private float currentAttackInterval;

    // 現在の攻撃範囲
    private float currentAttackRange;

    // 攻撃範囲内の敵
    private readonly List<Enemy> enemiesInRange =
        new List<Enemy>();

    // 現在のターゲット
    private Enemy currentTarget;

    // 次に攻撃できる時間
    private float nextAttackTime;

    // 攻撃範囲Collider
    private CircleCollider2D rangeCollider;


    /// <summary>
    /// 現在のレベル。
    /// </summary>
    public int Level => level;

    /// <summary>
    /// 現在の攻撃力。
    /// </summary>
    public int CurrentAttackDamage =>
        currentAttackDamage;

    /// <summary>
    /// 現在の攻撃間隔。
    /// </summary>
    public float CurrentAttackInterval =>
        currentAttackInterval;

    /// <summary>
    /// 現在の攻撃範囲。
    /// </summary>
    public float CurrentAttackRange =>
        currentAttackRange;


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


    /// <summary>
    /// タワーをクリックしたとき。
    /// </summary>
    private void OnMouseDown()
    {
        if (TowerPlacementManager.Instance == null)
            return;

        TowerPlacementManager.Instance.OpenTowerUI(this);
    }


    /// <summary>
    /// TowerDataを基準に現在の性能を計算する。
    /// </summary>
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
            - towerData.upgradeIntervalReduction * (level - 1);

        // 攻撃間隔が0以下にならないようにする
        currentAttackInterval =
            Mathf.Max(0.1f, currentAttackInterval);

        if (rangeCollider != null)
        {
            rangeCollider.radius =
                currentAttackRange;
        }
    }


    /// <summary>
    /// タワーを強化する。
    /// </summary>
    public bool Upgrade()
    {
        if (towerData == null)
            return false;

        if (level >= towerData.maxLevel)
            return false;

        level++;

        ApplyStats();

        return true;
    }


    /// <summary>
    /// 次のレベルに必要な強化費用。
    /// </summary>
    public int GetUpgradeCost()
    {
        if (towerData == null)
            return 0;

        return towerData.upgradeCost * level;
    }


    /// <summary>
    /// 売却時に返ってくる金額。
    /// </summary>
    public int GetSellPrice()
    {
        if (towerData == null)
            return 0;

        int totalCost =
            towerData.buildCost;

        // これまでの強化費用も含める
        for (int i = 1; i < level; i++)
        {
            totalCost +=
                towerData.upgradeCost * i;
        }

        return Mathf.FloorToInt(
            totalCost * towerData.sellRate
        );
    }


    /// <summary>
    /// 攻撃範囲に敵が入った。
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
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


    /// <summary>
    /// 攻撃範囲から敵が出た。
    /// </summary>
    private void OnTriggerExit2D(Collider2D other)
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


    /// <summary>
    /// 無効になった敵を削除する。
    /// </summary>
    private void RemoveInvalidEnemies()
    {
        enemiesInRange.RemoveAll(
            enemy =>
                enemy == null ||
                !enemy.gameObject.activeInHierarchy
        );
    }


    /// <summary>
    /// 最も進んでいる敵を取得する。
    /// </summary>
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


    /// <summary>
    /// 敵を攻撃する。
    /// </summary>
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