using UnityEngine;

/// <summary>
/// タワーの建設・強化・売却・UI選択を管理する。
/// </summary>
public class TowerPlacementManager : MonoBehaviour
{
    public static TowerPlacementManager Instance { get; private set; }

    [Header("UI")]
    public TowerBuildUI buildUI;

    // 現在選択中の建設地点
    private TowerPlacementPoint selectedPoint;

    // 現在選択中のタワー
    private Tower selectedTower;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }


    private void Start()
    {
        if (buildUI != null)
        {
            buildUI.Hide();
        }
    }


    /// <summary>
    /// 建設地点をクリックしたとき。
    /// </summary>
    public void OpenBuildUI(TowerPlacementPoint point)
    {
        if (point == null)
            return;

        if (point.IsOccupied)
            return;

        selectedPoint = point;
        selectedTower = null;

        if (buildUI != null)
        {
            buildUI.ShowBuildMode();
        }
    }


    /// <summary>
    /// 建設済みタワーをクリックしたとき。
    /// </summary>
    public void OpenTowerUI(Tower tower)
    {
        if (tower == null)
            return;

        selectedTower = tower;
        selectedPoint = null;

        if (buildUI != null)
        {
            buildUI.ShowTowerMode(tower);
        }
    }


    /// <summary>
    /// タワーを建設する。
    /// </summary>
    public void BuildTower(TowerData towerData)
    {
        if (selectedPoint == null)
            return;

        if (towerData == null)
            return;

        if (selectedPoint.IsOccupied)
        {
            CloseUI();
            return;
        }


        // ResourceManager確認
        if (ResourceManager.Instance == null)
        {
            Debug.LogError(
                "TowerPlacementManager: ResourceManagerが存在しません。"
            );

            return;
        }


        // お金が足りるか確認
        if (!ResourceManager.Instance.CanAfford(
            towerData.buildCost))
        {
            buildUI?.ShowNotEnoughMoney();
            return;
        }


        // Prefab確認
        if (towerData.towerPrefab == null)
        {
            Debug.LogError(
                $"TowerData「{towerData.towerName}」にTower Prefabが設定されていません。"
            );

            return;
        }


        // お金を消費
        if (!ResourceManager.Instance.SpendMoney(
            towerData.buildCost))
        {
            return;
        }


        // タワー生成
        GameObject towerObject = Instantiate(
            towerData.towerPrefab,
            selectedPoint.transform.position,
            Quaternion.identity
        );


        // Tower取得
        Tower tower =
            towerObject.GetComponent<Tower>();


        if (tower == null)
        {
            Debug.LogError(
                "生成されたTower PrefabにTower.csがありません。"
            );

            // 建設失敗なので返金
            ResourceManager.Instance.AddMoney(
                towerData.buildCost
            );

            Destroy(towerObject);

            return;
        }


        // TowerDataを設定
        tower.towerData = towerData;


        // 建設地点を使用済みにする
        selectedPoint.SetOccupied(tower);


        // UIを閉じる
        CloseUI();
    }


    /// <summary>
    /// 選択中のタワーを強化する。
    /// </summary>
    public void UpgradeSelectedTower()
    {
        if (selectedTower == null)
            return;

        if (selectedTower.towerData == null)
            return;

        if (ResourceManager.Instance == null)
        {
            Debug.LogError(
                "TowerPlacementManager: ResourceManagerが存在しません。"
            );

            return;
        }


        TowerData data =
            selectedTower.towerData;


        // 最大レベル確認
        if (selectedTower.Level >= data.maxLevel)
        {
            buildUI?.ShowMaxLevel();
            return;
        }


        // 強化価格
        int upgradeCost =
            selectedTower.GetUpgradeCost();


        // お金が足りるか確認
        if (!ResourceManager.Instance.CanAfford(
            upgradeCost))
        {
            buildUI?.ShowNotEnoughMoney();
            return;
        }


        // お金を消費
        if (!ResourceManager.Instance.SpendMoney(
            upgradeCost))
        {
            return;
        }


        // タワー強化
        bool success =
            selectedTower.Upgrade();


        if (!success)
        {
            // 強化失敗時は返金
            ResourceManager.Instance.AddMoney(
                upgradeCost
            );

            return;
        }


        // 強化後の情報をUIに反映
        if (buildUI != null)
        {
            buildUI.ShowTowerMode(
                selectedTower
            );
        }
    }


    /// <summary>
    /// 選択中のタワーを売却する。
    /// </summary>
    public void SellSelectedTower()
    {
        if (selectedTower == null)
            return;


        if (ResourceManager.Instance == null)
        {
            Debug.LogError(
                "TowerPlacementManager: ResourceManagerが存在しません。"
            );

            return;
        }


        // 売却価格
        int sellPrice =
            selectedTower.GetSellPrice();


        // 建設地点を探す
        TowerPlacementPoint point =
            FindPlacementPoint(
                selectedTower.transform.position
            );


        // タワーを破壊
        Destroy(selectedTower.gameObject);


        // 建設地点を再利用可能にする
        if (point != null)
        {
            point.ResetPoint();
        }


        // 売却額を加算
        ResourceManager.Instance.AddMoney(
            sellPrice
        );


        // UIを閉じる
        CloseUI();
    }


    /// <summary>
    /// タワーの位置から最も近い建設地点を取得する。
    /// </summary>
    private TowerPlacementPoint FindPlacementPoint(
        Vector3 position)
    {
        TowerPlacementPoint[] points =
            FindObjectsByType<TowerPlacementPoint>(
                FindObjectsSortMode.None
            );


        TowerPlacementPoint closestPoint = null;

        float closestDistance =
            float.MaxValue;


        foreach (
            TowerPlacementPoint point
            in points)
        {
            float distance =
                Vector3.Distance(
                    point.transform.position,
                    position
                );


            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestPoint = point;
            }
        }


        return closestPoint;
    }


    /// <summary>
    /// UIを閉じる。
    /// </summary>
    public void CloseUI()
    {
        selectedPoint = null;
        selectedTower = null;

        if (buildUI != null)
        {
            buildUI.Hide();
        }
    }
}