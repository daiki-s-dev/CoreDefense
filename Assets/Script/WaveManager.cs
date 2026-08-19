using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Wave全体を管理する。
///
/// ・Wave開始ボタン
/// ・Wave進行
/// ・4方向Spawner
/// ・敵の総数管理
/// ・敵の残数管理
/// ・Waveクリア判定
/// ・Wave UI
/// ・次Wave開始表示
/// </summary>
public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance { get; private set; }


    // =====================================================
    // Wave
    // =====================================================

    [Header("Wave")]
    public WaveData[] waves;


    // =====================================================
    // 4方向Spawner
    // =====================================================

    [Header("4方向Spawner")]

    public EnemySpawner northSpawner;

    public EnemySpawner southSpawner;

    public EnemySpawner eastSpawner;

    public EnemySpawner westSpawner;


    // =====================================================
    // Wave開始ボタン
    // =====================================================

    [Header("Wave開始ボタン")]

    [Tooltip("Wave開始ボタン")]
    public Button waveStartButton;


    [Tooltip("Wave開始ボタン全体")]
    public GameObject waveStartButtonObject;


    // =====================================================
    // Wave UI
    // =====================================================

    [Header("Wave UI")]

    [Tooltip("現在のWave")]
    public TMP_Text waveText;


    [Tooltip("残り敵数")]
    public TMP_Text enemyCountText;


    [Tooltip("Waveクリア表示")]
    public TMP_Text waveClearText;


    [Tooltip("次のWave表示")]
    public TMP_Text nextWaveText;


    [Tooltip("Waveクリア表示時間")]
    public float waveClearDisplayTime = 2f;


    // =====================================================
    // 状態
    // =====================================================

    [Header("状態")]

    [SerializeField]
    private int currentWaveIndex = -1;


    /// <summary>
    /// 現在のWave番号。
    /// </summary>
    public int CurrentWave
    {
        get
        {
            return currentWaveIndex + 1;
        }
    }


    /// <summary>
    /// Waveが進行中か。
    /// </summary>
    public bool IsWaveRunning
    {
        get;
        private set;
    }


    // =====================================================
    // 敵数
    // =====================================================

    /// <summary>
    /// Wave全体の敵総数。
    ///
    /// 例：
    /// North 5
    /// South 5
    /// East 10
    /// West 0
    ///
    /// → 20
    /// </summary>
    public int TotalEnemyCount
    {
        get;
        private set;
    }


    /// <summary>
    /// 現在残っている敵数。
    /// </summary>
    public int RemainingEnemyCount
    {
        get;
        private set;
    }


    /// <summary>
    /// 現在フィールド上に存在する敵。
    /// </summary>
    private readonly HashSet<Enemy> activeEnemies =
        new HashSet<Enemy>();


    // =====================================================
    // Coroutine
    // =====================================================

    private Coroutine waveCoroutine;

    private Coroutine clearUICoroutine;


    // =====================================================
    // Unity
    // =====================================================

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
        // -------------------------------------------------
        // Wave開始ボタン
        // -------------------------------------------------

        if (waveStartButton != null)
        {
            waveStartButton.onClick.AddListener(
                StartCurrentWave
            );
        }


        // -------------------------------------------------
        // 最初の状態
        // -------------------------------------------------

        IsWaveRunning = false;

        TotalEnemyCount = 0;

        RemainingEnemyCount = 0;


        HideWaveText();

        HideEnemyCountText();

        HideWaveClearText();


        // 最初はWave 1
        ShowNextWaveText(1);


        // Wave開始ボタンを表示
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


        // WaveDataがない
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
                $"Wave {currentWaveIndex + 1} がnullです。",
                this
            );

            return;
        }


        // -------------------------------------------------
        // 敵数を初期化
        // -------------------------------------------------

        TotalEnemyCount =
            wave.GetTotalEnemyCount();


        RemainingEnemyCount =
            TotalEnemyCount;


        // 念のため前Waveの敵情報をクリア
        activeEnemies.Clear();


        // -------------------------------------------------
        // UI
        // -------------------------------------------------

        HideWaveStartButton();

        HideNextWaveText();

        HideWaveClearText();


        ShowWaveText(
            CurrentWave
        );


        ShowEnemyCountText();


        // -------------------------------------------------
        // Coroutine
        // -------------------------------------------------

        waveCoroutine =
            StartCoroutine(
                StartWaveCoroutine(wave)
            );
    }


    // =====================================================
    // Wave進行
    // =====================================================

    private IEnumerator StartWaveCoroutine(
        WaveData wave)
    {
        IsWaveRunning = true;


        Debug.Log(
            $"========== {wave.waveName} 開始 =========="
        );


        // -------------------------------------------------
        // 開始前待機
        // -------------------------------------------------

        if (wave.startDelay > 0f)
        {
            yield return new WaitForSeconds(
                wave.startDelay
            );
        }


        // -------------------------------------------------
        // Wave BGMへ切り替え
        // -------------------------------------------------

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayBGM(
                AudioManager.BGMType.GameWave
            );
        }


        // -------------------------------------------------
        // Wave開始SE
        // -------------------------------------------------

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySE(
                AudioManager.SEType.WaveStart
            );
        }


        // -------------------------------------------------
        // Spawner開始
        // -------------------------------------------------

        StartSpawners(wave);


        // -------------------------------------------------
        // 敵スポーン終了待ち
        // -------------------------------------------------

        yield return StartCoroutine(
            WaitForSpawningFinished()
        );


        // -------------------------------------------------
        // 全敵消滅待ち
        // -------------------------------------------------

        yield return StartCoroutine(
            WaitForAllEnemiesDefeated()
        );


        // -------------------------------------------------
        // Waveクリア
        // -------------------------------------------------

        WaveCleared();
    }


    // =====================================================
    // Spawner開始
    // =====================================================

    private void StartSpawners(
        WaveData wave)
    {
        if (
            wave.useNorth &&
            northSpawner != null
        )
        {
            northSpawner.StartWave(
                wave
            );
        }


        if (
            wave.useSouth &&
            southSpawner != null
        )
        {
            southSpawner.StartWave(
                wave
            );
        }


        if (
            wave.useEast &&
            eastSpawner != null
        )
        {
            eastSpawner.StartWave(
                wave
            );
        }


        if (
            wave.useWest &&
            westSpawner != null
        )
        {
            westSpawner.StartWave(
                wave
            );
        }
    }


    // =====================================================
    // スポーン終了待ち
    // =====================================================

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
    // 敵登録
    // =====================================================

    /// <summary>
    /// 新しく敵が出現したときに登録する。
    /// </summary>
    public void RegisterEnemy(
        Enemy enemy)
    {
        if (enemy == null)
            return;


        activeEnemies.Add(
            enemy
        );


        Debug.Log(
            $"敵出現。現在のフィールド敵数：{activeEnemies.Count}"
        );


        UpdateEnemyCountUI();
    }


    // =====================================================
    // 敵消滅
    // =====================================================

    /// <summary>
    /// 敵が撃破またはコア到達で消滅したときに呼ばれる。
    /// </summary>
    public void NotifyEnemyRemoved(
        Enemy enemy)
    {
        if (enemy == null)
            return;


        // すでに削除済みなら何もしない
        bool removed =
            activeEnemies.Remove(
                enemy
            );


        if (!removed)
            return;


        // 残数を1減らす
        RemainingEnemyCount =
            Mathf.Max(
                0,
                RemainingEnemyCount - 1
            );


        Debug.Log(
            $"敵消滅。残り敵数：{RemainingEnemyCount}/{TotalEnemyCount}"
        );


        // UI更新
        UpdateEnemyCountUI();
    }


    // =====================================================
    // 全敵消滅待ち
    // =====================================================

    private IEnumerator WaitForAllEnemiesDefeated()
    {
        while (true)
        {
            // nullになったEnemyを削除
            activeEnemies.RemoveWhere(
                enemy => enemy == null
            );


            // 全敵が消滅
            if (
                activeEnemies.Count == 0 &&
                RemainingEnemyCount <= 0
            )
            {
                break;
            }


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


        // =================================================
        // 開始前BGMへ戻す
        // =================================================

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayBGM(
                AudioManager.BGMType.GamePreparation
            );
        }


        // -------------------------------------------------
        // Waveクリア表示
        // -------------------------------------------------

        ShowWaveClearText(
            CurrentWave
        );


        // -------------------------------------------------
        // WaveTextを消す
        // -------------------------------------------------

        HideWaveText();

        HideEnemyCountText();


        // -------------------------------------------------
        // 次Waveがあるか
        // -------------------------------------------------

        if (
            currentWaveIndex <
            waves.Length - 1
        )
        {
            // 次Wave番号
            int nextWave =
                CurrentWave + 1;


            // endDelay後に次Waveボタンと
            // 「次はWave X」を表示
            StartCoroutine(
                ShowNextWaveAfterDelay(
                    nextWave
                )
            );
        }
        else
        {
            // 全Waveクリア
            Debug.Log(
                "========== GAME CLEAR =========="
            );


            // =================================================
            // BGM停止
            // =================================================

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.StopBGM();
            }


            // =================================================
            // ゲームクリアSE
            // =================================================

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySE(
                    AudioManager.SEType.GameClear
                );
            }


            HideWaveStartButton();
            HideNextWaveText();


            // ClearSceneへ移動
            if (SceneController.Instance != null)
            {
                SceneController.Instance.LoadClear();
            }
            else
            {
                Debug.LogError(
                    "WaveManager: SceneController.Instance が見つかりません。"
                );
            }
        }


        waveCoroutine = null;
    }


    // =====================================================
    // 次Wave表示
    // =====================================================

    private IEnumerator ShowNextWaveAfterDelay(
        int nextWave)
    {
        float delay = 0f;


        if (
            currentWaveIndex >= 0 &&
            currentWaveIndex < waves.Length
        )
        {
            WaveData currentWave =
                waves[currentWaveIndex];


            if (currentWave != null)
            {
                delay =
                    Mathf.Max(
                        0f,
                        currentWave.endDelay
                    );
            }
        }


        if (delay > 0f)
        {
            yield return new WaitForSeconds(
                delay
            );
        }


        // 次Wave表示
        ShowNextWaveText(
            nextWave
        );


        // 開始ボタン表示
        ShowWaveStartButton();
    }


    // =====================================================
    // UI
    // =====================================================

    /// <summary>
    /// Wave番号を表示。
    /// </summary>
    private void ShowWaveText(
        int waveNumber)
    {
        if (waveText == null)
            return;


        waveText.text =
            $"Wave {waveNumber}";


        waveText.gameObject.SetActive(
            true
        );
    }


    /// <summary>
    /// Wave番号を非表示。
    /// </summary>
    private void HideWaveText()
    {
        if (waveText == null)
            return;


        waveText.gameObject.SetActive(
            false
        );
    }


    // =====================================================
    // 敵数UI
    // =====================================================

    /// <summary>
    /// 敵数UIを表示。
    /// </summary>
    private void ShowEnemyCountText()
    {
        if (enemyCountText == null)
            return;


        enemyCountText.gameObject.SetActive(
            true
        );


        UpdateEnemyCountUI();
    }


    /// <summary>
    /// 敵数UIを更新。
    ///
    /// 例：
    /// 敵 5 / 20
    /// </summary>
    private void UpdateEnemyCountUI()
    {
        if (enemyCountText == null)
            return;


        enemyCountText.text =
            $"敵 {RemainingEnemyCount} / {TotalEnemyCount}";
    }


    /// <summary>
    /// 敵数UIを非表示。
    /// </summary>
    private void HideEnemyCountText()
    {
        if (enemyCountText == null)
            return;


        enemyCountText.gameObject.SetActive(
            false
        );
    }


    // =====================================================
    // WaveクリアUI
    // =====================================================

    private void ShowWaveClearText(
        int waveNumber)
    {
        if (waveClearText == null)
            return;


        if (clearUICoroutine != null)
        {
            StopCoroutine(
                clearUICoroutine
            );
        }


        clearUICoroutine =
            StartCoroutine(
                WaveClearTextCoroutine(
                    waveNumber
                )
            );
    }


    private IEnumerator WaveClearTextCoroutine(
        int waveNumber)
    {
        waveClearText.text =
            $"Wave {waveNumber} CLEAR!";


        waveClearText.gameObject.SetActive(
            true
        );


        yield return new WaitForSeconds(
            waveClearDisplayTime
        );


        HideWaveClearText();


        clearUICoroutine = null;
    }


    private void HideWaveClearText()
    {
        if (waveClearText == null)
            return;


        waveClearText.gameObject.SetActive(
            false
        );
    }


    // =====================================================
    // 次Wave UI
    // =====================================================

    private void ShowNextWaveText(
        int waveNumber)
    {
        if (nextWaveText == null)
            return;


        // 次のWaveのWaveDataを取得
        int waveIndex = waveNumber - 1;


        if (
            waves == null ||
            waveIndex < 0 ||
            waveIndex >= waves.Length
        )
        {
            return;
        }


        WaveData nextWave =
            waves[waveIndex];


        if (nextWave == null)
        {
            return;
        }


        // =================================================
        // 敵が出現する方向を取得
        // =================================================

        string dangerDirection =
            GetDangerDirection(nextWave);


        // =================================================
        // UI表示
        // =================================================

        nextWaveText.text =
            $"次は Wave {waveNumber}\n" +
            $"⚠{dangerDirection}";


        nextWaveText.gameObject.SetActive(
            true
        );
    }


    private void HideNextWaveText()
    {
        if (nextWaveText == null)
            return;


        nextWaveText.gameObject.SetActive(
            false
        );
    }


    /// <summary>
    /// Waveで敵が出現する方向を取得する。
    /// </summary>
    private string GetDangerDirection(
        WaveData wave)
    {
        if (wave == null)
            return "";


        // =================================================
        // 全方向
        // =================================================

        if (
            wave.useNorth &&
            wave.useSouth &&
            wave.useEast &&
            wave.useWest
        )
        {
            return "全方向が危険！";
        }


        // =================================================
        // 個別の方向
        // =================================================

        List<string> directions =
            new List<string>();


        if (wave.useNorth)
        {
            directions.Add("上");
        }


        if (wave.useSouth)
        {
            directions.Add("下");
        }


        if (wave.useEast)
        {
            directions.Add("右");
        }


        if (wave.useWest)
        {
            directions.Add("左");
        }


        // =================================================
        // 方向が1つもない場合
        // =================================================

        if (directions.Count == 0)
        {
            return "出現方向なし";
        }


        // =================================================
        // 方向を「・」でつなぐ
        // =================================================

        string directionText =
            string.Join(
                "・",
                directions
            );


        return $"{directionText}方向が危険！";
    }


    // =====================================================
    // Wave開始ボタン
    // =====================================================

    private void ShowWaveStartButton()
    {
        if (waveStartButtonObject != null)
        {
            waveStartButtonObject.SetActive(
                true
            );
        }
        else if (waveStartButton != null)
        {
            waveStartButton.gameObject.SetActive(
                true
            );
        }
    }


    private void HideWaveStartButton()
    {
        if (waveStartButtonObject != null)
        {
            waveStartButtonObject.SetActive(
                false
            );
        }
        else if (waveStartButton != null)
        {
            waveStartButton.gameObject.SetActive(
                false
            );
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


        if (clearUICoroutine != null)
        {
            StopCoroutine(
                clearUICoroutine
            );

            clearUICoroutine = null;
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


        activeEnemies.Clear();


        TotalEnemyCount = 0;

        RemainingEnemyCount = 0;


        HideWaveText();

        HideEnemyCountText();

        HideWaveClearText();

        HideNextWaveText();

        HideWaveStartButton();
    }
}