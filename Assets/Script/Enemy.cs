using UnityEngine;

/// <summary>
/// タワーディフェンスの敵。
/// WaypointPathに沿って移動し、最後のWaypointに到達すると
/// コアへダメージを与えて消滅する。
///
/// HP・ダメージ処理・WaveManagerへの登録も管理する。
/// </summary>
public class Enemy : MonoBehaviour
{
    [Header("HP設定")]
    [Tooltip("敵の最大HP")]
    public int maxHP = 30;

    public int CurrentHP { get; private set; }


    [Header("移動設定")]
    public float moveSpeed = 2f;

    public float waypointReachDistance = 0.05f;


    [Header("コアへのダメージ")]
    public int damageToCore = 1;


    [Header("経路")]
    public WaypointPath waypointPath;


    [Header("コア")]
    public GameObject coreObject;


    private int currentWaypointIndex = 0;

    private bool hasReachedCore = false;

    private bool isDead = false;

    private bool isMoving = true;


    public int CurrentWaypointIndex =>
        currentWaypointIndex;


    public float RouteProgress { get; private set; }


    private void Awake()
    {
        CurrentHP = maxHP;
    }


    private void Start()
    {
        if (waypointPath == null)
        {
            Debug.LogError(
                $"{gameObject.name}: WaypointPathが設定されていません。",
                this
            );

            isMoving = false;
            return;
        }


        if (waypointPath.WaypointCount == 0)
        {
            Debug.LogError(
                $"{gameObject.name}: Waypointが設定されていません。",
                this
            );

            isMoving = false;
            return;
        }


        Transform firstWaypoint =
            waypointPath.GetWaypoint(0);


        if (firstWaypoint != null)
        {
            transform.position =
                firstWaypoint.position;

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
        if (isDead)
            return;


        if (hasReachedCore)
            return;


        if (!isMoving)
            return;


        MoveToWaypoint();
    }


    // =====================================================
    // HP・ダメージ
    // =====================================================

    public void TakeDamage(int damage)
    {
        if (isDead)
            return;


        if (damage <= 0)
            return;


        CurrentHP -= damage;

        CurrentHP =
            Mathf.Max(CurrentHP, 0);


        Debug.Log(
            $"{gameObject.name} が {damage} ダメージ。" +
            $" HP: {CurrentHP}/{maxHP}"
        );


        if (CurrentHP <= 0)
        {
            Die();
        }
    }


    /// <summary>
    /// 敵を撃破した。
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


        // WaveManagerへ撃破を通知
        if (WaveManager.Instance != null)
        {
            WaveManager.Instance.NotifyEnemyRemoved(this);
        }


        Destroy(gameObject);
    }


    public bool IsDead()
    {
        return isDead;
    }


    public float GetHPRatio()
    {
        if (maxHP <= 0)
            return 0f;


        return (float)CurrentHP / maxHP;
    }


    // =====================================================
    // Waypoint移動
    // =====================================================

    private void MoveToWaypoint()
    {
        Transform targetWaypoint =
            waypointPath.GetWaypoint(
                currentWaypointIndex
            );


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


        Vector3 direction =
            targetWaypoint.position -
            transform.position;


        transform.position =
            Vector3.MoveTowards(
                transform.position,
                targetWaypoint.position,
                moveSpeed * Time.deltaTime
            );


        if (
            direction.magnitude <=
            waypointReachDistance
        )
        {
            ReachWaypoint();
        }
        else
        {
            UpdateRouteProgress();
        }
    }


    private void UpdateRouteProgress()
    {
        Transform currentWaypoint =
            waypointPath.GetWaypoint(
                currentWaypointIndex
            );


        if (currentWaypoint == null)
            return;


        if (currentWaypointIndex == 0)
        {
            RouteProgress = 0f;
            return;
        }


        Transform previousWaypoint =
            waypointPath.GetWaypoint(
                currentWaypointIndex - 1
            );


        if (previousWaypoint == null)
            return;


        float segmentLength =
            Vector3.Distance(
                previousWaypoint.position,
                currentWaypoint.position
            );


        if (segmentLength <= 0f)
            return;


        float distanceFromPrevious =
            Vector3.Distance(
                previousWaypoint.position,
                transform.position
            );


        float segmentProgress =
            Mathf.Clamp01(
                distanceFromPrevious /
                segmentLength
            );


        RouteProgress =
            (currentWaypointIndex - 1) +
            segmentProgress;
    }


    private void ReachWaypoint()
    {
        RouteProgress =
            currentWaypointIndex;


        if (
            currentWaypointIndex >=
            waypointPath.WaypointCount - 1
        )
        {
            ReachCore();

            return;
        }


        currentWaypointIndex++;

        RouteProgress =
            currentWaypointIndex;
    }


    /// <summary>
    /// コアに到達した。
    /// </summary>
    private void ReachCore()
    {
        if (hasReachedCore)
            return;


        hasReachedCore = true;

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


        // WaveManagerへ消滅を通知
        if (WaveManager.Instance != null)
        {
            WaveManager.Instance.NotifyEnemyRemoved(this);
        }


        // コア到達した敵を消滅
        Destroy(gameObject);
    }


    // =====================================================
    // 外部設定
    // =====================================================

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

        CurrentHP = maxHP;

        RouteProgress = 0f;
    }


    public void StopMovement()
    {
        isMoving = false;
    }


    public void ResumeMovement()
    {
        if (isDead)
            return;


        if (hasReachedCore)
            return;


        isMoving = true;
    }
}