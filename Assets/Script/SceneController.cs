using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// ゲーム全体のシーン遷移を管理する。
/// </summary>
public class SceneController : MonoBehaviour
{
    public static SceneController Instance { get; private set; }

    private void Awake()
    {
        // すでに存在している場合
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);
    }


    private void Start()
    {
        // 現在のシーンを確認
        string currentScene =
            SceneManager.GetActiveScene().name;

        // TitleScene以外から起動された場合、
        // TitleSceneへ移動する
        if (currentScene != "TitleScene")
        {
            SceneManager.LoadScene("TitleScene");
        }
    }


    /// <summary>
    /// GameSceneへ移動する。
    /// </summary>
    public void LoadGame()
    {
        SceneManager.LoadScene("GameScene");
    }


    /// <summary>
    /// ClearSceneへ移動する。
    /// </summary>
    public void LoadClear()
    {
        SceneManager.LoadScene("ClearScene");
    }


    /// <summary>
    /// TitleSceneへ移動する。
    /// </summary>
    public void LoadTitle()
    {
        SceneManager.LoadScene("TitleScene");
    }
}