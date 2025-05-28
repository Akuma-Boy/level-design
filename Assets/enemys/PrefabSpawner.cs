using UnityEngine;
using System.Collections.Generic;

public class PrefabSpawner : MonoBehaviour
{
    [Header("Prefab e Posição")]
    public GameObject prefabToSpawn;
    public Transform spawnPoint;
    public float spawnRadius = 0f; // 0 = posição exata

    [Header("Tempo e Limite")]
    public float spawnInterval = 60f; // tempo entre spawns em segundos
    public int maxActiveSpawns = 0;   // 0 = ilimitado

    private float nextSpawnTime;
    private List<GameObject> activeSpawns = new List<GameObject>();

    void Update()
    {
        if (Time.time >= nextSpawnTime)
        {
            TrySpawn();
            nextSpawnTime = Time.time + spawnInterval;
        }

        CleanupDestroyedSpawns();
    }

    void TrySpawn()
    {
        if (prefabToSpawn == null || spawnPoint == null)
        {
            Debug.LogWarning("Prefab ou spawn point não definidos!", this);
            return;
        }

        if (maxActiveSpawns > 0 && activeSpawns.Count >= maxActiveSpawns)
            return;

        Vector3 spawnPosition = spawnPoint.position;
        if (spawnRadius > 0f)
        {
            Vector2 offset = Random.insideUnitCircle * spawnRadius;
            spawnPosition += new Vector3(offset.x, 0, offset.y);
        }

        GameObject newSpawn = Instantiate(prefabToSpawn, spawnPosition, spawnPoint.rotation);
        activeSpawns.Add(newSpawn);
    }

    void CleanupDestroyedSpawns()
    {
        activeSpawns.RemoveAll(spawn => spawn == null);
    }

    void OnDrawGizmos()
    {
        if (spawnPoint == null)
            return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(spawnPoint.position, spawnRadius);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, spawnPoint.position);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(spawnPoint.position + Vector3.up * 0.5f, Vector3.one * 0.5f);
    }
}
