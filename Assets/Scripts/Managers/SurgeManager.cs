using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SurgeManager : MonoBehaviour
{
    // ── Singleton ─────────────────────────────────────────────────────────────

    public static SurgeManager Instance;

    // ── Inspector ─────────────────────────────────────────────────────────────

    [Header("Surge Settings")]
    [Tooltip("Seconds of survival required to fill the meter from 0 → 100%.")]
    public float timeToFill = 25f;

    [Tooltip("How long Bulldozer Mode stays active once triggered.")]
    public float surgeDuration = 4f;

    [Header("UI References")]
    [Tooltip("Assign the Slider that visually represents the meter.")]
    public Slider surgeSlider;

    [Tooltip("Optional: an Image that glows / pulses when the meter is full. " +
             "Enable/disable it here rather than animating the slider tint.")]
    public Image readyGlowImage;

    // ── Runtime State ─────────────────────────────────────────────────────────

    private float currentFill = 0f;

    /// <summary>True while Bulldozer Mode is active. Read by PlayerController.</summary>
    public bool IsSurging { get; private set; }

    /// <summary>Normalised fill value 0–1. Useful for external UI animations.</summary>
    public float FillRatio => currentFill / timeToFill;

    // ── Internal ──────────────────────────────────────────────────────────────

    private bool wasReadyLastFrame = false;

    // ─────────────────────────────────────────────────────────────────────────
    //  Unity Lifecycle
    // ─────────────────────────────────────────────────────────────────────────

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Update()
    {
        // Meter doesn't fill while Bulldozer Mode is active or game is paused.
        if (IsSurging || Time.timeScale == 0f) return;

        if (currentFill < timeToFill)
        {
            currentFill += Time.deltaTime;

            // Hard clamp – prevents floating-point creep past the ceiling.
            if (currentFill > timeToFill) currentFill = timeToFill;

            RefreshUI();
        }

        // Trigger "meter full" feedback exactly once when it tips over.
        bool isReadyNow = (currentFill >= timeToFill);
        if (isReadyNow && !wasReadyLastFrame)
            OnMeterBecameFull();
        wasReadyLastFrame = isReadyNow;
    }

    // ─────────────────────────────────────────────────────────────────────────
    //  Public API
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Called by PlayerController when a double-tap is detected.
    /// Silently ignored if the meter isn't full or a surge is already running.
    /// </summary>
    public void TryActivateSurge()
    {
        if (currentFill < timeToFill) return;
        if (IsSurging) return;

        StartCoroutine(ActivateSurge());
    }

    /// <summary>
    /// Resets the meter to zero (e.g., on player death / game restart).
    /// </summary>
    public void ResetMeter()
    {
        StopAllCoroutines();
        IsSurging = false;
        currentFill = 0f;
        wasReadyLastFrame = false;
        RefreshUI();

        if (readyGlowImage != null)
            readyGlowImage.enabled = false;
    }

    // ─────────────────────────────────────────────────────────────────────────
    //  Surge Coroutine
    // ─────────────────────────────────────────────────────────────────────────

    private IEnumerator ActivateSurge()
    {
        IsSurging = true;
        currentFill = 0f;   // Drain the meter immediately on activation
        RefreshUI();

        // Hide the ready glow – we're surging now, not "ready"
        if (readyGlowImage != null)
            readyGlowImage.enabled = false;

        // Hook: play surge activation sound 
        // AudioManager.Instance.PlaySFX(surgeActivateSound);

        //Hook: spawn surge VFX on player 
        VFXManager.Instance.PlaySurgeAura(PlayerController.Instance.transform);

        yield return new WaitForSeconds(surgeDuration);

        IsSurging = false;

        // Hook: play surge-ended sound 
        // AudioManager.Instance.PlaySFX(surgeEndSound);
    }

    private void RefreshUI()
    {
        if (surgeSlider != null)
            surgeSlider.value = FillRatio;
    }

    private void OnMeterBecameFull()
    {
        // Show ready glow so the player knows they can double-tap
        if (readyGlowImage != null)
            readyGlowImage.enabled = true;

        // Hook: play "meter full" notification sound 
        // AudioManager.Instance.PlaySFX(meterFullSound);

        // Hook: haptic nudge on mobile
        // Haptics_Manager.Instance.LightTap();
    }

}