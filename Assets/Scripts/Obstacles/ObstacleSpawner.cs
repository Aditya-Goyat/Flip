using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ObstacleType
{
    public string name;
    public GameObject prefab;

    [Tooltip("This obstacle starts spawning after this many seconds.")]
    public float unlockTime = 0f;

    [Tooltip("Higher weight = spawns more often among unlocked obstacles.")]
    public int spawnWeight = 1;
}

public class ObstacleSpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DifficultyManager difficulty;

    [Header("Obstacle Types")]
    [SerializeField] private List<ObstacleType> obstacleTypes = new List<ObstacleType>();

    [Header("Spawn Area")]
    [SerializeField] private float spawnY = 6f;
    [SerializeField] private float[] laneXPositions = new float[] { -2.0f, -1.0f, 0f, 1.0f, 2.0f };

    [Header("Timing")]
    [SerializeField] private float startDelay = 1f;

    [Header("Pattern Difficulty")]
    [SerializeField] private float doubleSpawnStartDifficulty = 0.25f;
    [SerializeField] private float burstSpawnStartDifficulty = 0.55f;

    [SerializeField] private float maxDoubleSpawnChance = 0.30f;
    [SerializeField] private float maxBurstChance = 0.15f;

    [Header("Burst Settings")]
    [SerializeField] private int minBurstCount = 2;
    [SerializeField] private int maxBurstCount = 3;
    [SerializeField] private float burstGap = 0.12f;

    [Header("Fairness")]
    [SerializeField] private int maxSameLaneStreak = 2;

    private float nextSpawnTime;
    private int lastLane = -1;
    private int sameLaneStreak = 0;

    private void Start()
    {
        if (difficulty == null)
        {
            difficulty = FindFirstObjectByType<DifficultyManager>();
        }

        nextSpawnTime = Time.time + startDelay;
    }

    private void Update()
    {
        if (Time.time >= nextSpawnTime)
        {
            SpawnPattern();

            float interval = difficulty != null
                ? difficulty.CurrentSpawnInterval
                : 1.2f;

            nextSpawnTime = Time.time + interval;
        }
    }

    private void SpawnPattern()
    {
        float difficulty01 = difficulty != null ? difficulty.Difficulty01 : 0f;

        float doubleChance = 0f;
        float burstChance = 0f;

        if (difficulty01 >= doubleSpawnStartDifficulty)
        {
            float t = Mathf.InverseLerp(doubleSpawnStartDifficulty, 1f, difficulty01);
            doubleChance = Mathf.Lerp(0f, maxDoubleSpawnChance, t);
        }

        if (difficulty01 >= burstSpawnStartDifficulty)
        {
            float t = Mathf.InverseLerp(burstSpawnStartDifficulty, 1f, difficulty01);
            burstChance = Mathf.Lerp(0f, maxBurstChance, t);
        }

        float roll = Random.value;

        if (roll < burstChance)
        {
            int burstCount = Random.Range(minBurstCount, maxBurstCount + 1);
            StartCoroutine(SpawnBurst(burstCount));
        }
        else if (roll < burstChance + doubleChance)
        {
            SpawnDouble();
        }
        else
        {
            SpawnSingle();
        }
    }

    private void SpawnSingle()
    {
        int lane = GetNextFairLane();
        GameObject obstaclePrefab = GetRandomUnlockedObstaclePrefab();

        if (obstaclePrefab != null)
        {
            SpawnAtLane(lane, obstaclePrefab);
        }
    }

    private void SpawnDouble()
    {
        if (laneXPositions.Length < 2)
        {
            SpawnSingle();
            return;
        }

        int firstLane = GetNextFairLane();
        int secondLane = firstLane;

        int safety = 0;
        while (secondLane == firstLane && safety < 10)
        {
            secondLane = Random.Range(0, laneXPositions.Length);
            safety++;
        }

        GameObject firstPrefab = GetRandomUnlockedObstaclePrefab();
        GameObject secondPrefab = GetRandomUnlockedObstaclePrefab();

        if (firstPrefab != null)
        {
            SpawnAtLane(firstLane, firstPrefab);
        }

        if (secondPrefab != null)
        {
            SpawnAtLane(secondLane, secondPrefab);
        }
    }

    private IEnumerator SpawnBurst(int count)
    {
        for (int i = 0; i < count; i++)
        {
            int lane = GetNextFairLane();
            GameObject obstaclePrefab = GetRandomUnlockedObstaclePrefab();

            if (obstaclePrefab != null)
            {
                SpawnAtLane(lane, obstaclePrefab);
            }

            if (i < count - 1)
            {
                yield return new WaitForSeconds(burstGap);
            }
        }
    }

    private int GetNextFairLane()
    {
        if (laneXPositions.Length == 0) return 0;
        if (laneXPositions.Length == 1) return 0;

        int lane = Random.Range(0, laneXPositions.Length);

        if (sameLaneStreak >= maxSameLaneStreak && lane == lastLane)
        {
            int safety = 0;
            while (lane == lastLane && safety < 10)
            {
                lane = Random.Range(0, laneXPositions.Length);
                safety++;
            }
        }

        if (lane == lastLane)
            sameLaneStreak++;
        else
            sameLaneStreak = 0;

        lastLane = lane;
        return lane;
    }

    private GameObject GetRandomUnlockedObstaclePrefab()
    {
        float survivalTime = Time.timeSinceLevelLoad;

        List<ObstacleType> unlocked = new List<ObstacleType>();
        int totalWeight = 0;

        foreach (ObstacleType obstacleType in obstacleTypes)
        {
            if (obstacleType.prefab == null) continue;
            if (survivalTime < obstacleType.unlockTime) continue;
            if (obstacleType.spawnWeight <= 0) continue;

            unlocked.Add(obstacleType);
            totalWeight += obstacleType.spawnWeight;
        }

        if (unlocked.Count == 0)
        {
            return null;
        }

        int randomWeight = Random.Range(0, totalWeight);

        int runningWeight = 0;
        foreach (ObstacleType obstacleType in unlocked)
        {
            runningWeight += obstacleType.spawnWeight;

            if (randomWeight < runningWeight)
            {
                return obstacleType.prefab;
            }
        }

        return unlocked[unlocked.Count - 1].prefab;
    }

    private void SpawnAtLane(int laneIndex, GameObject obstaclePrefab)
    {
        if (laneIndex < 0 || laneIndex >= laneXPositions.Length) return;
        if (obstaclePrefab == null) return;

        Vector2 spawnPos = new Vector2(laneXPositions[laneIndex], spawnY);
        Instantiate(obstaclePrefab, spawnPos, obstaclePrefab.transform.rotation);
    }
}