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

    [Header("Audio")]
    [Tooltip("Assign the audio clip you want to play when Surge activates.")]
    public AudioClip surgeActivateSound;

    [Tooltip("Assign an AudioSource, or let the script find one automatically.")]
    public AudioSource audioSource;


    // ── Runtime State ─────────────────────────────────────────────────────────
    private float currentFill = 0f;

    /// <summary>True while Bulldozer Mode is active. Read by PlayerController.</summary>
    public bool IsSurging { get; private set; }

    /// <summary>Normalised fill value 0–1. Useful for external UI animations.</summary>
    public float FillRatio => Mathf.Clamp01(currentFill / timeToFill);

    // ── Internal ──────────────────────────────────────────────────────────────
    private bool wasReadyLastFrame = false;

    // ─────────────────────────────────────────────────────────────────────────
    //  Unity Lifecycle
    // ─────────────────────────────────────────────────────────────────────────
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    private void Update()
    {
        // Meter accumulation handling (Only runs when NOT surging and NOT paused)
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
    public void TryActivateSurge()
    {
        if (currentFill < timeToFill) return;
        if (IsSurging) return;

        StartCoroutine(ActivateSurge());
    }

    public void ResetMeter()
    {
        StopAllCoroutines();
        IsSurging = false;
        currentFill = 0f;
        wasReadyLastFrame = false;
        RefreshUI();

        if (readyGlowImage != null)
            readyGlowImage.enabled = false;

        // Safety check: Reset camera back to baseline view if game resets mid-surge
        if (Camerashake.Instance != null)
        {
            Camerashake.Instance.ResetZoom();
        }
    }

    // ─────────────────────────────────────────────────────────────────────────
    //  Surge Coroutine (Modified for Smooth Draining & Camera Juice Hooks)
    // ─────────────────────────────────────────────────────────────────────────
    private IEnumerator ActivateSurge()
    {
        IsSurging = true;

        // Hide the ready glow – we're surging now, not "ready"
        if (readyGlowImage != null)
            readyGlowImage.enabled = false;

        // --- PLAY AUDIO ---
        if (surgeActivateSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(surgeActivateSound);
        }

        // --- CAMERA JUICE ACTIVE HOOK ---
        if (Camerashake.Instance != null)
        {
            Camerashake.Instance.TriggerBoostZoom();
        }

        // Hook: spawn surge VFX on player 
        // VFXManager.Instance.PlaySurgeAura(PlayerController.Instance.transform);

        // Track how long we've been draining
        float elapsed = 0f;

        while (elapsed < surgeDuration)
        {
            elapsed += Time.deltaTime;

            // Calculate the drain percentage (going from 1 down to 0)
            float t = 1f - (elapsed / surgeDuration);

            // Map that percentage smoothly back to currentFill
            currentFill = t * timeToFill;

            RefreshUI();
            yield return null; // Wait for the next frame
        }

        // Ensure it is completely empty at the absolute end of the duration
        currentFill = 0f;
        RefreshUI();

        IsSurging = false;

        // --- CAMERA JUICE RESET HOOK ---
        if (Camerashake.Instance != null)
        {
            Camerashake.Instance.ResetZoom();
        }

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
        if (readyGlowImage != null)
            readyGlowImage.enabled = true;

        // Hook: play "meter full" notification sound 
        // AudioManager.Instance.PlaySFX(meterFullSound);

        // Hook: haptic nudge on mobile
        // Haptics_Manager.Instance.LightTap();
    }
}