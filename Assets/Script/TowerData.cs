using UnityEngine;

/// <summary>
/// タワー1種類分の性能データ。
/// </summary>
[CreateAssetMenu(
    fileName = "TowerData",
    menuName = "Tower Defense/Tower Data"
)]
public class TowerData : ScriptableObject
{
    [Header("基本情報")]
    public string towerName = "通常タワー";

    [TextArea(2, 4)]
    public string description = "バランスの良い基本タワーです。";


    [Header("見た目")]
    public Sprite icon;


    [Header("Prefab")]
    public GameObject towerPrefab;


    [Header("初期性能")]
    public int attackDamage = 10;

    [Tooltip("攻撃間隔（秒）")]
    public float attackInterval = 1.0f;

    [Tooltip("攻撃範囲")]
    public float attackRange = 3.0f;


    [Header("建設")]
    public int buildCost = 50;


    [Header("強化")]
    [Tooltip("最大レベル")]
    public int maxLevel = 3;

    [Tooltip("1回強化するための基本価格")]
    public int upgradeCost = 50;

    [Tooltip("強化1回あたりの攻撃力増加量")]
    public int upgradeDamage = 5;

    [Tooltip("強化1回あたりの射程増加量")]
    public float upgradeRange = 0.2f;

    [Tooltip("強化1回あたりの攻撃間隔減少量")]
    public float upgradeIntervalReduction = 0.1f;


    [Header("売却")]
    [Tooltip("売却時に返ってくる割合")]
    [Range(0f, 1f)]
    public float sellRate = 0.5f;
}