using TMPro;
using UnityEngine;

/// <summary>
/// 所持金をUIに表示する。
/// </summary>
public class MoneyUI : MonoBehaviour
{
    [Header("所持金表示")]
    [Tooltip("所持金を表示するTextMeshPro")]
    [SerializeField]
    private TMP_Text moneyText;


    [Header("表示設定")]
    [Tooltip("所持金の前に表示する文字")]
    [SerializeField]
    private string prefix = "所持金：";


    private void Start()
    {
        UpdateMoneyUI();
    }


    private void Update()
    {
        UpdateMoneyUI();
    }


    /// <summary>
    /// 所持金UIを更新する。
    /// </summary>
    public void UpdateMoneyUI()
    {
        if (moneyText == null)
            return;


        if (ResourceManager.Instance == null)
        {
            moneyText.text =
                $"{prefix}---";

            return;
        }


        moneyText.text =
            $"{prefix}{ResourceManager.Instance.CurrentMoney}";
    }
}