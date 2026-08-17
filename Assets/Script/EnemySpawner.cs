using System.Collections;
using UnityEngine;

/// <summary>
/// 1方向から敵を出現させるSpawner。
///
/// WaveDataから自分の方向に設定された敵数を取得し、
/// その数だけ敵を出現させる。
///
/// 例：
/// NorthSpawner
/// → Northの敵数だけ出現
///
/// SouthSpawner
/// → Southの敵数だけ出現
/// </summary>
public class EnemySpawner : MonoBehaviour
{
    // =====================================================
    // 方向
    // =====================================================

    public enum SpawnDirection
    {
        North,
        South,
        East,
        West
    }


    [Header("Spawner方向")]
    [Tooltip("このSpawnerが担当する方向")]
    public SpawnDirection direction;


    // =====================================================
    // Spawn設定
    // =====================================================

    [Header("Spawn設定")]

    [Tooltip("敵を出現させる位置")]
    public Transform spawnPoint;


    [Tooltip("敵が進むWaypointPath")]
    public WaypointPath waypointPath;


    [Tooltip("敵が到達するコア")]
    public GameObject coreObject;


    // =====================================================
    // 状態
    // =====================================================

    /// <summary>
    /// 現在Spawnerが敵を出しているか。
    /// </summary>
    public bool IsSpawning
    {
        get;
        private set;
    }


    /// <summary>
    /// 現在のWaveでこのSpawnerが出す予定の敵数。
    /// </summary>
    public int CurrentSpawnCount
    {
        get;
        private set;
    }


    private Coroutine spawnCoroutine;


    // =====================================================
    // Wave開始
    // =====================================================

    /// <summary>
    /// Waveのスポーンを開始する。
    /// </summary>
    public void StartWave(WaveData waveData)
    {
        if (waveData == null)
        {
            Debug.LogError(
                $"{gameObject.name}: WaveDataがnullです。",
                this
            );

            return;
        }


        // すでにスポーン中なら停止
        StopWave();


        // 今回出す敵数を取得
        CurrentSpawnCount =
            GetSpawnCount(waveData);


        // 敵数が0なら何もしない
        if (CurrentSpawnCount <= 0)
        {
            IsSpawning = false;
            return;
        }


        // 敵グループを取得
        WaveData.EnemyGroup[] groups =
            GetEnemyGroups(waveData);


        if (
            groups == null ||
            groups.Length == 0
        )
        {
            Debug.LogWarning(
                $"{gameObject.name}: " +
                $"{direction}にEnemyGroupが設定されていません。",
                this
            );

            IsSpawning = false;
            return;
        }


        spawnCoroutine =
            StartCoroutine(
                SpawnWaveCoroutine(
                    groups,
                    CurrentSpawnCount
                )
            );
    }


    // =====================================================
    // Spawn Coroutine
    // =====================================================

    private IEnumerator SpawnWaveCoroutine(
        WaveData.EnemyGroup[] groups,
        int enemyCount)
    {
        IsSpawning = true;


        int spawnedCount = 0;


        while (spawnedCount < enemyCount)
        {
            // 有効なグループを取得
            WaveData.EnemyGroup group =
                GetRandomValidGroup(groups);


            if (group == null)
            {
                Debug.LogError(
                    $"{gameObject.name}: " +
                    $"{direction}に有効なEnemyGroupがありません。",
                    this
                );

                break;
            }


            // 敵を出現
            SpawnEnemy(group);


            spawnedCount++;


            // 最後の1体なら待たない
            if (spawnedCount >= enemyCount)
                break;


            // 出現間隔
            float interval =
                Mathf.Max(
                    0f,
                    group.spawnInterval
                );


            if (interval > 0f)
            {
                yield return new WaitForSeconds(
                    interval
                );
            }
            else
            {
                yield return null;
            }
        }


        IsSpawning = false;

        spawnCoroutine = null;
    }


    // =====================================================
    // 敵数取得
    // =====================================================

    /// <summary>
    /// WaveDataからこのSpawnerの敵数を取得する。
    /// </summary>
    private int GetSpawnCount(
        WaveData waveData)
    {
        switch (direction)
        {
            case SpawnDirection.North:

                if (!waveData.useNorth)
                    return 0;

                return Mathf.Max(
                    0,
                    waveData.northEnemyCount
                );


            case SpawnDirection.South:

                if (!waveData.useSouth)
                    return 0;

                return Mathf.Max(
                    0,
                    waveData.southEnemyCount
                );


            case SpawnDirection.East:

                if (!waveData.useEast)
                    return 0;

                return Mathf.Max(
                    0,
                    waveData.eastEnemyCount
                );


            case SpawnDirection.West:

                if (!waveData.useWest)
                    return 0;

                return Mathf.Max(
                    0,
                    waveData.westEnemyCount
                );
        }


        return 0;
    }


