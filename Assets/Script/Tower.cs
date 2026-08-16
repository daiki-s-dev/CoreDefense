using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// タワーの攻撃を管理する。
///
/// 攻撃範囲内にいるEnemyを検知し、
/// その中からルートを最も先まで進んでいる敵を選択する。
///
/// 弾は使用せず、一定間隔ごとに直接ダメージを与える。
/// </summary>
public class Tower : MonoBehaviour
{
    [Header("攻撃設定")]
    [Tooltip("敵に与えるダメージ")]
    public int attackDamage = 10;

    [Tooltip("攻撃する間隔（秒）")]
    public float attackInterval = 1f;

    [Tooltip("攻撃範囲")]
    public float attackRange = 3f;


    [Header("ターゲット設定")]
    [Tooltip("コアに最も近い敵を優先する")]
    public bool targetFurthestProgressEnemy = true;


    // 攻撃範囲内にいる敵
    private readonly List<Enemy> enemiesInRange =
        new List<Enemy>();


    // 現在攻撃している敵
    private Enemy currentTarget;


    // 次に攻撃できる時間
    private float nextAttackTime = 0f;


    #region Unity Lifecycle

    private void Start()
    {
        // 攻撃範囲を設定
        UpdateAttackRange();
    }


    private void Update()
    {
        // 攻撃対象を探す
        currentTarget = FindTarget();


        // 攻撃対象がいなければ何もしない
        if (currentTarget == null)
            return;


        // 攻撃可能な時間になったら攻撃
        if (Time.time >= nextAttackTime)
        {
            Attack();

            // 次回攻撃時間を設定
            nextAttackTime =
                Time.time + attackInterval;
        }
    }

    #endregion


    #region 敵の検知

    /// <summary>
    /// 攻撃範囲に敵が入ったとき。
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        Enemy enemy =
            other.GetComponent<Enemy>();


        if (enemy == null)
            return;


        // すでに登録されていなければ追加
        if (!enemiesInRange.Contains(enemy))
        {
            enemiesInRange.Add(enemy);
        }
    }


    /// <summary>
    /// 攻撃範囲から敵が出たとき。
    /// </summary>
    private void OnTriggerExit2D(Collider2D other)
    {
        Enemy enemy =
            other.GetComponent<Enemy>();


        if (enemy == null)
            return;


        enemiesInRange.Remove(enemy);


        // 現在のターゲットだった場合
        if (currentTarget == enemy)
        {
            currentTarget = null;
        }
    }

    #endregion


    #region ターゲット選択

    /// <summary>
    /// 攻撃対象を探す。
    ///
    /// RouteProgressが最も大きい敵、
    /// つまりスタート地点から最も先まで進んでいる敵を選択する。
    /// </summary>
    private Enemy FindTarget()
    {
        // 無効な敵を削除
        RemoveInvalidEnemies();


        if (enemiesInRange.Count == 0)
            return null;


        Enemy target = null;


        // 最大の進行度
        float furthestProgress =
            -Mathf.Infinity;


        foreach (Enemy enemy in enemiesInRange)
        {
            if (enemy == null)
                continue;


            if (enemy.IsDead())
                continue;


            // ルート上の進行度を取得
            float progress =
                enemy.RouteProgress;


            // より先まで進んでいる敵を優先
            if (progress > furthestProgress)
            {
                furthestProgress = progress;
                target = enemy;
            }
        }


        return target;
    }


    /// <summary>
    /// 死亡・削除された敵をリストから取り除く。
    /// </summary>
    private void RemoveInvalidEnemies()
    {
        for (int i = enemiesInRange.Count - 1;
             i >= 0;
             i--)
        {
            Enemy enemy =
                enemiesInRange[i];


            if (enemy == null || enemy.IsDead())
            {
                enemiesInRange.RemoveAt(i);
            }
        }
    }

    #endregion


    #region 攻撃

    /// <summary>
    /// 現在のターゲットを攻撃する。
    /// </summary>
    private void Attack()
    {
        if (currentTarget == null)
            return;


        // 敵にダメージを与える
        currentTarget.TakeDamage(
            attackDamage
        );


        Debug.Log(
            $"{gameObject.name} が " +
            $"{currentTarget.gameObject.name} に " +
            $"{attackDamage} ダメージを与えました。" +
            $" RouteProgress: " +
            $"{currentTarget.RouteProgress:F2}"
        );
    }

    #endregion


    #region 攻撃範囲

    /// <summary>
    /// CircleCollider2Dの攻撃範囲を設定する。
    /// </summary>
    private void UpdateAttackRange()
    {
        CircleCollider2D circleCollider =
            GetComponent<CircleCollider2D>();


        if (circleCollider == null)
        {
            Debug.LogWarning(
                $"{gameObject.name}: " +
                "CircleCollider2Dがありません。",
                this
            );

            return;
        }


        circleCollider.radius =
            attackRange;


        circleCollider.isTrigger =
            true;
    }

    #endregion


    #region 外部からの操作

    /// <summary>
    /// タワーの攻撃を停止する。
    /// </summary>
    public void StopAttack()
    {
        enabled = false;
    }


    /// <summary>
    /// タワーの攻撃を再開する。
    /// </summary>
    public void ResumeAttack()
    {
        enabled = true;
    }

    #endregion
}