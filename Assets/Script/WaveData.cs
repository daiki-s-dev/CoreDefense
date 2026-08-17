using UnityEngine;

/// <summary>
/// 1Wave分の敵出現データ。
/// </summary>
[CreateAssetMenu(
    fileName = "WaveData",
    menuName = "Tower Defense/Wave Data"
)]
public class WaveData : ScriptableObject
{
    [Header("Wave設定")]
    public string waveName = "Wave 1";


    [Tooltip("Wave開始前の待ち時間")]
    public float startDelay = 1f;


    [Tooltip("Waveクリア後、次のWave開始ボタンを表示するまでの時間")]
    public float endDelay = 1f;


    [Header("使用する道")]
    public bool useNorth = true;

    public bool useSouth = false;

    public bool useEast = false;

    public bool useWest = false;


    [Header("敵グループ")]
    public EnemyGroup[] enemyGroups;


    [System.Serializable]
    public class EnemyGroup
    {
        [Header("敵Prefab")]
        public GameObject[] enemyPrefabs;


        [Header("出現数")]
        public int enemyCount = 5;


        [Header("出現間隔")]
        public float spawnInterval = 1f;
    }
}