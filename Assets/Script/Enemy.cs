using UnityEngine;

/// <summary>
/// タワーディフェンスの敵。
/// WaypointPathに沿って移動し、最後のWaypointに到達すると
/// コアへダメージを与えて消滅する。
///
/// また、タワーからダメージを受けるためのHP・ダメージ処理も管理する。
///
/// RouteProgressによって、ルート上の進行度を管理する。
/// RouteProgressが大きい敵ほどコアに近い。
/// </summary>
public class Enemy : MonoBehaviour
{
    [Header("HP設定")]
    [Tooltip("敵の最大HP")]
    public int maxHP = 30;

    // 現在のHP
    public int CurrentHP { get; private set; }


    [Header("移動設定")]
    [Tooltip("敵の移動速度")]
    public float moveSpeed = 2f;

    [Tooltip("Waypointに到達したと判定する距離")]
    public float waypointReachDistance = 0.05f;


    [Header("コアへのダメージ")]
    [Tooltip("コアに与えるダメージ")]
    public int damageToCore = 1;


    [Header("経路")]
    [Tooltip("敵が移動するWaypointPath")]
    public WaypointPath waypointPath;

    [Header("コア")]
    [Tooltip("最後に到達するコア")]
    public GameObject coreObject;


    // 現在目指しているWaypointの番号
    private int currentWaypointIndex = 0;

    // コアに到達したか
    private bool hasReachedCore = false;

    // 死亡したか
    private bool isDead = false;

    // 移動中か
    private bool isMoving = true;


    /// <summary>
    /// 現在のWaypoint番号。
    /// 外部から参照できるようにしている。
    /// </summary>
    public int CurrentWaypointIndex => currentWaypointIndex;


    /// <summary>
    /// スタート地点からのルート上の進行度。
    ///
    /// 数値が大きいほどコアに近い。
    ///
    /// 例：
    /// Waypoint 2と3の間を50%進んでいる場合 → 2.5
    /// </summary>
    public float RouteProgress { get; private set; }


    #region Unity Lifecycle

    private void Awake()
    {
        // ゲーム開始時にHPを最大値にする
        CurrentHP = maxHP;
    }


    private void Start()
    {
        // WaypointPathが設定されていない場合
        if (waypointPath == null)
        {
            Debug.LogError(
                $"{gameObject.name}: WaypointPathが設定されていません。",
                this
            );

            isMoving = false;
            return;
        }

        // Waypointが1つもない場合
        if (waypointPath.WaypointCount == 0)
        {
            Debug.LogError(
                $"{gameObject.name}: Waypointが設定されていません。",
                this
            );

            isMoving = false;
            return;
        }

        // 最初のWaypointから開始
        Transform firstWaypoint =
            waypointPath.GetWaypoint(0);

        if (firstWaypoint != null)
        {
            transform.position = firstWaypoint.position;
            currentWaypointIndex = 0;
            RouteProgress = 0f;
        }
        else
        {
            Debug.LogError(
                $"{gameObject.name}: 最初のWaypointが取得できません。",
                this
            );

            isMoving = false;
        }
    }


    private void Update()
    {
        // 死亡している場合
        if (isDead)
            return;

        // コアに到達済みの場合
        if (hasReachedCore)
            return;

        // 移動停止中の場合
        if (!isMoving)
            return;

        MoveToWaypoint();
    }

    #endregion


    #region HP・ダメージ

    /// <summary>
    /// 敵にダメージを与える。
    /// タワーから呼び出すことを想定している。
    /// </summary>
    public void TakeDamage(int damage)
    {
        // すでに死亡している場合
        if (isDead)
            return;

        // 0以下のダメージは無視
        if (damage <= 0)
            return;

        // HPを減らす
        CurrentHP -= damage;

        // HPがマイナスにならないようにする
        CurrentHP = Mathf.Max(CurrentHP, 0);

        Debug.Log(
            $"{gameObject.name} が {damage} ダメージを受けました。" +
            $" HP: {CurrentHP}/{maxHP}"
        );

        // HPが0になったら死亡
        if (CurrentHP <= 0)
        {
            Die();
        }
    }


    /// <summary>
    /// 敵が死亡したときの処理。
    /// </summary>
    private void Die()
    {
        if (isDead)
            return;

        isDead = true;
        isMoving = false;

        Debug.Log(
            $"{gameObject.name} を撃破しました。"
        );

        // 後でここに撃破報酬などを追加できる。
        //
        // 例：
        // ResourceManager.Instance.AddResource(10);

        Destroy(gameObject);
    }


    /// <summary>
    /// 敵が死亡しているか取得する。
    /// </summary>
    public bool IsDead()
    {
        return isDead;
    }


    /// <summary>
    /// 現在HPの割合を取得する。
    /// HPバーなどで使用できる。
    /// </summary>
    public float GetHPRatio()
    {
        if (maxHP <= 0)
            return 0f;

        return (float)CurrentHP / maxHP;
    }

