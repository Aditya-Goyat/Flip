using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class DeathUI : MonoBehaviour
{
    [SerializeField] GameObject continueButton;
    //[SerializeField] TMP_Text finalScoreText;
    //[SerializeField] TMP_Text bestScoreText;

    void OnEnable()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager.Instance is NULL");
            return;
        }

        if (continueButton != null)
        {
            continueButton.SetActive(GameManager.Instance.CanRevive());
        }
        else
        {
            Debug.LogError("Continue Button reference is NULL");
        }

        //// Score display (safe)
        //if (ScoreManager.Instance != null)
        //{
        //    finalScoreText.text =
        //        $"Score: {Mathf.FloorToInt(ScoreManager.Instance.CurrentScore)}";

        //    bestScoreText.text =
        //        $"Best: {Mathf.FloorToInt(ScoreManager.Instance.GetBestScore())}";
        //}
    }

    public void Restart()
    {
        GameManager.Instance.RestartRun();
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void ContinueWithAd()
    {
        if (!GameManager.Instance.CanRevive())
            return;

        AdsManager.Instance.ShowRewarded(() =>
        {
            GameManager.Instance.RevivePlayer();
            // FIX: Removed ScoreManager.Instance.StartScore() from here!
            // GameManager.RevivePlayer() now automatically calls ResumeScore() behind the scenes.
        });
    }
}