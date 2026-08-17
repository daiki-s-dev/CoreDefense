using UnityEngine;

/// <summary>
/// 1Wave分の敵出現データ。
///
/// 各方向ごとに出現する敵数を設定する。
///
/// 例：
/// North = 5
/// South = 5
/// East  = 10
/// West  = 0
///
/// この場合、Wave全体の敵数は20体。
/// </summary>
[CreateAssetMenu(
    fileName = "WaveData",
    menuName = "Tower Defense/Wave Data"
)]
public class WaveData : ScriptableObject
{
    // =====================================================
    // Wave設定
    // =====================================================

    [Header("Wave設定")]

    [Tooltip("Waveの名前")]
    public string waveName = "Wave 1";


    [Tooltip("Wave開始前の待ち時間")]
    public float startDelay = 1f;


    [Tooltip("Waveクリア後、次のWave開始ボタンを表示するまでの時間")]
    public float endDelay = 1f;


    // =====================================================
    // North
    // =====================================================

    [Header("North")]
    [Tooltip("Northから敵を出すか")]
    public bool useNorth = true;


    [Tooltip("Northから出現する敵の総数")]
    [Min(0)]
    public int northEnemyCount = 5;


    [Tooltip("Northから出現する敵グループ")]
    public EnemyGroup[] northEnemyGroups;


    // =====================================================
    // South
    // =====================================================

    [Header("South")]
    [Tooltip("Southから敵を出すか")]
    public bool useSouth = false;


    [Tooltip("Southから出現する敵の総数")]
    [Min(0)]
    public int southEnemyCount = 0;


    [Tooltip("Southから出現する敵グループ")]
    public EnemyGroup[] southEnemyGroups;


    // =====================================================
    // East
    // =====================================================

    [Header("East")]
    [Tooltip("Eastから敵を出すか")]
    public bool useEast = false;


    [Tooltip("Eastから出現する敵の総数")]
    [Min(0)]
    public int eastEnemyCount = 0;


    [Tooltip("Eastから出現する敵グループ")]
    public EnemyGroup[] eastEnemyGroups;


    // =====================================================
    // West
    // =====================================================

    [Header("West")]
    [Tooltip("Westから敵を出すか")]
    public bool useWest = false;


    [Tooltip("Westから出現する敵の総数")]
    [Min(0)]
    public int westEnemyCount = 0;


    [Tooltip("Westから出現する敵グループ")]
    public EnemyGroup[] westEnemyGroups;


    // =====================================================
    // 総敵数
    // =====================================================

    /// <summary>
    /// このWaveで実際に出現する敵の総数を取得する。
    /// </summary>
    public int GetTotalEnemyCount()
    {
        int total = 0;


        if (useNorth)
        {
            total += Mathf.Max(
                0,
                northEnemyCount
            );
        }


        if (useSouth)
        {
            total += Mathf.Max(
                0,
                southEnemyCount
            );
        }


        if (useEast)
        {
            total += Mathf.Max(
                0,
                eastEnemyCount
            );
        }


        if (useWest)
        {
            total += Mathf.Max(
                0,
                westEnemyCount
            );
        }


        return total;
    }


    // =====================================================
    // 敵グループ
    // =====================================================

    [System.Serializable]
    public class EnemyGroup
    {
        [Header("敵Prefab")]

        [Tooltip("このグループから出現させる敵Prefab")]
        public GameObject[] enemyPrefabs;


        [Header("出現間隔")]

        [Tooltip("この敵を出す間隔")]
        public float spawnInterval = 1f;
    }
}