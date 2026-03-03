using UnityEngine;
using System.Collections;

public class CollectibleSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject collectiblePrefab;
    public float spawnInterval = 8f;
    public float spawnXLimit = 2.0f;

    [Tooltip("Match this to your Obstacle Spawner's Y position (e.g., 6)")]
    public float spawnY = 6f;

    [Tooltip("How much space the collectible needs to avoid obstacles")]
    public float safeRadius = 0.6f;

    [Header("Group Settings")]
    [Tooltip("Minimum number of collectibles in a line")]
    public int minGroupSize = 2;
    [Tooltip("Maximum number of collectibles in a line")]
    public int maxGroupSize = 4;
    [Tooltip("Vertical distance between each collectible in the group")]
    public float spacingY = 1.0f;

    void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            if (collectiblePrefab != null)
            {
                SpawnSafely();
            }
        }
    }

    void SpawnSafely()
    {
        int maxAttempts = 5; // Try 5 times to find an empty spot

        // Pick a random number of collectibles for this group (e.g., 3 to 5)
        int groupSize = Random.Range(minGroupSize, maxGroupSize + 1);

        for (int i = 0; i < maxAttempts; i++)
        {
            float randomX = Random.Range(-spawnXLimit, spawnXLimit);
            Vector2 baseSpawnPos = new Vector2(randomX, spawnY);

            // Draw an invisible circle to see if anything is already here
            Collider2D hit = Physics2D.OverlapCircle(baseSpawnPos, safeRadius);

            // If we hit nothing, OR we hit something that IS NOT an obstacle, it's safe!
            if (hit == null || !hit.CompareTag("Obstacle"))
            {
                // Spawn the entire group in a vertical line
                for (int j = 0; j < groupSize; j++)
                {
                    // Stagger their Y positions so they form a trail
                    Vector2 spawnPos = new Vector2(randomX, spawnY + (j * spacingY));
                    Instantiate(collectiblePrefab, spawnPos, Quaternion.identity);
                }
                break; // Successfully spawned the group, stop trying
            }
        }
    }
}