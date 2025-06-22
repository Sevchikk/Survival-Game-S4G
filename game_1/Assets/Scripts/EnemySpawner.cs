using UnityEngine;
using System.Collections;

/// <summary>
/// Spawns enemies at random positions with a minimum distance from player.
/// </summary>
public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject[] objectsToSpawn;
    [SerializeField] private Vector2 spawnIntervalRange = new Vector2(3f, 7f);
    [SerializeField] private Vector2 spawnAreaX = new Vector2(-50f, 50f);
    [SerializeField] private Vector2 spawnAreaZ = new Vector2(-50f, 50f);
    [SerializeField] private float spawnHeight = 0f;
    private const int MAX_ENEMIES = 8;

    [Header("Avoid Player")]
    [SerializeField] private Character player;
    [SerializeField] private float minDistanceFromPlayer = 10f;

    private int activeEnemies;

    private void Start()
    {
        // Validate spawn settings
        if (objectsToSpawn == null || objectsToSpawn.Length == 0)
        {
            Debug.LogError("No objects to spawn assigned!", this);
            enabled = false;
            return;
        }

        // Find player if not assigned
        if (player == null)
        {
            player = FindFirstObjectByType<Character>();
            if (player == null)
            {
                Debug.LogWarning("Player not found in EnemySpawner!", this);
            }
        }

        StartCoroutine(SpawnEnemiesContinuously());
    }

    /// <summary>
    /// Continuously spawns enemies at random intervals.
    /// </summary>
    private IEnumerator SpawnEnemiesContinuously()
    {
        while (true)
        {
            if (activeEnemies < MAX_ENEMIES)
            {
                Vector3 spawnPosition = GetRandomSpawnPosition();
                if (spawnPosition != Vector3.zero)
                {
                    GameObject objectToSpawn = objectsToSpawn[Random.Range(0, objectsToSpawn.Length)];
                    GameObject spawnedObject = Instantiate(objectToSpawn, spawnPosition, Quaternion.identity);
                    spawnedObject.SetActive(true);
                    activeEnemies++;
                }
            }
            float randomInterval = Random.Range(spawnIntervalRange.x, spawnIntervalRange.y);
            yield return new WaitForSeconds(randomInterval);
        }
    }

    /// <summary>
    /// Generates a random spawn position, avoiding the player.
    /// </summary>
    private Vector3 GetRandomSpawnPosition()
    {
        const int maxAttempts = 30;
        for (int attempts = 0; attempts < maxAttempts; attempts++)
        {
            Vector3 position = new Vector3(
                Random.Range(spawnAreaX.x, spawnAreaX.y),
                spawnHeight,
                Random.Range(spawnAreaZ.x, spawnAreaZ.y)
            );
            if (player == null || Vector3.Distance(position, player.transform.position) >= minDistanceFromPlayer)
            {
                return position;
            }
        }
        Debug.LogWarning("Could not find a valid spawn position!", this);
        return Vector3.zero;
    }

    /// <summary>
    /// Updates the active enemy count when an enemy is destroyed.
    /// </summary>
    public void OnEnemyDestroyed(GameObject enemy)
    {
        if (enemy != null)
        {
            activeEnemies = Mathf.Max(0, activeEnemies - 1);
        }
    }
}