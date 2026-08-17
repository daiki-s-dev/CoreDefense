using UnityEngine;

/// <summary>
/// タワーの基本データ。
/// </summary>
[CreateAssetMenu(
    fileName = "TowerData",
    menuName = "Game/Tower Data"
)]
public class TowerData : ScriptableObject
{
    // =====================================================
    // 基本情報
    // =====================================================

    [Header("基本情報")]
    public string towerName;

    [TextArea]
    public string description;

    public Sprite icon;


    // =====================================================
    // 建設
    // =====================================================

    [Header("建設")]
    public GameObject towerPrefab;

    public int buildCost;


    // =====================================================
    // 基本ステータス
    // =====================================================

    [Header("基本ステータス")]
    public int attackDamage;

    public float attackInterval;

    public float attackRange;


    // =====================================================
    // 強化
    // =====================================================

    [Header("強化")]
    public int maxLevel = 3;

    public int upgradeCost = 100;

    public int upgradeDamage = 10;

    public float upgradeIntervalReduction = 0.1f;

    public float upgradeRange = 0.5f;


    // =====================================================
    // レベルごとの見た目
    // =====================================================

    [Header("レベルごとの見た目")]
    [Tooltip("Lv.1の見た目")]
    public GameObject level1Prefab;

    [Tooltip("Lv.2の見た目")]
    public GameObject level2Prefab;

    [Tooltip("Lv.3の見た目")]
    public GameObject level3Prefab;


    // =====================================================
    // 売却
    // =====================================================

    [Header("売却")]
    [Range(0f, 1f)]
    public float sellRate = 0.5f;
}