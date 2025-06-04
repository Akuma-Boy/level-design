using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ItemSpawner : MonoBehaviour
{
    [System.Serializable]
    public class SpawnableItem
    {
        public GameObject prefab;
        [Range(0, 100)] public float spawnChance = 50f;
        public int maxSpawnCount = 5;
    }

    [Header("Configurações Básicas")]
    [SerializeField] private List<SpawnableItem> itemsToSpawn = new List<SpawnableItem>();
    [SerializeField] private bool spawnOnStart = true;
    [SerializeField] private float initialSpawnDelay = 0f;
    [SerializeField] private float spawnInterval = 5f;
    [SerializeField] private int maxTotalItems = 10;

    [Header("Área de Spawn")]
    [SerializeField] private bool spawnInArea = false;
    [SerializeField] private Vector3 spawnAreaSize = new Vector3(5f, 0f, 5f);
    [SerializeField] private List<Transform> spawnPoints = new List<Transform>();

    private List<GameObject> spawnedItems = new List<GameObject>();

    void Start()
    {
        if (spawnOnStart)
        {
            InvokeRepeating(nameof(SpawnItems), initialSpawnDelay, spawnInterval);
        }
    }

    public void SpawnItems()
    {
        // Limpa itens destruídos da lista
        spawnedItems.RemoveAll(item => item == null);

        if (spawnedItems.Count >= maxTotalItems) return;

        foreach (var item in itemsToSpawn)
        {
            if (ShouldSpawn(item.spawnChance) && GetSpawnedCount(item.prefab) < item.maxSpawnCount)
            {
                SpawnItem(item.prefab);
            }
        }
    }

    private bool ShouldSpawn(float chance)
    {
        return Random.Range(0f, 100f) <= chance;
    }

    private int GetSpawnedCount(GameObject prefab)
    {
        return spawnedItems.Count(item => item != null && item.name.StartsWith(prefab.name));
    }

    private void SpawnItem(GameObject prefab)
    {
        Vector3 spawnPosition = GetSpawnPosition();
        Quaternion spawnRotation = Quaternion.Euler(0, Random.Range(0, 360f), 0);

        GameObject newItem = Instantiate(prefab, spawnPosition, spawnRotation);
        newItem.name = $"{prefab.name}_spawned"; // Marca como item spawnado

        spawnedItems.Add(newItem);
    }

    private Vector3 GetSpawnPosition()
    {
        if (spawnPoints.Count > 0 && !spawnInArea)
        {
            // Spawn em pontos definidos
            return spawnPoints[Random.Range(0, spawnPoints.Count)].position;
        }
        else if (spawnInArea)
        {
            // Spawn em área definida
            Vector3 randomOffset = new Vector3(
                Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2),
                0,
                Random.Range(-spawnAreaSize.z / 2, spawnAreaSize.z / 2)
            );
            return transform.position + randomOffset;
        }
        else
        {
            // Spawn na posição do objeto
            return transform.position;
        }
    }

    public void ClearAllItems()
    {
        foreach (var item in spawnedItems)
        {
            if (item != null) Destroy(item);
        }
        spawnedItems.Clear();
    }

    void OnDrawGizmosSelected()
    {
        if (spawnInArea)
        {
            Gizmos.color = new Color(0, 1, 0, 0.3f);
            Gizmos.DrawCube(transform.position, spawnAreaSize);
        }

        Gizmos.color = Color.red;
        foreach (var point in spawnPoints)
        {
            if (point != null) Gizmos.DrawSphere(point.position, 0.5f);
        }
    }
}