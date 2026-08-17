using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// タワー建設UIを管理する。
/// </summary>
public class TowerBuildUI : MonoBehaviour
{
    [Header("パネル")]
    public GameObject panel;


    [Header("タワー情報")]
    public Image towerIcon;
    public TMP_Text towerNameText;
    public TMP_Text descriptionText;

    public TMP_Text attackDamageText;
    public TMP_Text attackIntervalText;
    public TMP_Text attackRangeText;
    public TMP_Text buildCostText;


    [Header("所持金")]
    public TMP_Text moneyText;


    [Header("ボタン")]
    public Button buildButton;
    public Button closeButton;


    [Header("建設するタワー")]
    public TowerData selectedTowerData;


    [Header("メッセージ")]
    public TMP_Text messageText;


    private void Awake()
    {
        if (buildButton != null)
        {
            buildButton.onClick.AddListener(
                OnBuildButtonClicked
            );
        }

        if (closeButton != null)
        {
            closeButton.onClick.AddListener(
                OnCloseButtonClicked
            );
        }
    }


    private void OnDestroy()
    {
        if (buildButton != null)
        {
            buildButton.onClick.RemoveListener(
                OnBuildButtonClicked
            );
        }

        if (closeButton != null)
        {
            closeButton.onClick.RemoveListener(
                OnCloseButtonClicked
            );
        }
    }


    private void Start()
    {
        Hide();
    }


    /// <summary>
    /// UIを表示する。
    /// </summary>
    public void Show()
    {
        if (panel != null)
        {
            panel.SetActive(true);
        }

        ClearMessage();

        UpdateDisplay();
    }


    /// <summary>
    /// UIを非表示にする。
    /// </summary>
    public void Hide()
    {
        if (panel != null)
        {
            panel.SetActive(false);
        }
    }


    /// <summary>
    /// タワー情報を表示する。
    /// </summary>
    private void UpdateDisplay()
    {
        if (selectedTowerData == null)
        {
            Debug.LogWarning(
                "TowerDataが設定されていません。"
            );

            return;
        }


        if (towerIcon != null)
        {
            towerIcon.sprite =
                selectedTowerData.icon;
        }


        if (towerNameText != null)
        {
            towerNameText.text =
                selectedTowerData.towerName;
        }


        if (descriptionText != null)
        {
            descriptionText.text =
                selectedTowerData.description;
        }


        if (attackDamageText != null)
        {
            attackDamageText.text =
                $"攻撃力：{selectedTowerData.attackDamage}";
        }


        if (attackIntervalText != null)
        {
            attackIntervalText.text =
                $"攻撃間隔：{selectedTowerData.attackInterval:F1}秒";
        }


        if (attackRangeText != null)
        {
            attackRangeText.text =
                $"射程：{selectedTowerData.attackRange:F1}";
        }


        if (buildCostText != null)
        {
            buildCostText.text =
                $"建設費：{selectedTowerData.buildCost}";
        }


        UpdateMoneyDisplay();
    }


    /// <summary>
    /// 所持金を表示する。
    /// </summary>
    private void UpdateMoneyDisplay()
    {
        if (moneyText == null)
            return;

        if (ResourceManager.Instance == null)
        {
            moneyText.text = "所持金：---";
            return;
        }

        moneyText.text =
            $"所持金：{ResourceManager.Instance.CurrentMoney}";
    }


    /// <summary>
    /// 建設ボタンを押したとき。
    /// </summary>
    private void OnBuildButtonClicked()
    {
        if (selectedTowerData == null)
            return;

        if (TowerPlacementManager.Instance == null)
        {
            Debug.LogError(
                "TowerPlacementManagerが存在しません。"
            );

            return;
        }

        TowerPlacementManager.Instance.BuildTower(
            selectedTowerData
        );
    }


    /// <summary>
    /// 閉じるボタンを押したとき。
    /// </summary>
    private void OnCloseButtonClicked()
    {
        if (TowerPlacementManager.Instance != null)
        {
            TowerPlacementManager.Instance.CloseBuildUI();
        }
        else
        {
            Hide();
        }
    }


    /// <summary>
    /// お金が足りないときのメッセージ。
    /// </summary>
    public void ShowNotEnoughMoney()
    {
        if (messageText != null)
        {
            messageText.text =
                "お金が足りません！";
        }
    }


    /// <summary>
    /// メッセージを消す。
    /// </summary>
    private void ClearMessage()
    {
        if (messageText != null)
        {
            messageText.text = "";
        }
    }
}