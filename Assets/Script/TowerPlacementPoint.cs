using UnityEngine;

/// <summary>
/// タワーを建設できる場所を管理する。
/// </summary>
public class TowerPlacementPoint : MonoBehaviour
{
    [Header("表示")]
    public GameObject availableIndicator;


    [Header("状態")]
    [SerializeField]
    private bool isOccupied = false;


    // 建設されているタワー
    private Tower placedTower;


    // 建設地点Collider
    private Collider2D pointCollider;


    public bool IsOccupied =>
        isOccupied;


    public Tower PlacedTower =>
        placedTower;


    private void Awake()
    {
        pointCollider =
            GetComponent<Collider2D>();
    }


    private void Start()
    {
        UpdateIndicator();
    }


    /// <summary>
    /// タワーを建設したことを登録する。
    /// </summary>
    public void SetOccupied(Tower tower)
    {
        if (tower == null)
        {
            Debug.LogWarning(
                "TowerPlacementPoint: " +
                "nullのTowerを登録しようとしました。"
            );

            return;
        }


        isOccupied = true;

        placedTower = tower;


        UpdateIndicator();


        if (pointCollider != null)
        {
            pointCollider.enabled = false;
        }
    }


    /// <summary>
    /// タワー売却後に再利用可能にする。
    /// </summary>
    public void ResetPoint()
    {
        isOccupied = false;

        placedTower = null;


        if (pointCollider != null)
        {
            pointCollider.enabled = true;
        }


        UpdateIndicator();
    }


    private void UpdateIndicator()
    {
        if (availableIndicator != null)
        {
            availableIndicator.SetActive(
                !isOccupied
            );
        }
    }
}