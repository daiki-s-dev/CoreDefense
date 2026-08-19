using UnityEngine;

/// <summary>
/// タワーディフェンスの敵。
///
/// WaypointPathに沿って移動し、最後のWaypointに到達すると
/// コアへダメージを与えて消滅する。
///
/// HP・ダメージ処理・被ダメージ演出・死亡アニメーション・
/// WaveManagerへの登録も管理する。
/// </summary>
public class Enemy : MonoBehaviour
{
    // =====================================================
    // HP設定
    // =====================================================

    [Header("HP設定")]
    [Tooltip("敵の最大HP")]
    public int maxHP = 30;

    public int CurrentHP { get; private set; }


    // =====================================================
    // 報酬設定
    // =====================================================

    [Header("撃破報酬")]
    [Tooltip("この敵を倒したときに獲得するお金")]
    public int killReward = 10;


    // =====================================================
    // 移動設定
    // =====================================================

    [Header("移動設定")]
    public float moveSpeed = 2f;

    public float waypointReachDistance = 0.05f;


    // =====================================================
    // コアへのダメージ
    // =====================================================

    [Header("コアへのダメージ")]
    public int damageToCore = 1;


    // =====================================================
    // 経路
    // =====================================================

    [Header("経路")]
    public WaypointPath waypointPath;


    // =====================================================
    // コア
    // =====================================================

    [Header("コア")]
    public GameObject coreObject;


    // =====================================================
    // アニメーション
    // =====================================================

    [Header("アニメーション")]
    [Tooltip("EnemyAnimationController")]
    [SerializeField]
    private EnemyAnimationController animationController;


    [Tooltip("死亡アニメーション終了後に自動Destroyする")]
    [SerializeField]
    private bool useAutomaticDeathDestroy = false;


    [Tooltip("Animation Eventを使わない場合の死亡待機時間")]
    [SerializeField]
    private float deathAnimationLength = 0.5f;


    // =====================================================
    // 内部状態
    // =====================================================

    private int currentWaypointIndex = 0;

    private bool hasReachedCore = false;

    private bool isDead = false;

    private bool isMoving = true;

    // 死亡処理が開始されたか
    private bool deathStarted = false;


    // =====================================================
    // プロパティ
    // =====================================================

    /// <summary>
    /// 現在のWaypoint番号。
    /// </summary>
    public int CurrentWaypointIndex =>
        currentWaypointIndex;


    /// <summary>
    /// スタート地点からのルート進行度。
    /// 数値が大きいほどコアに近い。
    /// </summary>
    public float RouteProgress { get; private set; }


    // =====================================================
    // Unity Lifecycle
    // =====================================================

    private void Awake()
    {
        // HP初期化
        CurrentHP = maxHP;


        // AnimationControllerが設定されていない場合
        if (animationController == null)
        {
            animationController =
                GetComponent<EnemyAnimationController>();
        }


        // 見つからない場合は警告
        if (animationController == null)
        {
            Debug.LogWarning(
                $"{gameObject.name}: " +
                "EnemyAnimationControllerが設定されていません。",
                this
            );
        }
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


        // 最初のWaypointから開始
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
        // 死亡中
        if (isDead)
            return;


        // コア到達済み
        if (hasReachedCore)
            return;


        // 移動停止中
        if (!isMoving)
            return;


        MoveToWaypoint();
    }


    // =====================================================
    // HP・ダメージ
    // =====================================================

    /// <summary>
    /// 敵にダメージを与える。
    /// </summary>
    public void TakeDamage(int damage)
    {
        // すでに死亡処理中
        if (isDead)
            return;


        // 0以下のダメージは無視
        if (damage <= 0)
            return;


        // HP減少
        CurrentHP -= damage;

        CurrentHP =
            Mathf.Max(
                CurrentHP,
                0
            );


        Debug.Log(
            $"{gameObject.name} が {damage} ダメージ。" +
            $" HP: {CurrentHP}/{maxHP}"
        );


        // =================================================
        // 死亡判定
        // =================================================

        if (CurrentHP <= 0)
        {
            // HPが0になった場合は
            // 被ダメージSEではなく死亡SEを再生
            PlayDeathSE();

            Die();

            return;
        }


        // =================================================
        // 被ダメージSE
        // =================================================

        PlayDamageSE();


        // =================================================
        // 被ダメージ演出
        // =================================================

        if (animationController != null)
        {
            animationController.PlayDamageEffect();
        }
    }


    // =====================================================
    // 被ダメージSE
    // =====================================================

