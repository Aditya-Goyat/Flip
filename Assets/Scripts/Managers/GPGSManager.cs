using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;

public class GPGSManager : MonoBehaviour
{
    public static GPGSManager Instance;

    [Tooltip("Paste your specific Leaderboard ID from the Play Console here")]
    public string leaderboardID = "YOUR_LEADERBOARD_ID_HERE";

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Tells Unity to use Google Play Games for social features
        PlayGamesPlatform.Activate();
    }

    private void Start()
    {
        AuthenticateUser();
    }

    public void AuthenticateUser()
    {
        PlayGamesPlatform.Instance.Authenticate((SignInStatus status) =>
        {
            if (status == SignInStatus.Success)
            {
                Debug.Log("Successfully signed into Google Play Games!");
            }
            else
            {
                Debug.LogWarning("Failed to sign in to Google Play. Status: " + status);
            }
        });
    }

    public void SubmitScore(float score)
    {
        if (Social.localUser.authenticated)
        {
            // Convert float score to integer for the leaderboard
            int finalScore = Mathf.FloorToInt(score);

            Social.ReportScore(finalScore, leaderboardID, (bool success) =>
            {
                if (success) Debug.Log("Score posted to Google Play successfully!");
                else Debug.LogWarning("Failed to post score to Google Play.");
            });
        }
    }

    public void ShowLeaderboardUI()
    {
        Debug.Log("ShowLeaderboardUI called");

        if (Social.localUser.authenticated)
        {
            Debug.Log("Authenticated, opening leaderboard");

            PlayGamesPlatform.Instance.ShowLeaderboardUI();
        }
        else
        {
            Debug.LogWarning("User not authenticated");
            AuthenticateUser();
        }
    }
}