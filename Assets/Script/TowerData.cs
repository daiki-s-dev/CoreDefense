using UnityEngine;

/// <summary>
/// タワー1種類分の性能データ。
/// ScriptableObjectとして作成して使用する。
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

    [Header("性能")]
    public int attackDamage = 10;

    [Tooltip("攻撃間隔（秒）")]
    public float attackInterval = 1.0f;

    [Tooltip("攻撃範囲")]
    public float attackRange = 3.0f;

    [Header("建設")]
    public int buildCost = 50;
}