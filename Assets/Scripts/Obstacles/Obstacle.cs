using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [SerializeField] private float speedMultiplier = 1f;
    [SerializeField] private float fallbackSpeed = 4f;
    [SerializeField] private float destroyY = -6f;

    private DifficultyManager difficulty;

    private void Start()
    {
        difficulty = FindFirstObjectByType<DifficultyManager>();
    }

    private void Update()
    {
        float baseSpeed = difficulty != null
            ? difficulty.CurrentObstacleSpeed
            : fallbackSpeed;

        float powerupMultiplier = 1f;
        if (PowerupManager.Instance != null)
        {
            powerupMultiplier = PowerupManager.Instance.ObstacleSpeedMultiplier;
        }

        float finalSpeed = baseSpeed * speedMultiplier * powerupMultiplier;

        transform.Translate(Vector2.down * finalSpeed * Time.deltaTime, Space.World);

        if (transform.position.y < destroyY)
        {
            Destroy(gameObject);
        }
    }
}