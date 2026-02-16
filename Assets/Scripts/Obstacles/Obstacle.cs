using UnityEngine;

public class Obstacle : MonoBehaviour
{
    DifficultyManager difficulty;

    void Start()
    {
        difficulty = FindFirstObjectByType<DifficultyManager>();
    }

    void Update()
    {
        float speed = difficulty != null
            ? difficulty.CurrentObstacleSpeed
            : 4f;

        // FIX: Added Space.World to force movement straight down regardless of rotation
        transform.Translate(Vector2.down * speed * Time.deltaTime, Space.World);

        if (transform.position.y < -6f)
            Destroy(gameObject);
    }
}