using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
    [Header("Time")]
    [SerializeField] float rampStartTime = 10f;

    [Header("Obstacle Speed")]
    [SerializeField] float baseObstacleSpeed = 4f;
    [SerializeField] float maxObstacleSpeed = 9f;
    [SerializeField] float speedRampRate = 0.05f;

    [Header("Spawn Rate")]
    [SerializeField] float baseSpawnInterval = 1.2f;
    [SerializeField] float minSpawnInterval = 0.45f;
    [SerializeField] float spawnRampRate = 0.01f;

    public float CurrentObstacleSpeed { get; private set; }
    public float CurrentSpawnInterval { get; private set; }

    void Update()
    {
        float t = Mathf.Max(0f, Time.timeSinceLevelLoad - rampStartTime);

        CurrentObstacleSpeed =
            Mathf.Min(maxObstacleSpeed, baseObstacleSpeed + t * speedRampRate);

        CurrentSpawnInterval =
            Mathf.Max(minSpawnInterval, baseSpawnInterval - t * spawnRampRate);
    }
}
