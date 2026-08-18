using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// タワー建設・タワー管理UIを管理する。
/// </summary>
public class TowerBuildUI : MonoBehaviour
{
    public static bool IsUIOpen { get; private set; }


    // =====================================================
    // パネル
    // =====================================================

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
    // 建設モード閉じるボタン
    // =====================================================

    [Header("建設モード閉じる")]
    public Button buildCloseButton;


    // =====================================================
    // タワー情報
    // =====================================================

    [Header("タワー情報")]
    public Image towerIcon;

    public TMP_Text towerNameText;

    public TMP_Text descriptionText;


    // =====================================================
    // 建設モード：現在のステータス
    // =====================================================

    [Header("建設モード：現在のステータス")]
    public TMP_Text attackDamageText;

    public TMP_Text attackIntervalText;

    public TMP_Text attackRangeText;


    // =====================================================
    // タワー管理モード：現在のステータス
    // =====================================================

    [Header("タワー管理モード：現在のステータス")]
    public TMP_Text towerModeAttackDamageText;

    public TMP_Text towerModeAttackIntervalText;

    public TMP_Text towerModeAttackRangeText;


    // =====================================================
    // アップグレード後のステータス
    // =====================================================

    [Header("アップグレード後のステータス")]
    public TMP_Text nextAttackDamageText;

    public TMP_Text nextAttackIntervalText;

    public TMP_Text nextAttackRangeText;


    // =====================================================
    // 建設費
    // =====================================================

    [Header("建設費")]
    public TMP_Text buildCostText;


    // =====================================================
    // 所持金
    // =====================================================

    [Header("所持金")]

    [Tooltip("建設モードに表示する所持金")]
    public TMP_Text buildModeMoneyText;

    [Tooltip("タワー管理モードに表示する所持金")]
    public TMP_Text towerModeMoneyText;


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
    // タワー管理モード閉じるボタン
    // =====================================================

    [Header("タワー管理モード閉じる")]
    public Button towerCloseButton;


    // =====================================================
    // メッセージ
    // =====================================================

    [Header("メッセージ")]
    public TMP_Text messageText;


    // =====================================================
    // 現在選択しているタワーデータ
    // =====================================================

    private TowerData selectedTowerData;


    // =====================================================
    // 初期化
    // =====================================================

    private void Awake()
    {
        // -------------------------------------------------
        // タワー選択
        // -------------------------------------------------

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


        // -------------------------------------------------
        // 建設
        // -------------------------------------------------

        if (buildButton != null)
        {
            buildButton.onClick.AddListener(
                OnBuildButtonClicked
            );
        }


        // -------------------------------------------------
        // 強化
        // -------------------------------------------------

        if (upgradeButton != null)
        {
            upgradeButton.onClick.AddListener(
                OnUpgradeButtonClicked
            );
        }


        // -------------------------------------------------
        // 売却
        // -------------------------------------------------

        if (sellButton != null)
        {
            sellButton.onClick.AddListener(
                OnSellButtonClicked
            );
        }


        // -------------------------------------------------
        // 建設モード閉じる
        // -------------------------------------------------

        if (buildCloseButton != null)
        {
            buildCloseButton.onClick.AddListener(
                OnBuildCloseButtonClicked
            );
        }


        // -------------------------------------------------
        // タワー管理モード閉じる
        // -------------------------------------------------

        if (towerCloseButton != null)
        {
            towerCloseButton.onClick.AddListener(
                OnTowerCloseButtonClicked
            );
        }
    }


    private void OnDestroy()
    {
        IsUIOpen = false;


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


        if (buildCloseButton != null)
        {
            buildCloseButton.onClick.RemoveListener(
                OnBuildCloseButtonClicked
            );
        }


        if (towerCloseButton != null)
        {
            towerCloseButton.onClick.RemoveListener(
                OnTowerCloseButtonClicked
            );
        }
    }


    // =====================================================
    // 更新
    // =====================================================

    private void Update()
    {
        // UIが開いていない場合は更新しない
        if (!IsUIOpen)
            return;


        // 所持金をリアルタイム更新
        UpdateMoneyDisplay();
    }


    // =====================================================
    // 開始
    // =====================================================

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
        IsUIOpen = true;


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


