using UnityEngine;

/// <summary>
/// タワーを建設できる場所を管理する。
/// </summary>
public class TowerPlacementPoint : MonoBehaviour
{
    [Header("表示")]
    public GameObject availableIndicator;

    [Header("設定")]
    [Tooltip("この場所に建設済みか")]
    [SerializeField]
    private bool isOccupied = false;


    /// <summary>
    /// 建設済みか。
    /// </summary>
    public bool IsOccupied => isOccupied;


    private void Start()
    {
        UpdateIndicator();
    }


    /// <summary>
    /// 建設地点をクリックしたとき。
    /// </summary>
    private void OnMouseDown()
    {
        if (isOccupied)
        {
            Debug.Log("この場所にはすでにタワーがあります。");
            return;
        }

        if (TowerPlacementManager.Instance == null)
        {
            Debug.LogError(
                "TowerPlacementManagerが存在しません。"
            );

            return;
        }

        TowerPlacementManager.Instance.OpenBuildUI(this);
    }


    /// <summary>
    /// タワー建設済みにする。
    /// </summary>
    public void SetOccupied()
    {
        isOccupied = true;

        UpdateIndicator();
    }


    /// <summary>
    /// 建設可能表示を更新する。
    /// </summary>
    private void UpdateIndicator()
    {
        if (availableIndicator != null)
        {
            availableIndicator.SetActive(!isOccupied);
        }
    }
}