using UnityEngine;

/// <summary>
/// タワーの建設・強化・売却・クリック判定・UI選択を管理する。
///
/// クリック判定はOnMouseDownを使わず、
/// 画面上のクリック位置からPhysics2D.Raycastを行う。
/// </summary>
public class TowerPlacementManager : MonoBehaviour
{
    public static TowerPlacementManager Instance { get; private set; }


    // =====================================================
    // UI
    // =====================================================

    [Header("UI")]
    public TowerBuildUI buildUI;


    // =====================================================
    // クリック設定
    // =====================================================

    [Header("クリック判定")]

    [Tooltip("タワークリック判定用Layer")]
    public LayerMask towerClickLayer;

    [Tooltip("建設地点クリック判定用Layer")]
    public LayerMask placementPointLayer;


    // =====================================================
    // 選択状態
    // =====================================================

    // 選択中の建設地点
    private TowerPlacementPoint selectedPoint;

    // 選択中のタワー
    private Tower selectedTower;


    // =====================================================
    // UI状態
    // =====================================================

    public bool IsUIOpen =>
        selectedPoint != null ||
        selectedTower != null;


    // =====================================================
    // Unity
    // =====================================================

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


    private void Update()
    {
        HandleMouseClick();
    }


    // =====================================================
    // クリック処理
    // =====================================================

    private void HandleMouseClick()
    {
        // 左クリックではない
        if (!Input.GetMouseButtonDown(0))
            return;


        // UIをクリックしている場合
        if (TowerBuildUI.IsUIOpen)
            return;


        // カメラ取得
        Camera cam =
            Camera.main;


        if (cam == null)
            return;


        // マウス位置をワールド座標に変換
        Vector3 mousePosition =
            Input.mousePosition;


        Vector3 worldPosition =
            cam.ScreenToWorldPoint(
                mousePosition
            );


        worldPosition.z = 0f;


        // =================================================
        // ① タワーを優先して検索
        // =================================================

        Collider2D towerHit =
            Physics2D.OverlapPoint(
                worldPosition,
                towerClickLayer
            );


        if (towerHit != null)
        {
            TowerClickArea clickArea =
                towerHit.GetComponent<TowerClickArea>();


            if (clickArea != null)
            {
                Tower tower =
                    clickArea.Tower;


                if (tower != null)
                {
                    OpenTowerUI(tower);

                    return;
                }
            }
        }


        // =================================================
        // ② 建設地点を検索
        // =================================================

        Collider2D pointHit =
            Physics2D.OverlapPoint(
                worldPosition,
                placementPointLayer
            );


        if (pointHit != null)
        {
            TowerPlacementPoint point =
                pointHit.GetComponent<
                    TowerPlacementPoint
                >();


            if (point != null)
            {
                OpenBuildUI(point);

                return;
            }
        }
    }


    // =====================================================
    // 建設地点UI
    // =====================================================

    public void OpenBuildUI(
        TowerPlacementPoint point)
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


    // =====================================================
    // タワーUI
    // =====================================================

    public void OpenTowerUI(
        Tower tower)
    {
        if (tower == null)
            return;


        selectedTower = tower;

        selectedPoint = null;


        if (buildUI != null)
        {
            buildUI.ShowTowerMode(
                tower
            );
        }
    }


    // =====================================================
    // 建設
    // =====================================================

    public void BuildTower(
        TowerData towerData)
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


        if (ResourceManager.Instance == null)
        {
            Debug.LogError(
                "ResourceManagerが存在しません。"
            );

            return;
        }


        // お金確認
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
                $"TowerData「{towerData.towerName}」に" +
                "Tower Prefabが設定されていません。"
            );

            return;
        }


        // お金を消費
        if (!ResourceManager.Instance.SpendMoney(
            towerData.buildCost))
        {
            return;
        }


        // =================================================
        // Tower生成
        // =================================================

        GameObject towerObject =
            Instantiate(
                towerData.towerPrefab,
                selectedPoint.transform.position,
                Quaternion.identity
            );


        Tower tower =
            towerObject.GetComponent<Tower>();


        if (tower == null)
        {
            Debug.LogError(
                "Tower PrefabにTower.csがありません。"
            );


            ResourceManager.Instance.AddMoney(
                towerData.buildCost
            );


            Destroy(towerObject);

            return;
        }


        // TowerData設定
        tower.towerData =
            towerData;


        // Tower初期化
        tower.InitializeTower();


        // 建設地点登録
        selectedPoint.SetOccupied(
            tower
        );


        // UIを閉じる
        CloseUI();
    }


    // =====================================================
    // 強化
    // =====================================================

    public void UpgradeSelectedTower()
    {
        if (selectedTower == null)
            return;


        if (selectedTower.towerData == null)
            return;


        if (ResourceManager.Instance == null)
        {
            Debug.LogError(
                "ResourceManagerが存在しません。"
            );

            return;
        }


        TowerData data =
            selectedTower.towerData;


        // 最大レベル
        if (selectedTower.Level >=
            data.maxLevel)
        {
            buildUI?.ShowMaxLevel();

            return;
        }


        int upgradeCost =
            selectedTower.GetUpgradeCost();


        // お金確認
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


        // 強化
        bool success =
            selectedTower.Upgrade();


        if (!success)
        {
            ResourceManager.Instance.AddMoney(
                upgradeCost
            );

            return;
        }


        // UI更新
        if (buildUI != null)
        {
            buildUI.ShowTowerMode(
                selectedTower
            );
        }
    }


    // =====================================================
    // 売却
    // =====================================================

    public void SellSelectedTower()
    {
        if (selectedTower == null)
            return;


        if (ResourceManager.Instance == null)
        {
            Debug.LogError(
                "ResourceManagerが存在しません。"
            );

            return;
        }


        int sellPrice =
            selectedTower.GetSellPrice();


        TowerPlacementPoint point =
            FindPlacementPoint(
                selectedTower.transform.position
            );


        Destroy(
            selectedTower.gameObject
        );


        if (point != null)
        {
            point.ResetPoint();
        }


        ResourceManager.Instance.AddMoney(
            sellPrice
        );


        CloseUI();
    }


    // =====================================================
    // 建設地点検索
    // =====================================================

    private TowerPlacementPoint FindPlacementPoint(
        Vector3 position)
    {
        TowerPlacementPoint[] points =
            FindObjectsByType<
                TowerPlacementPoint
            >(
                FindObjectsSortMode.None
            );


        TowerPlacementPoint closestPoint =
            null;


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


    // =====================================================
    // 選択解除
    // =====================================================

    public void ClearSelection()
    {
        selectedPoint = null;

        selectedTower = null;
    }


    // =====================================================
    // UIを閉じる
    // =====================================================

    public void CloseUI()
    {
        ClearSelection();


        if (buildUI != null)
        {
            buildUI.Hide();
        }
    }
}