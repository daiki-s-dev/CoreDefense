using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Wave全体を管理する。
///
/// ・Wave開始ボタン
/// ・Wave進行
/// ・敵の残数管理
/// ・Waveクリア判定
/// ・次Wave開始ボタン表示
/// </summary>
public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance { get; private set; }


    [Header("Wave")]
    public WaveData[] waves;


    [Header("4方向Spawner")]
    public EnemySpawner northSpawner;

    public EnemySpawner southSpawner;

    public EnemySpawner eastSpawner;

    public EnemySpawner westSpawner;


    [Header("Wave UI")]
    [Tooltip("Wave開始ボタン")]
    public Button waveStartButton;


    [Tooltip("Wave開始ボタンの表示オブジェクト")]
    public GameObject waveStartButtonObject;


    [Header("状態")]
    [SerializeField]
    private int currentWaveIndex = -1;


    /// <summary>
    /// 現在Wave番号。
    /// </summary>
    public int CurrentWave =>
        currentWaveIndex + 1;


    /// <summary>
    /// Waveが進行中か。
    /// </summary>
    public bool IsWaveRunning { get; private set; }


    /// <summary>
    /// 現在フィールドに存在する敵。
    /// </summary>
    private readonly HashSet<Enemy> activeEnemies =
        new HashSet<Enemy>();


    private Coroutine waveCoroutine;


    private void Awake()
    {
        if (
            Instance != null &&
            Instance != this
        )
        {
            Destroy(gameObject);
            return;
        }


        Instance = this;
    }


    private void Start()
    {
        // 開始ボタンにイベント登録
        if (waveStartButton != null)
        {
            waveStartButton.onClick.AddListener(
                StartCurrentWave
            );
        }


        // 最初はWave 1開始ボタンを表示
        ShowWaveStartButton();
    }


    private void OnDestroy()
    {
        if (waveStartButton != null)
        {
            waveStartButton.onClick.RemoveListener(
                StartCurrentWave
            );
        }


        if (Instance == this)
        {
            Instance = null;
        }
    }


    // =====================================================
    // Wave開始
    // =====================================================

    /// <summary>
    /// 現在のWaveを開始する。
    /// </summary>
    public void StartCurrentWave()
    {
        // すでにWave中なら開始しない
        if (IsWaveRunning)
            return;


        // Waveが存在しない
        if (
            waves == null ||
            waves.Length == 0
        )
        {
            Debug.LogError(
                "WaveManager: WaveDataが設定されていません。",
                this
            );

            return;
        }


        // 次のWaveへ
        currentWaveIndex++;


        // 全Wave終了
        if (
            currentWaveIndex >=
            waves.Length
        )
        {
            Debug.Log(
                "全Wave終了しました。"
            );

            HideWaveStartButton();

            return;
        }


        WaveData wave =
            waves[currentWaveIndex];


        if (wave == null)
        {
            Debug.LogError(
                $"Wave {currentWaveIndex + 1} がnullです。"
            );

            return;
        }


        HideWaveStartButton();


        waveCoroutine =
            StartCoroutine(
                StartWaveCoroutine(wave)
            );
    }


    private IEnumerator StartWaveCoroutine(
        WaveData wave)
    {
        IsWaveRunning = true;


        Debug.Log(
            $"========== {wave.waveName} 開始 =========="
        );


        // 開始前待機
        if (wave.startDelay > 0f)
        {
            yield return new WaitForSeconds(
                wave.startDelay
            );
        }


        // Spawner開始
        StartSpawners(wave);


        // 敵がスポーンし終わるまで待つ
        yield return StartCoroutine(
            WaitForSpawningFinished()
        );


        // ここではまだWaveクリアではない
        //
        // フィールドに敵が残っているため、
        // すべて消滅するまで待つ。
        yield return StartCoroutine(
            WaitForAllEnemiesDefeated()
        );


        // Waveクリア
        WaveCleared();
    }


    // =====================================================
    // Spawner
    // =====================================================

    private void StartSpawners(
        WaveData wave)
    {
        if (
            wave.useNorth &&
            northSpawner != null
        )
        {
            northSpawner.StartWave(wave);
        }


        if (
            wave.useSouth &&
            southSpawner != null
        )
        {
            southSpawner.StartWave(wave);
        }


        if (
            wave.useEast &&
            eastSpawner != null
        )
        {
            eastSpawner.StartWave(wave);
        }


        if (
            wave.useWest &&
            westSpawner != null
        )
        {
            westSpawner.StartWave(wave);
        }
    }


    /// <summary>
    /// Spawnerのスポーンがすべて終了するまで待つ。
    /// </summary>
    private IEnumerator WaitForSpawningFinished()
    {
        while (true)
        {
            bool isSpawning = false;


            if (
                northSpawner != null &&
                northSpawner.IsSpawning
            )
            {
                isSpawning = true;
            }


            if (
                southSpawner != null &&
                southSpawner.IsSpawning
            )
            {
                isSpawning = true;
            }


            if (
                eastSpawner != null &&
                eastSpawner.IsSpawning
            )
            {
                isSpawning = true;
            }


            if (
                westSpawner != null &&
                westSpawner.IsSpawning
            )
            {
                isSpawning = true;
            }


            if (!isSpawning)
                break;


            yield return null;
        }
    }


    // =====================================================
    // 敵管理
    // =====================================================

    /// <summary>
    /// 新しく敵が出現したときに登録する。
    /// </summary>
    public void RegisterEnemy(
        Enemy enemy)
    {
        if (enemy == null)
            return;


        activeEnemies.Add(enemy);


        Debug.Log(
            $"敵出現。現在の敵数：{activeEnemies.Count}"
        );
    }


    /// <summary>
    /// 敵が撃破またはコア到達で消滅したときに呼ばれる。
    /// </summary>
    public void NotifyEnemyRemoved(
        Enemy enemy)
    {
        if (enemy == null)
            return;


        activeEnemies.Remove(enemy);


        Debug.Log(
            $"敵消滅。残り敵数：{activeEnemies.Count}"
        );
    }


    /// <summary>
    /// フィールド上の敵がすべて消えるまで待つ。
    /// </summary>
    private IEnumerator WaitForAllEnemiesDefeated()
    {
        while (true)
        {
            // 念のためnullを削除
            activeEnemies.RemoveWhere(
                enemy => enemy == null
            );


            if (activeEnemies.Count == 0)
                break;


            yield return null;
        }
    }


    // =====================================================
    // Waveクリア
    // =====================================================

    private void WaveCleared()
    {
        IsWaveRunning = false;


        Debug.Log(
            $"========== Wave {CurrentWave} CLEAR =========="
        );


        // まだWaveが残っている
        if (
            currentWaveIndex <
            waves.Length - 1
        )
        {
            ShowWaveStartButton();
        }
        else
        {
            // 全Waveクリア
            Debug.Log(
                "========== GAME CLEAR =========="
            );


            HideWaveStartButton();
        }


        waveCoroutine = null;
    }


    // =====================================================
    // UI
    // =====================================================

    private void ShowWaveStartButton()
    {
        if (waveStartButtonObject != null)
        {
            waveStartButtonObject.SetActive(true);
        }
        else if (waveStartButton != null)
        {
            waveStartButton.gameObject.SetActive(true);
        }
    }


    private void HideWaveStartButton()
    {
        if (waveStartButtonObject != null)
        {
            waveStartButtonObject.SetActive(false);
        }
        else if (waveStartButton != null)
        {
            waveStartButton.gameObject.SetActive(false);
        }
    }


    // =====================================================
    // 強制停止
    // =====================================================

    public void StopAllWaves()
    {
        if (waveCoroutine != null)
        {
            StopCoroutine(
                waveCoroutine
            );

            waveCoroutine = null;
        }


        if (northSpawner != null)
        {
            northSpawner.StopWave();
        }


        if (southSpawner != null)
        {
            southSpawner.StopWave();
        }


        if (eastSpawner != null)
        {
            eastSpawner.StopWave();
        }


        if (westSpawner != null)
        {
            westSpawner.StopWave();
        }


        IsWaveRunning = false;
    }
}