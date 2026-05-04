using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI References")]
    [SerializeField] GameObject deathCanvas;

    [Header("HUD Elements (Sprite 2D)")]
    [Tooltip("Drag your 2D Sprite object here")]
    public Transform glitchWarningSprite;
    public float offScreenX = -10f;
    public float onScreenX = 0f;
    public float slideDuration = 0.5f;

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
    private Coroutine slideCoroutine;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        ApplyColor(false);

        // Ensure the sprite starts hidden off-screen
        if (glitchWarningSprite != null)
        {
            Vector3 startPos = glitchWarningSprite.position;
            startPos.x = offScreenX;
            glitchWarningSprite.position = startPos;
        }

        if (ScoreManager.Instance != null) ScoreManager.Instance.StartScore();
    }

    public void OnPlayerDeath()
    {
        if (isDead) return;
        isDead = true;
        if (ScoreManager.Instance != null) ScoreManager.Instance.StopScore();

        bool isGlitch = false;
        if (FlipManager.Instance != null) isGlitch = FlipManager.IsInverted;

        if (VFXManager.Instance != null && PlayerController.Instance != null)
        {
            VFXManager.Instance.TriggerDeathSequence(PlayerController.Instance.transform.position, isGlitch);
        }
        else
        {
            ShowDeathScreen();
        }
    }

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
            PlayerController.Instance.gameObject.SetActive(true);
            PlayerController.Instance.Revive();
        }

        if (FlipManager.Instance != null) FlipManager.Instance.FreezeFlips(3f);

        // Resume the score timer where it left off
        if (ScoreManager.Instance != null) ScoreManager.Instance.ResumeScore();
    }

    public void RestartRun()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Game");
    }

    public void ToggleGlitchUI(bool isGlitching)
    {
        ApplyColor(isGlitching);

        // Slide the 2D Sprite in or out
        if (glitchWarningSprite != null)
        {
            if (slideCoroutine != null) StopCoroutine(slideCoroutine);

            float targetX = isGlitching ? onScreenX : offScreenX;
            slideCoroutine = StartCoroutine(SlideSpriteRoutine(targetX));
        }
    }

    private IEnumerator SlideSpriteRoutine(float targetX)
    {
        float elapsedTime = 0f;
        Vector3 startPos = glitchWarningSprite.position;

        while (elapsedTime < slideDuration)
        {
            elapsedTime += Time.deltaTime;

            // SmoothStep creates a nice "ease in, ease out" flow animation
            float t = Mathf.SmoothStep(0f, 1f, elapsedTime / slideDuration);

            Vector3 newPos = startPos;
            newPos.x = Mathf.Lerp(startPos.x, targetX, t);
            glitchWarningSprite.position = newPos;

            yield return null;
        }

        // Snap exactly to the target to finish
        Vector3 finalPos = startPos;
        finalPos.x = targetX;
        glitchWarningSprite.position = finalPos;
    }

    private void ApplyColor(bool isGlitching)
    {
        Color worldColor = isGlitching ? glitchColor : normalColor;
        Color playerColor = isGlitching ? playerGlitchColor : playerNormalColor;

        if (playerRenderer != null) playerRenderer.color = playerColor;

        foreach (var sprite in glitchSprites)
            if (sprite != null) sprite.color = worldColor;

        foreach (var txt in uiTexts)
            if (txt != null) txt.color = worldColor;

        if (movingGridRenderer != null)
            movingGridRenderer.material.color = isGlitching ?
                new Color(0.21f, 0.46f, 0.01f, 1f) : new Color(0f, 0.18f, 0.28f, 1f);

        if (secondaryGridRenderer != null)
            secondaryGridRenderer.material.color = worldColor * 0.6f;

        if (gradientOverlay != null)
            gradientOverlay.color = isGlitching ?
                new Color(1f, 0.18f, 0f, 1f) : new Color(0.11f, 0.61f, 0.76f, 0.63f);

        if (depthOverlay != null)
            depthOverlay.color = isGlitching ?
                new Color(1f, 0.58f, 0f, 0.8f) : new Color(1f, 1f, 1f, 1f);
    }
}