using UnityEngine;

/// <summary>
/// タワー建設全体を管理する。
/// </summary>
public class TowerPlacementManager : MonoBehaviour
{
    public static TowerPlacementManager Instance { get; private set; }


    [Header("UI")]
    public TowerBuildUI buildUI;


    // 現在選択されている建設地点
    private TowerPlacementPoint selectedPoint;


    private void Awake()
    {
        if (Instance != null &&
            Instance != this)
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
    /// 建設地点がクリックされたときに呼ばれる。
    /// </summary>
    public void OpenBuildUI(
        TowerPlacementPoint point)
    {
        if (point == null)
            return;

        if (point.IsOccupied)
            return;

        selectedPoint = point;

        if (buildUI == null)
        {
            Debug.LogError(
                "TowerBuildUIが設定されていません。",
                this
            );

            return;
        }

        buildUI.Show();
    }


    /// <summary>
    /// 選択中の建設地点にタワーを建設する。
    /// </summary>
    public void BuildTower(TowerData towerData)
    {
        if (selectedPoint == null)
        {
            Debug.LogWarning(
                "建設地点が選択されていません。"
            );

            return;
        }

        if (towerData == null)
        {
            Debug.LogError(
                "TowerDataが設定されていません。"
            );

            return;
        }


        // すでに建設されていないか確認
        if (selectedPoint.IsOccupied)
        {
            CloseBuildUI();
            return;
        }


        // お金を確認
        if (ResourceManager.Instance == null)
        {
            Debug.LogError(
                "ResourceManagerが存在しません。"
            );

            return;
        }


        if (!ResourceManager.Instance.CanAfford(
            towerData.buildCost))
        {
            buildUI.ShowNotEnoughMoney();
            return;
        }


        // お金を消費
        bool spent =
            ResourceManager.Instance.SpendMoney(
                towerData.buildCost
            );

        if (!spent)
        {
            buildUI.ShowNotEnoughMoney();
            return;
        }


        // タワー生成
        if (towerData.towerPrefab == null)
        {
            Debug.LogError(
                $"{towerData.towerName}: Tower Prefabが設定されていません。"
            );

            // 建設できなかったのでお金を返す
            ResourceManager.Instance.AddMoney(
                towerData.buildCost
            );

            return;
        }


        Instantiate(
            towerData.towerPrefab,
            selectedPoint.transform.position,
            Quaternion.identity
        );


        // 建設地点を使用済みにする
        selectedPoint.SetOccupied();


        // UIを閉じる
        CloseBuildUI();
    }


    /// <summary>
    /// 建設UIを閉じる。
    /// </summary>
    public void CloseBuildUI()
    {
        selectedPoint = null;

        if (buildUI != null)
        {
            buildUI.Hide();
        }
    }
}