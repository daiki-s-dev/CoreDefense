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

    // この場所に建設されているタワー
    private Tower placedTower;

    // 建設地点のCollider
    private Collider2D pointCollider;


    /// <summary>
    /// 現在タワーが建設されているか。
    /// </summary>
    public bool IsOccupied => isOccupied;


    /// <summary>
    /// 現在建設されているタワー。
    /// </summary>
    public Tower PlacedTower => placedTower;


    private void Awake()
    {
        pointCollider = GetComponent<Collider2D>();
    }


    private void Start()
    {
        UpdateIndicator();
    }


    /// <summary>
    /// 建設地点をクリックしたとき。
    /// </summary>
    private void OnMouseDown()
    {
        // すでにタワーがある場合は何もしない
        if (isOccupied)
            return;

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
    /// タワーを建設したことを登録する。
    /// </summary>
    public void SetOccupied(Tower tower)
    {
        if (tower == null)
        {
            Debug.LogWarning(
                "TowerPlacementPoint: nullのTowerを登録しようとしました。"
            );

            return;
        }

        isOccupied = true;
        placedTower = tower;

        UpdateIndicator();

        // 建設地点自身のColliderを無効化
        // タワーのColliderでクリックできるようにする
        if (pointCollider != null)
        {
            pointCollider.enabled = false;
        }
    }


    /// <summary>
    /// タワー売却後に建設地点を再利用可能にする。
    /// </summary>
    public void ResetPoint()
    {
        isOccupied = false;
        placedTower = null;

        // 建設地点のColliderを再び有効化
        if (pointCollider != null)
        {
            pointCollider.enabled = true;
        }

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