    /// <summary>
    /// 敵がダメージを受けたときのSEを再生する。
    /// </summary>
    private void PlayDamageSE()
    {
        if (AudioManager.Instance == null)
            return;


        AudioManager.Instance.PlaySE(
            AudioManager.SEType.EnemyDamage
        );
    }


    // =====================================================
    // 死亡SE
    // =====================================================

    /// <summary>
    /// 敵が倒されたときのSEを再生する。
    /// </summary>
    private void PlayDeathSE()
    {
        if (AudioManager.Instance == null)
            return;


        AudioManager.Instance.PlaySE(
            AudioManager.SEType.EnemyDeath
        );
    }


    // =====================================================
    // 死亡
    // =====================================================

    /// <summary>
    /// 敵の死亡処理を開始する。
    /// </summary>
    private void Die()
    {
        if (isDead)
            return;


        isDead = true;

        isMoving = false;

        deathStarted = true;


        Debug.Log(
            $"{gameObject.name} を撃破しました。"
        );


        // =================================================
        // 死亡アニメーション
        // =================================================

        if (animationController != null)
        {
            animationController.PlayDeathAnimation();
        }
        else
        {
            // AnimationControllerがない場合
            // 即座に死亡処理
            FinishDeath();

            return;
        }


        // =================================================
        // 死亡アニメーション終了処理
        // =================================================

        if (useAutomaticDeathDestroy)
        {
            float animationLength =
                animationController.GetDeathAnimationLength();


            if (animationLength <= 0f)
            {
                animationLength =
                    deathAnimationLength;
            }


            Invoke(
                nameof(FinishDeath),
                animationLength
            );
        }

        // useAutomaticDeathDestroy=falseの場合は
        // Animation EventからFinishDeathAnimation()
        // を呼び出す。
    }


    /// <summary>
    /// 死亡アニメーション終了時に呼び出す。
    ///
    /// Animation Eventから呼び出すことができる。
    /// </summary>
    public void FinishDeathAnimation()
    {
        if (!deathStarted)
            return;


        // =================================================
        // 撃破報酬
        // =================================================

        if (ResourceManager.Instance != null)
        {
            ResourceManager.Instance.AddMoney(
                killReward
            );

            Debug.Log(
                $"{gameObject.name} の撃破報酬として " +
                $"{killReward}G 獲得しました。"
            );
        }
        else
        {
            Debug.LogWarning(
                "ResourceManagerが存在しないため、" +
                "撃破報酬を受け取れませんでした。",
                this
            );
        }


        // 死亡処理完了
        FinishDeath();
    }


    /// <summary>
    /// 死亡処理を完全に終了する。
    /// </summary>
    private void FinishDeath()
    {
        // すでにDestroy処理済みなら終了
        if (!deathStarted)
            return;


        deathStarted = false;


        // =================================================
        // WaveManagerへ通知
        // =================================================

        if (WaveManager.Instance != null)
        {
            WaveManager.Instance.NotifyEnemyRemoved(
                this
            );
        }


        Debug.Log(
            $"{gameObject.name} の死亡処理が完了しました。"
        );


        // =================================================
        // 敵を削除
        // =================================================

        Destroy(gameObject);
    }


    /// <summary>
    /// 敵が死亡しているか。
    /// </summary>
    public bool IsDead()
    {
        return isDead;
    }


    /// <summary>
    /// HP割合を取得する。
    /// </summary>
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


    // =====================================================
    // RouteProgress
    // =====================================================

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


    // =====================================================
    // Waypoint到達
    // =====================================================

    private void ReachWaypoint()
    {
        RouteProgress =
            currentWaypointIndex;


        // 最後のWaypointか
        if (
            currentWaypointIndex >=
            waypointPath.WaypointCount - 1
        )
        {
            ReachCore();

            return;
        }


        // 次のWaypointへ
        currentWaypointIndex++;


        RouteProgress =
            currentWaypointIndex;
    }


    // =====================================================
    // コア到達
    // =====================================================

    private void ReachCore()
    {
        if (hasReachedCore)
            return;


        hasReachedCore = true;

        isMoving = false;


        // =================================================
        // コアにダメージ
        // =================================================

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


        // =================================================
        // WaveManagerへ通知
        // =================================================

        if (WaveManager.Instance != null)
        {
            WaveManager.Instance.NotifyEnemyRemoved(
                this
            );
        }


        // =================================================
        // コア到達した敵は即消滅
        // =================================================

        Destroy(gameObject);
    }


    // =====================================================
    // 外部設定
    // =====================================================

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

        deathStarted = false;

        CurrentHP = maxHP;

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
}