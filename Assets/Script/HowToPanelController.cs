using UnityEngine;

/// <summary>
/// タイトル画面の遊び方UIを管理する。
/// ページ切り替え、表示・非表示を行う。
/// </summary>
public class HowToPanelController : MonoBehaviour
{
    [Header("遊び方パネル")]
    [SerializeField]
    private GameObject howToPanel;

    [Header("遊び方ページ")]
    [SerializeField]
    private GameObject[] pages;

    private int currentPage = 0;


    private void Start()
    {
        // ゲーム開始時は遊び方を閉じる
        CloseHowTo();
    }


    /// <summary>
    /// 遊び方を開く。
    /// 必ず1ページ目から開始する。
    /// </summary>
    public void OpenHowTo()
    {
        currentPage = 0;

        if (howToPanel != null)
        {
            howToPanel.SetActive(true);
        }

        UpdatePage();
    }


    /// <summary>
    /// 遊び方を閉じる。
    /// </summary>
    public void CloseHowTo()
    {
        currentPage = 0;

        if (howToPanel != null)
        {
            howToPanel.SetActive(false);
        }
    }


    /// <summary>
    /// 次のページへ進む。
    /// </summary>
    public void NextPage()
    {
        if (pages == null || pages.Length == 0)
            return;

        if (currentPage >= pages.Length - 1)
            return;

        currentPage++;

        UpdatePage();
    }


    /// <summary>
    /// 前のページへ戻る。
    /// </summary>
    public void PreviousPage()
    {
        if (pages == null || pages.Length == 0)
            return;

        if (currentPage <= 0)
            return;

        currentPage--;

        UpdatePage();
    }


    /// <summary>
    /// 現在のページだけを表示する。
    /// </summary>
    private void UpdatePage()
    {
        if (pages == null)
            return;

        for (int i = 0; i < pages.Length; i++)
        {
            if (pages[i] == null)
                continue;

            pages[i].SetActive(i == currentPage);
        }
    }
}