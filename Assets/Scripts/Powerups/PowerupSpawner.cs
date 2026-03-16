using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PowerupSpawnEntry
{
    public PowerupType type;
    public GameObject prefab;
    public int spawnWeight = 1;
}

public class PowerupSpawner : MonoBehaviour
{
    [Header("Powerup Prefabs")]
    [SerializeField] private List<PowerupSpawnEntry> powerups = new List<PowerupSpawnEntry>();

    [Header("Spawn Area")]
    [SerializeField] private float spawnY = 6f;
    [SerializeField] private float spawnXMin = -2.2f;
    [SerializeField] private float spawnXMax = 2.2f;

    [Header("Spawn Timing")]
    [SerializeField] private float firstSpawnDelay = 10f;
    [SerializeField] private float minSpawnInterval = 12f;
    [SerializeField] private float maxSpawnInterval = 20f;

    [Header("Spawn Limits")]
    [SerializeField] private int maxPowerupsAlive = 1;

    private float nextSpawnTime;

    private void Start()
    {
        ScheduleNextSpawn(firstSpawnDelay);
    }

    private void Update()
    {
        if (Time.time >= nextSpawnTime)
        {
            TrySpawnPowerup();
            ScheduleNextSpawn(Random.Range(minSpawnInterval, maxSpawnInterval));
        }
    }

    private void TrySpawnPowerup()
    {
        GameObject[] existingPowerups = GameObject.FindGameObjectsWithTag("Powerup");
        if (existingPowerups.Length >= maxPowerupsAlive)
        {
            return;
        }

        GameObject selectedPrefab = GetWeightedRandomPowerupPrefab();
        if (selectedPrefab == null) return;

        float x = Random.Range(spawnXMin, spawnXMax);
        Vector2 spawnPos = new Vector2(x, spawnY);

        Instantiate(selectedPrefab, spawnPos, selectedPrefab.transform.rotation);
    }

    private GameObject GetWeightedRandomPowerupPrefab()
    {
        int totalWeight = 0;

        foreach (var p in powerups)
        {
            if (p.prefab == null) continue;
            if (p.spawnWeight <= 0) continue;
            totalWeight += p.spawnWeight;
        }

        if (totalWeight <= 0) return null;

        int randomWeight = Random.Range(0, totalWeight);
        int runningWeight = 0;

        foreach (var p in powerups)
        {
            if (p.prefab == null) continue;
            if (p.spawnWeight <= 0) continue;

            runningWeight += p.spawnWeight;
            if (randomWeight < runningWeight)
            {
                return p.prefab;
            }
        }

        return null;
    }

    private void ScheduleNextSpawn(float delay)
    {
        nextSpawnTime = Time.time + delay;
    }
}