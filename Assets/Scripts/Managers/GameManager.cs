using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI References")]
    [SerializeField] GameObject deathCanvas;

    [Header("HUD Elements")]
    public TextMeshProUGUI glitchWarningText;

    [Header("Theme Settings - World")]
    public Color normalColor = new Color(0f, 1f, 1f);
    public Color glitchColor = new Color(1f, 0.12f, 0.12f);

    [Header("Theme Settings - Player")]
    public SpriteRenderer playerRenderer;
    public Color playerNormalColor = Color.white;
    public Color playerGlitchColor = new Color(1f, 0.9f, 0.2f);

    [Header("Elements to Recolor")]
    public SpriteRenderer[] glitchSprites;
    public TextMeshProUGUI[] uiTexts;

    [Header("Background Layers")]
    public Renderer movingGridRenderer;
    public Renderer secondaryGridRenderer;
    public SpriteRenderer gradientOverlay;
    public SpriteRenderer depthOverlay;

    bool isDead;
    bool hasRevived;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        ApplyColor(false);
        if (glitchWarningText != null) glitchWarningText.gameObject.SetActive(false);
        ScoreManager.Instance.StartScore();
    }

    public void OnPlayerDeath()
    {
        if (isDead) return;
        isDead = true;

        if (ScoreManager.Instance != null) ScoreManager.Instance.StopScore();

        // Check if we are in glitch mode
        bool isGlitch = false;
        if (FlipManager.Instance != null) isGlitch = FlipManager.IsInverted;

        // Trigger VFX Sequence (Particles + Pause)
        if (VFXManager.Instance != null && PlayerController.Instance != null)
        {
            VFXManager.Instance.TriggerDeathSequence(PlayerController.Instance.transform.position, isGlitch);
        }
        else
        {
            ShowDeathScreen(); // Fallback
        }
    }

    // Called by VFXManager after the delay
    public void ShowDeathScreen()
    {
        if (deathCanvas != null) deathCanvas.SetActive(true);
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
        if (deathCanvas != null) deathCanvas.SetActive(false);

        if (PlayerController.Instance != null)
        {
            // FIX: Turn the player's GameObject back on after the VFX Manager hid it
            PlayerController.Instance.gameObject.SetActive(true);

            PlayerController.Instance.Revive();
        }
        ObstacleCleaner.ClearAll();

        if (FlipManager.Instance != null) FlipManager.Instance.FreezeFlips(3f);
    }

    public void RestartRun()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Game");
    }

    public void ToggleGlitchUI(bool isGlitching)
    {
        if (glitchWarningText != null) glitchWarningText.gameObject.SetActive(isGlitching);
        ApplyColor(isGlitching);
        if (isGlitching) StartCoroutine(GlitchTextAnimation());
        else StopAllCoroutines();
    }

    private void ApplyColor(bool isGlitching)
    {
        Color worldColor = isGlitching ? glitchColor : normalColor;
        Color playerColor = isGlitching ? playerGlitchColor : playerNormalColor;

        if (playerRenderer != null) playerRenderer.color = playerColor;

        foreach (var sprite in glitchSprites) if (sprite != null) sprite.color = worldColor;
        foreach (var txt in uiTexts) if (txt != null) txt.color = worldColor;

        if (movingGridRenderer != null)
            movingGridRenderer.material.color = isGlitching ? new Color(0.21f, 0.46f, 0.01f, 1f) : new Color(0f, 0.18f, 0.28f, 1f);

        if (secondaryGridRenderer != null)
            secondaryGridRenderer.material.color = worldColor * 0.6f;

        if (gradientOverlay != null)
            gradientOverlay.color = isGlitching ? new Color(1f, 0.18f, 0f, 1f) : new Color(0.11f, 0.61f, 0.76f, 0.63f);

        if (depthOverlay != null)
            depthOverlay.color = isGlitching ? new Color(1f, 0.58f, 0f, 0.8f) : new Color(1f, 1f, 1f, 1f);
    }

    private IEnumerator GlitchTextAnimation()
    {
        if (!glitchWarningText) yield break;
        RectTransform rect = glitchWarningText.rectTransform;
        Vector2 originalPos = Vector2.zero;

        while (true)
        {
            rect.anchoredPosition = originalPos + new Vector2(Random.Range(-5f, 5f), Random.Range(-5f, 5f));
            glitchWarningText.color = Random.value > 0.8f ? Color.white : glitchColor;
            yield return new WaitForSeconds(0.05f);
        }
    }
}