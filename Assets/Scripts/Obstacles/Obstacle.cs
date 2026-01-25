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

        transform.Translate(Vector2.down * speed * Time.deltaTime);

        if (transform.position.y < -6f)
            Destroy(gameObject);
    }
}
