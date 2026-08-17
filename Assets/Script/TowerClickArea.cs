using UnityEngine;

/// <summary>
/// タワー本体のクリック判定用。
///
/// 実際のクリック処理は
/// TowerPlacementManagerが行う。
/// </summary>
public class TowerClickArea : MonoBehaviour
{
    private Tower tower;

    public Tower Tower => tower;


    private void Awake()
    {
        tower =
            GetComponentInParent<Tower>();


        if (tower == null)
        {
            Debug.LogError(
                $"{gameObject.name}: " +
                "親にTower.csが見つかりません。",
                this
            );
        }
    }
}