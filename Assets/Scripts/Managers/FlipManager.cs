using UnityEngine;
using System.Collections;

public class FlipManager : MonoBehaviour
{
    public static bool IsInverted { get; private set; }

    [Header("Base Timing")]
    [SerializeField] float startMinFlipTime = 4f;
    [SerializeField] float startMaxFlipTime = 7f;

    [Header("Endgame Timing")]
    [SerializeField] float endMinFlipTime = 1.5f;
    [SerializeField] float endMaxFlipTime = 3f;

    [Header("Ramp")]
    [SerializeField] float rampStartTime = 15f;
    [SerializeField] float fullRampTime = 90f;

    [Header("Safety")]
    [SerializeField] float minGapBetweenFlips = 1f;

    [Header("Audio")]
    [Tooltip("Assign the sound effect to play when controls flip")]
    [SerializeField] private AudioClip flipSound;
    [Tooltip("Assign an AudioSource component attached to this GameObject")]
    [SerializeField] private AudioSource flipAudioSource;

    float lastFlipTime;

    public static FlipManager Instance;

    private void Awake()
    {
        Instance = this;

        // Auto-grab AudioSource if not manually assigned
        if (flipAudioSource == null)
            flipAudioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        IsInverted = false;
        lastFlipTime = -minGapBetweenFlips;
        StartCoroutine(FlipRoutine());
    }

    private IEnumerator FlipRoutine()
    {
        while (true)
        {
            float t = GetRampT();
            float minTime = Mathf.Lerp(startMinFlipTime, endMinFlipTime, t);
            float maxTime = Mathf.Lerp(startMaxFlipTime, endMaxFlipTime, t);

            float wait = Random.Range(minTime, maxTime);
            yield return new WaitForSeconds(wait);

            if (Time.time - lastFlipTime < minGapBetweenFlips)
                continue;

            DoFlip();
        }
    }

    private float GetRampT()
    {
        float elapsed = Time.timeSinceLevelLoad - rampStartTime;
        return Mathf.Clamp01(elapsed / fullRampTime);
    }

    private void DoFlip()
    {
        IsInverted = !IsInverted;
        lastFlipTime = Time.time;

        // Play flip haptic
        if (Haptics_Manager.Instance != null)
        {
            Haptics_Manager.Instance.FlipTap();
        }

        // Play the flip sound effect
        if (flipAudioSource != null && flipSound != null)
        {
            flipAudioSource.PlayOneShot(flipSound);
        }

        if (ScreenFlash.Instance != null)
        {
            ScreenFlash.Instance.Flash();
        }

        // Tell the GameManager to switch the UI/Background colors
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ToggleGlitchUI(IsInverted);
        }
    }

    public void FreezeFlips(float duration)
    {
        StopAllCoroutines();
        StartCoroutine(FreezeRoutine(duration));
    }

    private IEnumerator FreezeRoutine(float duration)
    {
        IsInverted = false;

        // Reset the UI colors to Normal while frozen
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ToggleGlitchUI(false);
        }

        yield return new WaitForSeconds(duration);
        StartCoroutine(FlipRoutine());
    }
}