        // 建設モードのCloseButtonを表示
        if (buildCloseButton != null)
        {
            buildCloseButton.gameObject.SetActive(true);
        }


        // タワー管理モードのCloseButtonを非表示
        if (towerCloseButton != null)
        {
            towerCloseButton.gameObject.SetActive(false);
        }


        selectedTowerData = null;


        // タワー情報を非表示
        HideTowerInformation();


        ClearMessage();


        // 所持金を更新
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


        ShowTowerInformation();


        UpdateTowerDataDisplay(data);


        ClearMessage();
    }


    // =====================================================
    // タワー情報表示
    // =====================================================

    /// <summary>
    /// タワー情報を表示する。
    /// </summary>
    private void ShowTowerInformation()
    {
        // -------------------------------------------------
        // 共通情報
        // -------------------------------------------------

        if (towerIcon != null)
        {
            towerIcon.gameObject.SetActive(true);
        }


        if (towerNameText != null)
        {
            towerNameText.gameObject.SetActive(true);
        }


        if (descriptionText != null)
        {
            descriptionText.gameObject.SetActive(true);
        }


        // -------------------------------------------------
        // 建設モードの現在ステータス
        // -------------------------------------------------

        if (attackDamageText != null)
        {
            attackDamageText.gameObject.SetActive(true);
        }


        if (attackIntervalText != null)
        {
            attackIntervalText.gameObject.SetActive(true);
        }


        if (attackRangeText != null)
        {
            attackRangeText.gameObject.SetActive(true);
        }


        // -------------------------------------------------
        // タワー管理モードの現在ステータス
        // -------------------------------------------------

        if (towerModeAttackDamageText != null)
        {
            towerModeAttackDamageText.gameObject.SetActive(true);
        }


        if (towerModeAttackIntervalText != null)
        {
            towerModeAttackIntervalText.gameObject.SetActive(true);
        }


        if (towerModeAttackRangeText != null)
        {
            towerModeAttackRangeText.gameObject.SetActive(true);
        }


        // -------------------------------------------------
        // 建設費
        // -------------------------------------------------

        if (buildCostText != null)
        {
            buildCostText.gameObject.SetActive(true);
        }
    }


    /// <summary>
    /// タワー情報を非表示にする。
    /// </summary>
    private void HideTowerInformation()
    {
        // -------------------------------------------------
        // 共通情報
        // -------------------------------------------------

        if (towerIcon != null)
        {
            towerIcon.gameObject.SetActive(false);
        }


        if (towerNameText != null)
        {
            towerNameText.gameObject.SetActive(false);
        }


        if (descriptionText != null)
        {
            descriptionText.gameObject.SetActive(false);
        }


        // -------------------------------------------------
        // 建設モードの現在ステータス
        // -------------------------------------------------

        if (attackDamageText != null)
        {
            attackDamageText.gameObject.SetActive(false);
        }


        if (attackIntervalText != null)
        {
            attackIntervalText.gameObject.SetActive(false);
        }


        if (attackRangeText != null)
        {
            attackRangeText.gameObject.SetActive(false);
        }


        // -------------------------------------------------
        // タワー管理モードの現在ステータス
        // -------------------------------------------------

        if (towerModeAttackDamageText != null)
        {
            towerModeAttackDamageText.gameObject.SetActive(false);
        }


        if (towerModeAttackIntervalText != null)
        {
            towerModeAttackIntervalText.gameObject.SetActive(false);
        }


        if (towerModeAttackRangeText != null)
        {
            towerModeAttackRangeText.gameObject.SetActive(false);
        }


        // -------------------------------------------------
        // 建設費
        // -------------------------------------------------

        if (buildCostText != null)
        {
            buildCostText.gameObject.SetActive(false);
        }


        HideNextStatus();
    }


    // =====================================================
    // 建設時のタワー情報
    // =====================================================

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


        // -------------------------------------------------
        // 建設モードの現在ステータス
        // -------------------------------------------------

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


        // -------------------------------------------------
        // 建設費
        // -------------------------------------------------

        if (buildCostText != null)
        {
            buildCostText.text =
                $"建設費：{data.buildCost}";
        }


        HideNextStatus();


        UpdateMoneyDisplay();
    }


    // =====================================================
    // 建設
    // =====================================================

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


        // 建設後に所持金を更新
        UpdateMoneyDisplay();
    }


    // =====================================================
    // タワー管理モード
    // =====================================================

    /// <summary>
    /// 建設済みタワーの管理UIを表示する。
    /// </summary>
    public void ShowTowerMode(
        Tower tower)
    {
        if (tower == null)
            return;


        IsUIOpen = true;


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


        // 建設モードのCloseButtonを非表示
        if (buildCloseButton != null)
        {
            buildCloseButton.gameObject.SetActive(false);
        }


        // タワー管理モードのCloseButtonを表示
        if (towerCloseButton != null)
        {
            towerCloseButton.gameObject.SetActive(true);
        }


        ShowTowerInformation();


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
        if (tower == null)
            return;


        TowerData data =
            tower.towerData;


        if (data == null)
            return;


        // -------------------------------------------------
        // アイコン
        // -------------------------------------------------

        if (towerIcon != null)
        {
            towerIcon.gameObject.SetActive(true);

            towerIcon.sprite =
                data.icon;
        }


        // -------------------------------------------------
        // 名前
        // -------------------------------------------------

        if (towerNameText != null)
        {
            towerNameText.gameObject.SetActive(true);

            towerNameText.text =
                data.towerName;
        }


        // -------------------------------------------------
        // 説明
        // -------------------------------------------------

        if (descriptionText != null)
        {
            descriptionText.gameObject.SetActive(true);

            descriptionText.text =
                data.description;
        }


        // -------------------------------------------------
        // 現在の攻撃力
        // -------------------------------------------------

        if (towerModeAttackDamageText != null)
        {
            towerModeAttackDamageText.gameObject.SetActive(true);

            towerModeAttackDamageText.text =
                $"攻撃力：{tower.CurrentAttackDamage}";
        }


        // -------------------------------------------------
        // 現在の攻撃間隔
        // -------------------------------------------------

        if (towerModeAttackIntervalText != null)
        {
            towerModeAttackIntervalText.gameObject.SetActive(true);

            towerModeAttackIntervalText.text =
                $"攻撃間隔：{tower.CurrentAttackInterval:F1}秒";
        }


        // -------------------------------------------------
        // 現在の射程
        // -------------------------------------------------

        if (towerModeAttackRangeText != null)
        {
            towerModeAttackRangeText.gameObject.SetActive(true);

            towerModeAttackRangeText.text =
                $"射程：{tower.CurrentAttackRange:F1}";
        }


        // -------------------------------------------------
        // レベル
        // -------------------------------------------------

        if (towerLevelText != null)
        {
            towerLevelText.gameObject.SetActive(true);

            towerLevelText.text =
                $"Lv.{tower.Level} / {data.maxLevel}";
        }


        // -------------------------------------------------
        // 最大レベル
        // -------------------------------------------------

        bool isMaxLevel =
            tower.Level >= data.maxLevel;


        if (isMaxLevel)
        {
            HideNextStatus();


            if (upgradeCostText != null)
            {
                upgradeCostText.gameObject.SetActive(true);

                upgradeCostText.text =
                    "最大レベル";
            }


            if (upgradeButton != null)
            {
                upgradeButton.interactable =
                    false;
            }
        }
        else
        {
            ShowNextStatus(tower);


            if (upgradeCostText != null)
            {
                upgradeCostText.gameObject.SetActive(true);

                upgradeCostText.text =
                    $"強化費：{tower.GetUpgradeCost()}";
            }


            if (upgradeButton != null)
            {
                upgradeButton.interactable =
                    true;
            }
        }


        // -------------------------------------------------
        // 売却価格
        // -------------------------------------------------

        if (sellPriceText != null)
        {
            sellPriceText.gameObject.SetActive(true);

            sellPriceText.text =
                $"売却額：{tower.GetSellPrice()}";
        }


        // -------------------------------------------------
        // CloseButton
        // -------------------------------------------------

        if (towerCloseButton != null)
        {
            towerCloseButton.gameObject.SetActive(true);

            towerCloseButton.interactable =
                true;
        }


        // 所持金を更新
        UpdateMoneyDisplay();
    }


    // =====================================================
    // 次のレベルのステータス
    // =====================================================

    private void ShowNextStatus(
        Tower tower)
    {
        if (nextAttackDamageText != null)
        {
            nextAttackDamageText.gameObject.SetActive(true);

            nextAttackDamageText.text =
                $"攻撃力\n" +
                $"{tower.CurrentAttackDamage} → " +
                $"{tower.GetNextAttackDamage()}";
        }


        if (nextAttackIntervalText != null)
        {
            nextAttackIntervalText.gameObject.SetActive(true);

            nextAttackIntervalText.text =
                $"攻撃間隔\n" +
                $"{tower.CurrentAttackInterval:F1}秒 → " +
                $"{tower.GetNextAttackInterval():F1}秒";
        }


        if (nextAttackRangeText != null)
        {
            nextAttackRangeText.gameObject.SetActive(true);

            nextAttackRangeText.text =
                $"射程\n" +
                $"{tower.CurrentAttackRange:F1} → " +
                $"{tower.GetNextAttackRange():F1}";
        }
    }


    private void HideNextStatus()
    {
        if (nextAttackDamageText != null)
        {
            nextAttackDamageText.gameObject.SetActive(false);
        }


        if (nextAttackIntervalText != null)
        {
            nextAttackIntervalText.gameObject.SetActive(false);
        }


        if (nextAttackRangeText != null)
        {
            nextAttackRangeText.gameObject.SetActive(false);
        }
    }


    // =====================================================
    // 強化
    // =====================================================

    private void OnUpgradeButtonClicked()
    {
        if (TowerPlacementManager.Instance == null)
            return;


        TowerPlacementManager.Instance
            .UpgradeSelectedTower();


        // 強化後に所持金を更新
        UpdateMoneyDisplay();
    }


    // =====================================================
    // 売却
    // =====================================================

    private void OnSellButtonClicked()
    {
        if (TowerPlacementManager.Instance == null)
            return;


        TowerPlacementManager.Instance
            .SellSelectedTower();


        // 売却後に所持金を更新
        UpdateMoneyDisplay();
    }


    // =====================================================
    // 建設モードClose
    // =====================================================

    private void OnBuildCloseButtonClicked()
    {
        Debug.Log(
            "TowerBuildUI: 建設モードのCloseButtonが押されました。"
        );


        Hide();
    }


    // =====================================================
    // タワー管理モードClose
    // =====================================================

    private void OnTowerCloseButtonClicked()
    {
        Debug.Log(
            "TowerBuildUI: タワー管理モードのCloseButtonが押されました。"
        );


        Hide();
    }


    // =====================================================
    // 共通
    // =====================================================

    /// <summary>
    /// UIを非表示にする。
    /// </summary>
    public void Hide()
    {
        IsUIOpen = false;


        if (panel != null)
        {
            panel.SetActive(false);
        }


        selectedTowerData = null;
    }


    // =====================================================
    // 所持金
    // =====================================================

    /// <summary>
    /// 建設モード・タワー管理モードの
    /// 所持金表示を更新する。
    /// </summary>
    private void UpdateMoneyDisplay()
    {
        // ResourceManagerが存在しない場合
        if (ResourceManager.Instance == null)
        {
            if (buildModeMoneyText != null)
            {
                buildModeMoneyText.text =
                    "所持金：---";
            }


            if (towerModeMoneyText != null)
            {
                towerModeMoneyText.text =
                    "所持金：---";
            }


            return;
        }


        // 現在の所持金
        string moneyText =
            $"所持金：{ResourceManager.Instance.CurrentMoney}";


        // 建設モード
        if (buildModeMoneyText != null)
        {
            buildModeMoneyText.text =
                moneyText;
        }


        // タワー管理モード
        if (towerModeMoneyText != null)
        {
            towerModeMoneyText.text =
                moneyText;
        }
    }


    // =====================================================
    // メッセージ
    // =====================================================

    public void ShowNotEnoughMoney()
    {
        ShowMessage(
            "お金が足りません！"
        );
    }


    public void ShowMaxLevel()
    {
        ShowMessage(
            "このタワーは最大レベルです。"
        );
    }


    private void ShowMessage(
        string message)
    {
        if (messageText != null)
        {
            messageText.text =
                message;
        }
    }


    private void ClearMessage()
    {
        if (messageText != null)
        {
            messageText.text =
                "";
        }
    }
}