using UnityEngine;
using System.Collections;

/// <summary>
/// Spawns collectable objects at random positions with a minimum distance from player.
/// </summary>
public class ObjectSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject[] objectsToSpawn;
    [SerializeField] private float spawnInterval = 1f;
    [SerializeField] private Vector2 spawnAreaX = new Vector2(-50f, 50f);
    [SerializeField] private Vector2 spawnAreaZ = new Vector2(-50f, 50f);
    [SerializeField] private float spawnHeight = 0f;
    [SerializeField] private int maxObjects = 10;
    [SerializeField] private float objectLifetime = 30f;

    [Header("Avoid Player")]
    [SerializeField] private Character player;
    [SerializeField] private float minDistanceFromPlayer = 5f;

    private int activeObjects;

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
                Debug.LogWarning("Player not found in ObjectSpawner!", this);
            }
        }

        StartCoroutine(SpawnObjectsContinuously());
    }

    /// <summary>
    /// Continuously spawns objects at fixed intervals.
    /// </summary>
    private IEnumerator SpawnObjectsContinuously()
    {
        while (true)
        {
            if (activeObjects < maxObjects)
            {
                Vector3 spawnPosition = GetRandomSpawnPosition();
                if (spawnPosition != Vector3.zero)
                {
                    GameObject objectToSpawn = objectsToSpawn[Random.Range(0, objectsToSpawn.Length)];
                    GameObject spawnedObject = Instantiate(objectToSpawn, spawnPosition, Quaternion.identity);
                    activeObjects++;
                    // Destroy object after lifetime to prevent clutter
                    Destroy(spawnedObject, objectLifetime);
                }
            }
            yield return new WaitForSeconds(spawnInterval);
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
    /// Updates the active object count when an object is destroyed.
    /// </summary>
    private void OnObjectDestroyed(GameObject obj)
    {
        if (obj != null)
            activeObjects = Mathf.Max(0, activeObjects - 1);
    }
}