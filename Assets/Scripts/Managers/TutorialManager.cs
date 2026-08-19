using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public GameObject tutorialPanel;

    void Start()
    {
        // Check if it's the player's first time
        if (PlayerPrefs.GetInt("HasPlayedBefore", 0) == 0)
        {
            // First time playing! 
            tutorialPanel.SetActive(true);

            // FREEZE THE GAME so obstacles don't kill them while they read!
            Time.timeScale = 0f;
        }
        else
        {
            // Returning player. Make sure panel is hidden and game runs normally.
            tutorialPanel.SetActive(false);
            Time.timeScale = 1f;
        }
    }

    // Link this method to your "GOT IT!" button's OnClick event
    public void CloseTutorialAndPlay()
    {
        // Save that they've seen it
        PlayerPrefs.SetInt("HasPlayedBefore", 1);
        PlayerPrefs.Save();

        // Hide the panel
        tutorialPanel.SetActive(false);

        // UNFREEZE the game so the action starts!
        Time.timeScale = 1f;
    }
}