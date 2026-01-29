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
    public Renderer backgroundGridRenderer;

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

        // 4. Change Background Grid (Player Color but Darker)
        if (backgroundGridRenderer != null)
        {
            // Create a darker version of the player color (40% brightness)
            Color bgColor = playerColor * 0.28f;
            bgColor.a = 0.8f; // Ensure alpha stays 100%
            backgroundGridRenderer.material.color = bgColor;
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