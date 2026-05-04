using UnityEngine;

public class ChaserObstacle : MonoBehaviour
{
    [Header("Chaser Settings")]
    [Tooltip("How fast the obstacle moves horizontally towards the player.")]
    [SerializeField] private float trackingSpeed = 2.5f;
    [SerializeField] private float fallbackDownSpeed = 4f;
    [SerializeField] private float destroyY = -6f;

    private DifficultyManager difficulty;
    private Transform playerTransform;

    void Start()
    {
        // Grab the difficulty manager to match the global falling speed
        difficulty = FindFirstObjectByType<DifficultyManager>();

        // Find the player safely using the Singleton we already built
        if (PlayerController.Instance != null)
        {
            playerTransform = PlayerController.Instance.transform;
        }
    }

    void Update()
    {
        // 1. Calculate how fast we should fall based on the current difficulty
        float downSpeed = difficulty != null ? difficulty.CurrentObstacleSpeed : fallbackDownSpeed;

        Vector3 currentPos = transform.position;

        // 2. Track the player horizontally (if the player exists and is alive)
        if (playerTransform != null && playerTransform.gameObject.activeInHierarchy)
        {
            // Mathf.MoveTowards smoothly glides the X position toward the player at a set speed
            currentPos.x = Mathf.MoveTowards(currentPos.x, playerTransform.position.x, trackingSpeed * Time.deltaTime);
        }

        // 3. Apply the downward movement
        currentPos.y -= downSpeed * Time.deltaTime;

        // 4. Update the actual position
        transform.position = currentPos;

        // 5. Clean up when it goes off screen
        if (transform.position.y < destroyY)
        {
            Destroy(gameObject);
        }
    }
}