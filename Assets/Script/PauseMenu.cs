using UnityEngine;

/// <summary>
/// ゲーム中のポーズメニューを管理する。
/// ESCキーで開閉し、メニュー表示中はゲームを停止する。
/// </summary>
public class PauseMenu : MonoBehaviour
{
    [Header("ポーズメニュー")]
    [SerializeField]
    private GameObject pausePanel;

    // ポーズ中か
    private bool isPaused = false;


    private void Start()
    {
        // ゲーム開始時はメニューを閉じる
        isPaused = false;

        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }

        // 念のため時間を通常速度に戻す
        Time.timeScale = 1f;
    }


    private void Update()
    {
        // ESCキー
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }


    /// <summary>
    /// ポーズ状態を切り替える。
    /// </summary>
    public void TogglePause()
    {
        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }


    /// <summary>
    /// ゲームを停止してメニューを表示する。
    /// </summary>
    public void PauseGame()
    {
        isPaused = true;

        if (pausePanel != null)
        {
            pausePanel.SetActive(true);
        }

        // ゲーム時間を停止
        Time.timeScale = 0f;
    }


    /// <summary>
    /// ゲームを再開する。
    /// </summary>
    public void ResumeGame()
    {
        isPaused = false;

        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }

        // ゲーム時間を再開
        Time.timeScale = 1f;
    }


    /// <summary>
    /// タイトル画面へ戻る。
    /// </summary>
    public void ReturnToTitle()
    {
        // シーン移動前に時間を戻す
        Time.timeScale = 1f;

        isPaused = false;

        if (SceneController.Instance != null)
        {
            SceneController.Instance.LoadTitle();
        }
        else
        {
            Debug.LogError(
                "PauseMenu: SceneController.Instanceが見つかりません。"
            );
        }
    }


    private void OnDestroy()
    {
        // シーンを離れるときに必ず時間を戻す
        Time.timeScale = 1f;
    }
}