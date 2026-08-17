using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// タワー建設・タワー管理UIを管理する。
/// </summary>
public class TowerBuildUI : MonoBehaviour
{
    [Header("パネル")]
    public GameObject panel;


    // =====================================================
    // 建設モード
    // =====================================================

    [Header("建設モード")]
    public GameObject buildModePanel;


    [Header("タワー選択ボタン")]
    public Button towerButton1;
    public Button towerButton2;
    public Button towerButton3;


    [Header("タワーデータ")]
    public TowerData towerData1;
    public TowerData towerData2;
    public TowerData towerData3;


    // =====================================================
    // タワー情報
    // =====================================================

    [Header("タワー情報")]
    public Image towerIcon;

    public TMP_Text towerNameText;

    public TMP_Text descriptionText;

    public TMP_Text attackDamageText;

    public TMP_Text attackIntervalText;

    public TMP_Text attackRangeText;

    public TMP_Text buildCostText;


    // =====================================================
    // 所持金
    // =====================================================

    [Header("所持金")]
    public TMP_Text moneyText;


    // =====================================================
    // 建設
    // =====================================================

    [Header("建設")]
    public Button buildButton;


    // =====================================================
    // タワー管理モード
    // =====================================================

    [Header("タワー管理モード")]
    public GameObject towerModePanel;


    public TMP_Text towerLevelText;

    public TMP_Text upgradeCostText;

    public TMP_Text sellPriceText;


    public Button upgradeButton;

    public Button sellButton;


    // =====================================================
    // 共通
    // =====================================================

    [Header("閉じる")]
    public Button closeButton;


    [Header("メッセージ")]
    public TMP_Text messageText;


    // 現在選択しているタワーデータ
    private TowerData selectedTowerData;


