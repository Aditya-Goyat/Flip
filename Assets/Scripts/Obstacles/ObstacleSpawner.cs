using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [SerializeField] GameObject obstaclePrefab;
    [SerializeField] float spawnXRange = 2.5f;
    [SerializeField] float spawnY = 6f;

    DifficultyManager difficulty;
    float nextSpawnTime;

    void Start()
    {
        difficulty = FindFirstObjectByType<DifficultyManager>();
        nextSpawnTime = Time.time + 1f;
    }

    void Update()
    {
        if (Time.time >= nextSpawnTime)
        {
            Spawn();
            float interval = difficulty != null
                ? difficulty.CurrentSpawnInterval
                : 1.2f;

            nextSpawnTime = Time.time + interval;
        }
    }

    void Spawn()
    {
        float x = Random.Range(-spawnXRange, spawnXRange);
        Instantiate(obstaclePrefab, new Vector2(x, spawnY), Quaternion.identity);
    }
}
