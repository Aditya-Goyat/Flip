using UnityEngine;
using TMPro;

public class ScoreDisplay : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI currentScoreText;
    [SerializeField] private TextMeshProUGUI bestScoreText;

    // OnEnable is called automatically whenever the Death Canvas is turned on (SetActive(true))
    void OnEnable()
    {
        if (ScoreManager.Instance == null) return;

        // Convert the float score to an integer (Mathf.FloorToInt) so it looks clean (e.g., "45" instead of "45.32")
        int current = Mathf.FloorToInt(ScoreManager.Instance.CurrentScore);
        int best = Mathf.FloorToInt(ScoreManager.Instance.GetBestScore());

        if (currentScoreText != null)
            currentScoreText.text = "SCORE: " + current.ToString();

        if (bestScoreText != null)
            bestScoreText.text = "BEST: " + best.ToString();
    }
}