    // =====================================================
    // EnemyGroup取得
    // =====================================================

    /// <summary>
    /// WaveDataからこのSpawner用のEnemyGroupを取得する。
    /// </summary>
    private WaveData.EnemyGroup[] GetEnemyGroups(
        WaveData waveData)
    {
        switch (direction)
        {
            case SpawnDirection.North:

                return waveData.northEnemyGroups;


            case SpawnDirection.South:

                return waveData.southEnemyGroups;


            case SpawnDirection.East:

                return waveData.eastEnemyGroups;


            case SpawnDirection.West:

                return waveData.westEnemyGroups;
        }


        return null;
    }


    // =====================================================
    // 有効なEnemyGroup取得
    // =====================================================

    private WaveData.EnemyGroup GetRandomValidGroup(
        WaveData.EnemyGroup[] groups)
    {
        if (
            groups == null ||
            groups.Length == 0
        )
        {
            return null;
        }


        // 有効なグループを探す
        int validCount = 0;


        foreach (
            WaveData.EnemyGroup group
            in groups)
        {
            if (IsValidGroup(group))
            {
                validCount++;
            }
        }


        if (validCount == 0)
            return null;


        // 有効なグループからランダム選択
        int randomIndex =
            Random.Range(
                0,
                validCount
            );


        int currentIndex = 0;


        foreach (
            WaveData.EnemyGroup group
            in groups)
        {
            if (!IsValidGroup(group))
                continue;


            if (currentIndex == randomIndex)
            {
                return group;
            }


            currentIndex++;
        }


        return null;
    }


    private bool IsValidGroup(
        WaveData.EnemyGroup group)
    {
        if (group == null)
            return false;


        if (
            group.enemyPrefabs == null ||
            group.enemyPrefabs.Length == 0
        )
        {
            return false;
        }


        // nullではないPrefabが1つでもあるか
        foreach (
            GameObject prefab
            in group.enemyPrefabs)
        {
            if (prefab != null)
                return true;
        }


        return false;
    }


    // =====================================================
    // 敵生成
    // =====================================================

    private void SpawnEnemy(
        WaveData.EnemyGroup group)
    {
        if (group == null)
            return;


        // 有効なPrefabをランダム取得
        GameObject enemyPrefab =
            GetRandomEnemyPrefab(group);


        if (enemyPrefab == null)
        {
            Debug.LogError(
                $"{gameObject.name}: " +
                $"{direction}のEnemyPrefabが見つかりません。",
                this
            );

            return;
        }


        // Spawn位置
        Vector3 spawnPosition =
            transform.position;


        if (spawnPoint != null)
        {
            spawnPosition =
                spawnPoint.position;
        }


        // 敵生成
        GameObject enemyObject =
            Instantiate(
                enemyPrefab,
                spawnPosition,
                Quaternion.identity
            );


        // Enemy取得
        Enemy enemy =
            enemyObject.GetComponent<Enemy>();


        if (enemy == null)
        {
            Debug.LogError(
                $"{enemyObject.name}: " +
                "Enemy.csがありません。",
                enemyObject
            );

            Destroy(enemyObject);

            return;
        }


        // WaypointとCoreを設定
        enemy.SetPath(
            waypointPath,
            coreObject
        );


        // WaveManagerへ登録
        if (WaveManager.Instance != null)
        {
            WaveManager.Instance.RegisterEnemy(
                enemy
            );
        }
    }


    // =====================================================
    // EnemyPrefab取得
    // =====================================================

    private GameObject GetRandomEnemyPrefab(
        WaveData.EnemyGroup group)
    {
        if (
            group == null ||
            group.enemyPrefabs == null ||
            group.enemyPrefabs.Length == 0
        )
        {
            return null;
        }


        // 有効なPrefab数
        int validCount = 0;


        foreach (
            GameObject prefab
            in group.enemyPrefabs)
        {
            if (prefab != null)
            {
                validCount++;
            }
        }


        if (validCount == 0)
            return null;


        int randomIndex =
            Random.Range(
                0,
                validCount
            );


        int currentIndex = 0;


        foreach (
            GameObject prefab
            in group.enemyPrefabs)
        {
            if (prefab == null)
                continue;


            if (currentIndex == randomIndex)
            {
                return prefab;
            }


            currentIndex++;
        }


        return null;
    }


    // =====================================================
    // Wave停止
    // =====================================================

    /// <summary>
    /// このSpawnerのWaveを停止する。
    /// </summary>
    public void StopWave()
    {
        if (spawnCoroutine != null)
        {
            StopCoroutine(
                spawnCoroutine
            );

            spawnCoroutine = null;
        }


        IsSpawning = false;
        CurrentSpawnCount = 0;
    }
}