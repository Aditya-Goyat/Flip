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
    public TextMeshProUGUI glitchWarningText; // Assign the "CONTROLS FLIPPED" text here

    [Header("Theme Settings - World")]
    public Color normalColor = new Color(0f, 1f, 1f); // Cyan
    public Color glitchColor = new Color(1f, 0f, 0.33f); // Neon Red

    [Header("Theme Settings - Player")]
    public SpriteRenderer playerRenderer; // Drag Player object here
    public Color playerNormalColor = Color.white; // High contrast vs Cyan
    public Color playerGlitchColor = new Color(1f, 0.9f, 0.2f); // Yellow (High contrast vs Red)

    [Header("Elements to Recolor")]
    // Drag ALL Sprite Renderers (e.g. your sprite prefabs) here
    public SpriteRenderer[] glitchSprites;
    // Drag ALL Text that should change color
    public TextMeshProUGUI[] uiTexts;
    // Drag the Background Grid object here
    [Header("Background Layers")]
    public Renderer movingGridRenderer;
    public Renderer secondaryGridRenderer; // optional depth layer
    public SpriteRenderer gradientOverlay; // static fade
    public SpriteRenderer depthOverlay;    // optional

    // Game State
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
        // Ensure we start with normal colors
        ApplyColor(false);
        if (glitchWarningText != null) glitchWarningText.gameObject.SetActive(false);

        ScoreManager.Instance.StartScore();
    }

    // --- YOUR ORIGINAL LOGIC ---

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
        SceneManager.LoadScene("Game");
    }

    // --- NEW COLOR CHANGE LOGIC ---

    public void ToggleGlitchUI(bool isGlitching)
    {
        // 1. Show/Hide Warning Text
        if (glitchWarningText != null)
            glitchWarningText.gameObject.SetActive(isGlitching);

        // 2. Apply Colors (Function now handles distinction internally)
        ApplyColor(isGlitching);

        // 3. Text Shake Effect
        if (isGlitching) StartCoroutine(GlitchTextAnimation());
        else StopAllCoroutines();
    }

    private void ApplyColor(bool isGlitching)
    {
        Color worldColor = isGlitching ? glitchColor : normalColor;
        Color playerColor = isGlitching ? playerGlitchColor : playerNormalColor;

        // 1. Change Player Color (Distinct)
        if (playerRenderer != null)
        {
            playerRenderer.color = playerColor;
        }

        // 2. Change Sprite Colors (Prefabs/Obstacles/Environment)
        foreach (var sprite in glitchSprites)
        {
            if (sprite != null) sprite.color = worldColor;
        }

        // 3. Change Text Colors
        foreach (var txt in uiTexts)
        {
            if (txt != null) txt.color = worldColor;
        }

        // 4. Moving Grid Layer
        if (movingGridRenderer != null)
        {
            movingGridRenderer.material.color = worldColor;
        }

        // 5. Secondary Grid (slightly dimmer for depth)
        if (secondaryGridRenderer != null)
        {
            Color dimColor = worldColor * 0.6f;
            dimColor.a = 1f;
            secondaryGridRenderer.material.color = dimColor;
        }

        // 6. Gradient Overlay (never full color shift)
        if (gradientOverlay != null)
        {
            gradientOverlay.color = isGlitching
                ? new Color(1f, 0f, 0f, 0.8f)  // red tint during glitch
                : new Color(0.47f, 0.47f, 0.47f, 0.89f);   // normal dark fade
        }

        // 7. Noise Overlay (subtle tint)
        if (depthOverlay != null)
        {
            depthOverlay.color = isGlitching
                 ? new Color(1f, 0f, 0f, 0.8f)  // red tint during glitch
                 : new Color(1f, 1f, 1f, 1f);   // normal dark fade
        }
  
    }

    private IEnumerator GlitchTextAnimation()
    {
        if (!glitchWarningText) yield break;
        RectTransform rect = glitchWarningText.rectTransform;
        Vector2 originalPos = Vector2.zero; // Assuming centered anchor

        while (true)
        {
            // Shake position
            rect.anchoredPosition = originalPos + new Vector2(Random.Range(-5f, 5f), Random.Range(-5f, 5f));
            // Strobe color to white occasionally
            glitchWarningText.color = Random.value > 0.8f ? Color.white : glitchColor;
            yield return new WaitForSeconds(0.05f);
        }
    }
}