using UnityEngine;

[CreateAssetMenu(
    fileName = "TowerData",
    menuName = "Tower Defense/Tower Data"
)]
public class TowerData : ScriptableObject
{
    [Header("基本情報")]
    public string towerName;

    [TextArea]
    public string description;

    public Sprite icon;


    [Header("建設")]
    public int buildCost = 100;


    [Header("基本ステータス")]
    public int attackDamage = 30;

    public float attackInterval = 1.2f;

    public float attackRange = 3.5f;


    [Header("強化")]
    public int maxLevel = 3;

    public int upgradeCost = 50;

    public int upgradeDamage = 10;

    public float upgradeIntervalReduction = 0.2f;

    public float upgradeRange = 0.5f;


    [Header("売却")]
    [Range(0f, 1f)]
    public float sellRate = 0.7f;


    // =====================================================
    // タワーPrefab
    // =====================================================

    [Header("タワーPrefab")]

    [Tooltip("建設時に生成するタワー本体Prefab")]
    public GameObject towerPrefab;


    [Header("レベル別見た目Prefab")]

    [Tooltip("Lv1の見た目")]
    public GameObject level1Prefab;

    [Tooltip("Lv2の見た目")]
    public GameObject level2Prefab;

    [Tooltip("Lv3の見た目")]
    public GameObject level3Prefab;

    [Tooltip("Lv4の見た目")]
    public GameObject level4Prefab;

    [Tooltip("Lv5の見た目")]
    public GameObject level5Prefab;


    /// <summary>
    /// 指定したレベルの見た目Prefabを取得する。
    /// </summary>
    public GameObject GetLevelPrefab(int level)
    {
        switch (level)
        {
            case 1:
                return level1Prefab;

            case 2:
                return level2Prefab;

            case 3:
                return level3Prefab;

            case 4:
                return level4Prefab;

            case 5:
                return level5Prefab;

            default:
                return null;
        }
    }
}