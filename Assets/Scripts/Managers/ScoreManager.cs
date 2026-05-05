using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    [Header("Scoring Settings")]
    [SerializeField] private float baseScoreMultiplier = 10f; // Points per second at start
    [SerializeField] private float maxScoreMultiplier = 50f;  // Points per second at max speed

    private float score;
    private bool isRunning;
    private DifficultyManager difficulty;

    public float CurrentScore => score;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        // Cache the difficulty manager to avoid finding it every frame
        difficulty = FindFirstObjectByType<DifficultyManager>();
    }

    void Update()
    {
        if (!isRunning) return;

        // Calculate how fast the score should increase based on difficulty
        float multiplier = baseScoreMultiplier;

        if (difficulty != null)
        {
            // We use the Difficulty01 (0 to 1) to Lerp between base and max scoring speed
            multiplier = Mathf.Lerp(baseScoreMultiplier, maxScoreMultiplier, difficulty.Difficulty01);
        }

        // Add to the score based on the current speed/difficulty
        score += Time.deltaTime * multiplier;
    }

    public void StartScore()
    {
        score = 0f;
        isRunning = true;
    }

    public void StopScore()
    {
        isRunning = false;
        SaveBestScore();
    }

    public void ResumeScore()
    {
        isRunning = true;
    }

    void SaveBestScore()
    {
        float best = PlayerPrefs.GetFloat("BEST_SCORE", 0f);
        if (score > best)
        {
            PlayerPrefs.SetFloat("BEST_SCORE", score);
            PlayerPrefs.Save();
        }
    }

    public float GetBestScore()
    {
        return PlayerPrefs.GetFloat("BEST_SCORE", 0f);
    }
}