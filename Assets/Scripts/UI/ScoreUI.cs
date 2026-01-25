using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreUI : MonoBehaviour
{
    [SerializeField] TMP_Text scoreText;

    void Update()
    {
        float score = ScoreManager.Instance.CurrentScore;
        scoreText.text = Mathf.FloorToInt(score).ToString();
    }
}
