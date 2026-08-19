using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ゲームオーバーUIを管理する。
///
/// ゲームオーバー時にパネルを表示し、
/// タイトルへ戻るボタンを管理する。
/// </summary>
public class GameOverUI : MonoBehaviour
{
    public static GameOverUI Instance { get; private set; }


    // =====================================================
    // UI
    // =====================================================

    [Header("ゲームオーバーUI")]

    [Tooltip("ゲームオーバー時に表示するパネル")]
    [SerializeField]
    private GameObject gameOverPanel;


    [Tooltip("タイトルへ戻るボタン")]
    [SerializeField]
    private Button titleButton;


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


        // 最初は非表示
        Hide();


        // タイトルボタン
        if (titleButton != null)
        {
            titleButton.onClick.AddListener(
                OnTitleButtonClicked
            );
        }
    }


    private void OnDestroy()
    {
        if (titleButton != null)
        {
            titleButton.onClick.RemoveListener(
                OnTitleButtonClicked
            );
        }


        if (Instance == this)
        {
            Instance = null;
        }
    }


    // =====================================================
    // ゲームオーバー表示
    // =====================================================

    /// <summary>
    /// ゲームオーバーUIを表示する。
    /// </summary>
    public void Show()
    {
        if (gameOverPanel == null)
        {
            Debug.LogError(
                "GameOverUI: GameOverPanelが設定されていません。",
                this
            );

            return;
        }


        gameOverPanel.SetActive(true);
    }


    /// <summary>
    /// ゲームオーバーUIを非表示にする。
    /// </summary>
    public void Hide()
    {
        if (gameOverPanel == null)
            return;


        gameOverPanel.SetActive(false);
    }


    // =====================================================
    // タイトルボタン
    // =====================================================

    private void OnTitleButtonClicked()
    {
        // ゲームオーバー状態で停止していた時間を戻す
        Time.timeScale = 1f;


        if (SceneController.Instance != null)
        {
            SceneController.Instance.LoadTitle();
        }
        else
        {
            Debug.LogError(
                "GameOverUI: SceneController.Instanceが見つかりません。",
                this
            );
        }
    }
}