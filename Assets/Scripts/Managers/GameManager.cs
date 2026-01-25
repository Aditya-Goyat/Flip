using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] GameObject deathCanvas;

    bool isDead;
    bool hasRevived;

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
        ScoreManager.Instance.StartScore();
    }


    public void OnPlayerDeath()
    {
        if (isDead) return;

        isDead = true;
        Time.timeScale = 0f;
        deathCanvas.SetActive(true);

        ScoreManager.Instance.StopScore();

    }

    public bool CanRevive()
    {
        return !hasRevived;
    }

    public void RevivePlayer()
    {
        hasRevived = true;
        isDead = false;

        Time.timeScale = 1f;
        deathCanvas.SetActive(false);

        PlayerController.Instance.Revive();
        ObstacleCleaner.ClearAll();
        FlipManager.Instance.FreezeFlips(3f);
    }

    public void RestartRun()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
    }
}
