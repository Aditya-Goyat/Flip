using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    float score;
    bool isRunning;

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

    void Update()
    {
        if (!isRunning) return;

        score += Time.deltaTime;
    }

    public void StartScore()
    {
        score = 0f;
        isRunning = true;
    }

    // New method to unpause without resetting to zero
    public void ResumeScore()
    {
        isRunning = true;
    }

    public void StopScore()
    {
        isRunning = false;
        SaveBestScore();
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