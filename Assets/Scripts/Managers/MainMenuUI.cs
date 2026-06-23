using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] GameObject settingsPanel;

    [Header("Buttons")]
    public Button playButton;
    public Button openSettingsButton;
    public Button closeSettingsButton;
    public Button exitButton;

    [Header("Transition")]
    public GameObject menuUI;
    public Animator shipAnimator;
    public CanvasGroup spaceBackground;  // drag your space BG CanvasGroup here
    public GameObject boostVFX;

    [Header("Camera Shake")]
    public Camera mainCamera;            // drag your Main Camera here
    public float shakeDuration = 0.4f;
    public float shakeMagnitude = 0.08f;

    [Header("Timing")]
    public float fadeInDuration = 0.8f;      // how long space BG fades in
    public float fadeOutDuration = 0.5f;     // how long space BG fades out before scene loads
    public float blastoffClipDuration = 1f;  // exact length of your Blastoff clip in seconds
    public string gameplaySceneName = "GameplayScene";

    [Header("Audio")]
    public AudioClip uiClickSound;

    private bool isStarting = false;

    void Start()
    {
        // Space background starts invisible
        if (spaceBackground != null) spaceBackground.alpha = 0f;

        if (playButton) playButton.onClick.AddListener(Play);
        if (openSettingsButton) openSettingsButton.onClick.AddListener(OpenSettings);
        if (closeSettingsButton) closeSettingsButton.onClick.AddListener(CloseSettings);
        if (exitButton) exitButton.onClick.AddListener(Exit);
    }

    void PlayClickSound()
    {
        if (AudioManager.Instance != null && uiClickSound != null)
            AudioManager.Instance.PlaySFX(uiClickSound);
    }

    public void Play()
    {
        if (isStarting) return;
        PlayClickSound();
        if (playButton) StartCoroutine(AnimateButton(playButton.transform));
        StartCoroutine(LaunchSequence());
    }

    private IEnumerator LaunchSequence()
    {
        isStarting = true;

        // Lock all buttons immediately
        if (playButton) playButton.interactable = false;
        if (openSettingsButton) openSettingsButton.interactable = false;
        if (exitButton) exitButton.interactable = false;

        // 1. Hide menu UI, turn VFX on — stays on through entire sequence
        if (menuUI != null) menuUI.SetActive(false);
        if (boostVFX != null) boostVFX.SetActive(true);

        // 2. Start pre-loading scene silently in background
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(gameplaySceneName);
        asyncLoad.allowSceneActivation = false;

        // 3. Fade space background IN (0 → 1) while scene pre-loads
        float timer = 0f;
        while (timer < fadeInDuration)
        {
            timer += Time.deltaTime;
            if (spaceBackground != null)
                spaceBackground.alpha = Mathf.Clamp01(timer / fadeInDuration);
            yield return null;
        }
        if (spaceBackground != null) spaceBackground.alpha = 1f;

        // 4. Camera shake the moment blastoff triggers
        if (mainCamera != null)
            StartCoroutine(CameraShake());

        // 5. Fire the Blastoff animation
        if (shipAnimator != null)
            shipAnimator.SetTrigger("Blastoff");

        // 6. Wait for the full blastoff clip to finish
        yield return new WaitForSeconds(blastoffClipDuration);

        // 7. Fade space background OUT (1 → 0) before switching scenes
        timer = 0f;
        while (timer < fadeOutDuration)
        {
            timer += Time.deltaTime;
            if (spaceBackground != null)
                spaceBackground.alpha = Mathf.Lerp(1f, 0f, timer / fadeOutDuration);
            yield return null;
        }
        if (spaceBackground != null) spaceBackground.alpha = 0f;

        // 8. Scene is fully pre-loaded — activate it instantly
        asyncLoad.allowSceneActivation = true;
    }

    private IEnumerator CameraShake()
    {
        Vector3 originalPos = mainCamera.transform.localPosition;
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * shakeMagnitude;
            float y = Random.Range(-1f, 1f) * shakeMagnitude;

            mainCamera.transform.localPosition = new Vector3(
                originalPos.x + x,
                originalPos.y + y,
                originalPos.z
            );

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Always restore exact original position after shake
        mainCamera.transform.localPosition = originalPos;
    }

    public void OpenSettings()
    {
        PlayClickSound();
        if (openSettingsButton) StartCoroutine(AnimateButton(openSettingsButton.transform));
        settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        PlayClickSound();
        if (closeSettingsButton) StartCoroutine(AnimateButton(closeSettingsButton.transform));
        settingsPanel.SetActive(false);
    }

    public void Exit()
    {
        PlayClickSound();
        if (exitButton) StartCoroutine(AnimateButton(exitButton.transform));
        Invoke(nameof(QuitApp), 0.2f);
    }

    private void QuitApp()
    {
        Application.Quit();
    }

    private IEnumerator AnimateButton(Transform btn)
    {
        Vector3 original = Vector3.one;
        Vector3 punch = Vector3.one * 0.9f;
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