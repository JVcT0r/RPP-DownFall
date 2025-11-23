using UnityEngine;
using System.Collections.Generic;

public class EnemySpawnerBoss : MonoBehaviour
{
    [Header("Prefabs dos inimigos")]
    public GameObject meleeEnemyPrefab;
    public GameObject rangedEnemyPrefab;

    [Header("Configurações de Spawn")]
    public Transform[] spawnPoints;
    public int maxEnemies = 3;
    public float spawnInterval = 3f;

    [Header("Referência do Boss")]
    public BossHealth bossHealth;

    private float timer;
    private List<GameObject> currentEnemies = new List<GameObject>();

    private void Update()
    {
        
        if (bossHealth == null || bossHealth.CurrentRealHealth <= 0)
            return;

        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            timer = 0f;

            
            currentEnemies.RemoveAll(e => e == null);
            
            TrySpawnInAllPoints();
        }
    }

    private void TrySpawnInAllPoints()
    {
        foreach (Transform point in spawnPoints)
        {
            if (currentEnemies.Count >= maxEnemies)
                break; 

            SpawnEnemy(point);
        }
    }

    private void SpawnEnemy(Transform point)
    {
        
        GameObject prefab = Random.value < 0.5f ? meleeEnemyPrefab : rangedEnemyPrefab;

        GameObject enemy = Instantiate(prefab, point.position, Quaternion.identity);

        currentEnemies.Add(enemy);
    }
}