    private void Awake()
    {
        // タワー選択
        if (towerButton1 != null)
        {
            towerButton1.onClick.AddListener(
                () => SelectTower(towerData1)
            );
        }


        if (towerButton2 != null)
        {
            towerButton2.onClick.AddListener(
                () => SelectTower(towerData2)
            );
        }


        if (towerButton3 != null)
        {
            towerButton3.onClick.AddListener(
                () => SelectTower(towerData3)
            );
        }


        // 建設
        if (buildButton != null)
        {
            buildButton.onClick.AddListener(
                OnBuildButtonClicked
            );
        }


        // 強化
        if (upgradeButton != null)
        {
            upgradeButton.onClick.AddListener(
                OnUpgradeButtonClicked
            );
        }


        // 破壊
        if (sellButton != null)
        {
            sellButton.onClick.AddListener(
                OnSellButtonClicked
            );
        }


        // 閉じる
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(
                OnCloseButtonClicked
            );
        }
    }


    private void OnDestroy()
    {
        if (towerButton1 != null)
        {
            towerButton1.onClick.RemoveAllListeners();
        }

        if (towerButton2 != null)
        {
            towerButton2.onClick.RemoveAllListeners();
        }

        if (towerButton3 != null)
        {
            towerButton3.onClick.RemoveAllListeners();
        }

        if (buildButton != null)
        {
            buildButton.onClick.RemoveListener(
                OnBuildButtonClicked
            );
        }

        if (upgradeButton != null)
        {
            upgradeButton.onClick.RemoveListener(
                OnUpgradeButtonClicked
            );
        }

        if (sellButton != null)
        {
            sellButton.onClick.RemoveListener(
                OnSellButtonClicked
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


    // =====================================================
    // 建設モード
    // =====================================================

    /// <summary>
    /// 建設モードを表示する。
    /// </summary>
    public void ShowBuildMode()
    {
        if (panel != null)
        {
            panel.SetActive(true);
        }


        if (buildModePanel != null)
        {
            buildModePanel.SetActive(true);
        }


        if (towerModePanel != null)
        {
            towerModePanel.SetActive(false);
        }


        selectedTowerData = null;

        ClearMessage();

        UpdateMoneyDisplay();
    }


    /// <summary>
    /// タワーを選択する。
    /// </summary>
    private void SelectTower(
        TowerData data)
    {
        if (data == null)
            return;


        selectedTowerData = data;

        UpdateTowerDataDisplay(data);

        ClearMessage();
    }


    /// <summary>
    /// タワー性能をUIに表示する。
    /// </summary>
    private void UpdateTowerDataDisplay(
        TowerData data)
    {
        if (towerIcon != null)
        {
            towerIcon.sprite =
                data.icon;
        }


        if (towerNameText != null)
        {
            towerNameText.text =
                data.towerName;
        }


        if (descriptionText != null)
        {
            descriptionText.text =
                data.description;
        }


        if (attackDamageText != null)
        {
            attackDamageText.text =
                $"攻撃力：{data.attackDamage}";
        }


        if (attackIntervalText != null)
        {
            attackIntervalText.text =
                $"攻撃間隔：{data.attackInterval:F1}秒";
        }


        if (attackRangeText != null)
        {
            attackRangeText.text =
                $"射程：{data.attackRange:F1}";
        }


        if (buildCostText != null)
        {
            buildCostText.text =
                $"建設費：{data.buildCost}";
        }


        UpdateMoneyDisplay();
    }


    /// <summary>
    /// 建設ボタン。
    /// </summary>
    private void OnBuildButtonClicked()
    {
        if (selectedTowerData == null)
        {
            ShowMessage(
                "タワーを選択してください。"
            );

            return;
        }


        if (TowerPlacementManager.Instance == null)
            return;


        TowerPlacementManager.Instance.BuildTower(
            selectedTowerData
        );
    }


    // =====================================================
    // タワー管理モード
    // =====================================================

    /// <summary>
    /// 建設済みタワーをクリックしたときのUI。
    /// </summary>
    public void ShowTowerMode(
        Tower tower)
    {
        if (tower == null)
            return;


        if (panel != null)
        {
            panel.SetActive(true);
        }


        if (buildModePanel != null)
        {
            buildModePanel.SetActive(false);
        }


        if (towerModePanel != null)
        {
            towerModePanel.SetActive(true);
        }


        ClearMessage();

        UpdateTowerManagementDisplay(
            tower
        );
    }


    /// <summary>
    /// タワー管理情報を更新する。
    /// </summary>
    private void UpdateTowerManagementDisplay(
        Tower tower)
    {
        TowerData data =
            tower.towerData;


        if (data == null)
            return;


        if (towerIcon != null)
        {
            towerIcon.sprite =
                data.icon;
        }


        if (towerNameText != null)
        {
            towerNameText.text =
                data.towerName;
        }


        if (descriptionText != null)
        {
            descriptionText.text =
                data.description;
        }


        if (attackDamageText != null)
        {
            attackDamageText.text =
                $"攻撃力：{tower.CurrentAttackDamage}";
        }


        if (attackIntervalText != null)
        {
            attackIntervalText.text =
                $"攻撃間隔：{tower.CurrentAttackInterval:F1}秒";
        }


        if (attackRangeText != null)
        {
            attackRangeText.text =
                $"射程：{tower.CurrentAttackRange:F1}";
        }


        if (towerLevelText != null)
        {
            towerLevelText.text =
                $"Lv.{tower.Level} / {data.maxLevel}";
        }


        if (upgradeCostText != null)
        {
            if (tower.Level >= data.maxLevel)
            {
                upgradeCostText.text =
                    "最大レベル";
            }
            else
            {
                upgradeCostText.text =
                    $"強化費：{tower.GetUpgradeCost()}";
            }
        }


        if (sellPriceText != null)
        {
            sellPriceText.text =
                $"売却額：{tower.GetSellPrice()}";
        }


        // 最大レベルなら強化ボタンを押せなくする
        if (upgradeButton != null)
        {
            upgradeButton.interactable =
                tower.Level < data.maxLevel;
        }


        UpdateMoneyDisplay();
    }


    /// <summary>
    /// 強化ボタン。
    /// </summary>
    private void OnUpgradeButtonClicked()
    {
        if (TowerPlacementManager.Instance == null)
            return;


        TowerPlacementManager.Instance
            .UpgradeSelectedTower();
    }


    /// <summary>
    /// 売却ボタン。
    /// </summary>
    private void OnSellButtonClicked()
    {
        if (TowerPlacementManager.Instance == null)
            return;


        TowerPlacementManager.Instance
            .SellSelectedTower();
    }


    // =====================================================
    // 共通
    // =====================================================

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
    /// 所持金表示を更新する。
    /// </summary>
    private void UpdateMoneyDisplay()
    {
        if (moneyText == null)
            return;


        if (ResourceManager.Instance == null)
        {
            moneyText.text =
                "所持金：---";

            return;
        }


        moneyText.text =
            $"所持金：{ResourceManager.Instance.CurrentMoney}";
    }


    /// <summary>
    /// お金が足りない。
    /// </summary>
    public void ShowNotEnoughMoney()
    {
        ShowMessage(
            "お金が足りません！"
        );
    }


    /// <summary>
    /// 最大レベル。
    /// </summary>
    public void ShowMaxLevel()
    {
        ShowMessage(
            "このタワーは最大レベルです。"
        );
    }


    /// <summary>
    /// メッセージを表示する。
    /// </summary>
    private void ShowMessage(
        string message)
    {
        if (messageText != null)
        {
            messageText.text =
                message;
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


    /// <summary>
    /// 閉じるボタン。
    /// </summary>
    private void OnCloseButtonClicked()
    {
        if (TowerPlacementManager.Instance != null)
        {
            TowerPlacementManager.Instance.CloseUI();
        }
        else
        {
            Hide();
        }
    }
}