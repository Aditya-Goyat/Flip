using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float speedMultiplier = 1f;
    [SerializeField] private float fallbackSpeed = 4f;
    [SerializeField] private float destroyY = -6f;

    [Header("Spawn Settings")]
    [Tooltip("If true, the obstacle spawner will pause until this obstacle is destroyed.")]
    [SerializeField] private bool pausesSpawning = false;

    // NEW: Lets the Spawner read this variable to know if this is a massive obstacle
    public bool PausesSpawning => pausesSpawning;

    // A global counter that the Spawner can look at to know if it should wait
    public static int ActivePauseObstacles { get; private set; } = 0;

    private DifficultyManager difficulty;

    private void Awake()
    {
        // FIX: Moved this to Awake() so the Spawner knows INSTANTLY before the next frame
        if (pausesSpawning)
        {
            ActivePauseObstacles++;
        }
    }

    private void Start()
    {
        difficulty = FindFirstObjectByType<DifficultyManager>();
    }

    private void Update()
    {
        float baseSpeed = difficulty != null
            ? difficulty.CurrentObstacleSpeed
            : fallbackSpeed;

        float finalSpeed = baseSpeed * speedMultiplier;

        transform.Translate(Vector2.down * finalSpeed * Time.deltaTime, Space.World);

        if (transform.position.y < destroyY)
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (pausesSpawning)
        {
            ActivePauseObstacles--;
            if (ActivePauseObstacles < 0) ActivePauseObstacles = 0;
        }
    }
}