    #endregion


    #region Waypoint移動

    /// <summary>
    /// 現在のWaypointへ移動する。
    /// </summary>
    private void MoveToWaypoint()
    {
        Transform targetWaypoint =
            waypointPath.GetWaypoint(currentWaypointIndex);

        if (targetWaypoint == null)
        {
            Debug.LogError(
                $"{gameObject.name}: Waypoint " +
                $"{currentWaypointIndex} が見つかりません。",
                this
            );

            isMoving = false;
            return;
        }


        // 移動前の位置
        Vector3 previousPosition =
            transform.position;


        // Waypointへ向かう方向
        Vector3 direction =
            targetWaypoint.position - transform.position;


        // Waypointへ移動
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetWaypoint.position,
            moveSpeed * Time.deltaTime
        );


        // Waypointに到達したか確認
        if (direction.magnitude <= waypointReachDistance)
        {
            ReachWaypoint();
        }
        else
        {
            // 現在のWaypointへ向かってどの程度進んだかを更新
            UpdateRouteProgress();
        }
    }


    /// <summary>
    /// 現在のWaypoint間での進行度を計算する。
    ///
    /// 例えば、
    /// Waypoint 2 → Waypoint 3 の途中50%なら
    /// RouteProgress = 2.5
    /// </summary>
    private void UpdateRouteProgress()
    {
        // 現在のWaypoint
        Transform currentWaypoint =
            waypointPath.GetWaypoint(currentWaypointIndex);

        if (currentWaypoint == null)
            return;


        // Waypoint 0の場合は進行度0
        if (currentWaypointIndex == 0)
        {
            RouteProgress = 0f;
            return;
        }


        // ひとつ前のWaypoint
        Transform previousWaypoint =
            waypointPath.GetWaypoint(
                currentWaypointIndex - 1
            );

        if (previousWaypoint == null)
            return;


        // 前のWaypointから現在のWaypointまでの距離
        float segmentLength =
            Vector3.Distance(
                previousWaypoint.position,
                currentWaypoint.position
            );

        if (segmentLength <= 0f)
            return;


        // 前のWaypointから現在位置までの距離
        float distanceFromPrevious =
            Vector3.Distance(
                previousWaypoint.position,
                transform.position
            );


        // 0～1の範囲に収める
        float segmentProgress =
            Mathf.Clamp01(
                distanceFromPrevious / segmentLength
            );


        // Waypoint番号 + 区間内の進行度
        RouteProgress =
            (currentWaypointIndex - 1)
            + segmentProgress;
    }


    /// <summary>
    /// Waypointに到達したときの処理。
    /// </summary>
    private void ReachWaypoint()
    {
        // Waypointに到達したので進行度を更新
        RouteProgress = currentWaypointIndex;


        // 最後のWaypointか確認
        if (currentWaypointIndex >=
            waypointPath.WaypointCount - 1)
        {
            ReachCore();
            return;
        }


        // 次のWaypointへ
        currentWaypointIndex++;


        // 次のWaypointへ進んだことを記録
        RouteProgress = currentWaypointIndex;
    }


    /// <summary>
    /// 最後のWaypoint（コア）に到達したときの処理。
    /// </summary>
    private void ReachCore()
    {
        // すでに処理済みなら何もしない
        if (hasReachedCore)
            return;


        // コア到達状態
        hasReachedCore = true;

        // 移動停止
        isMoving = false;


        // コアにダメージ
        if (coreObject != null)
        {
            coreObject.SendMessage(
                "TakeDamage",
                damageToCore,
                SendMessageOptions.DontRequireReceiver
            );
        }
        else
        {
            Debug.LogWarning(
                $"{gameObject.name}: " +
                "coreObjectが設定されていません。",
                this
            );
        }


        // コアに到達した敵は消滅
        Destroy(gameObject);
    }

    #endregion


    #region 外部からの設定

    /// <summary>
    /// EnemySpawnerなどから移動ルートを設定する。
    /// </summary>
    public void SetPath(
        WaypointPath path,
        GameObject core)
    {
        waypointPath = path;
        coreObject = core;

        currentWaypointIndex = 0;

        hasReachedCore = false;
        isDead = false;
        isMoving = true;

        // HPを初期化
        CurrentHP = maxHP;

        // 進行度を初期化
        RouteProgress = 0f;
    }


    /// <summary>
    /// 敵の移動を停止する。
    /// </summary>
    public void StopMovement()
    {
        isMoving = false;
    }


    /// <summary>
    /// 敵の移動を再開する。
    /// </summary>
    public void ResumeMovement()
    {
        if (isDead)
            return;

        if (hasReachedCore)
            return;

        isMoving = true;
    }

    #endregion
}