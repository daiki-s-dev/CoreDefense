using System.Collections;
using UnityEngine;

/// <summary>
/// 敵をスポーンする。
/// 1つのSpawnerにつき1本のWaypointPathを担当する。
/// </summary>
public class EnemySpawner : MonoBehaviour
{
    [Header("Spawner設定")]
    public Transform spawnPoint;

    public WaypointPath waypointPath;

    public GameObject coreObject;


    /// <summary>
    /// 現在スポーン中か。
    /// </summary>
    public bool IsSpawning { get; private set; }


    private Coroutine spawnCoroutine;


    /// <summary>
    /// Waveのスポーンを開始する。
    /// </summary>
    public void StartWave(
        WaveData waveData)
    {
        if (waveData == null)
            return;


        StopWave();


        spawnCoroutine =
            StartCoroutine(
                SpawnWaveCoroutine(
                    waveData
                )
            );
    }


    /// <summary>
    /// スポーンを停止する。
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
    }


    private IEnumerator SpawnWaveCoroutine(
        WaveData waveData)
    {
        IsSpawning = true;


        foreach (
            WaveData.EnemyGroup enemyGroup
            in waveData.enemyGroups)
        {
            if (enemyGroup == null)
                continue;


            if (
                enemyGroup.enemyPrefabs == null ||
                enemyGroup.enemyPrefabs.Length == 0
            )
            {
                continue;
            }


            for (
                int i = 0;
                i < enemyGroup.enemyCount;
                i++
            )
            {
                SpawnEnemy(enemyGroup);


                if (
                    enemyGroup.spawnInterval > 0f
                )
                {
                    yield return new WaitForSeconds(
                        enemyGroup.spawnInterval
                    );
                }
            }
        }


        IsSpawning = false;

        spawnCoroutine = null;
    }


    /// <summary>
    /// 敵を1体生成する。
    /// </summary>
    private void SpawnEnemy(
        WaveData.EnemyGroup enemyGroup)
    {
        if (
            enemyGroup.enemyPrefabs == null ||
            enemyGroup.enemyPrefabs.Length == 0
        )
        {
            return;
        }


        GameObject enemyPrefab =
            enemyGroup.enemyPrefabs[
                Random.Range(
                    0,
                    enemyGroup.enemyPrefabs.Length
                )
            ];


        if (enemyPrefab == null)
            return;


        Vector3 spawnPosition =
            spawnPoint != null
            ? spawnPoint.position
            : transform.position;


        GameObject enemyObject =
            Instantiate(
                enemyPrefab,
                spawnPosition,
                Quaternion.identity
            );


        Enemy enemy =
            enemyObject.GetComponent<Enemy>();


        if (enemy == null)
        {
            Debug.LogError(
                $"{enemyPrefab.name}: Enemy.csがありません。",
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
}