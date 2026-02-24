using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class MainMenuUI : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] GameObject settingsPanel;

    [Header("Buttons")]
    public Button playButton;
    public Button openSettingsButton;
    public Button closeSettingsButton;
    public Button exitButton;

    [Header("Audio")]
    [Tooltip("The sound to play when any UI button is clicked")]
    public AudioClip uiClickSound;

    void Start()
    {
        // Add listeners directly in code to avoid drag-and-drop errors in the Inspector
        if (playButton) playButton.onClick.AddListener(Play);
        if (openSettingsButton) openSettingsButton.onClick.AddListener(OpenSettings);
        if (closeSettingsButton) closeSettingsButton.onClick.AddListener(CloseSettings);
        if (exitButton) exitButton.onClick.AddListener(Exit);
    }

    void PlayClickSound()
    {
        if (AudioManager.Instance != null && uiClickSound != null)
        {
            AudioManager.Instance.PlaySFX(uiClickSound);
        }
    }

    public void Play()
    {
        PlayClickSound(); // Play the sound!

        if (playButton) StartCoroutine(AnimateButton(playButton.transform));

        // Delay scene load slightly to let sound and animation play
        Invoke(nameof(LoadGameScene), 0.2f);
    }

    public void OpenSettings()
    {
        PlayClickSound(); // Play the sound!
        if (openSettingsButton) StartCoroutine(AnimateButton(openSettingsButton.transform));

        settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        PlayClickSound(); // Play the sound!
        if (closeSettingsButton) StartCoroutine(AnimateButton(closeSettingsButton.transform));

        settingsPanel.SetActive(false);
    }

    public void Exit()
    {
        PlayClickSound(); // Play the sound!
        if (exitButton) StartCoroutine(AnimateButton(exitButton.transform));

        Invoke(nameof(QuitApp), 0.2f);
    }

    private void LoadGameScene()
    {
        SceneManager.LoadScene("Game");
    }

    private void QuitApp()
    {
        Application.Quit();
    }

    // Small "Click" punch effect
    private IEnumerator AnimateButton(Transform btn)
    {
        Vector3 original = Vector3.one;
        Vector3 punch = Vector3.one * 0.9f; // Shrink slightly

        float duration = 0.1f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            btn.localScale = Vector3.Lerp(original, punch, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        btn.localScale = original;
    }
}
