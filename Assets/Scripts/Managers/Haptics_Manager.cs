using System.Collections;
using UnityEngine;

public class Haptics_Manager : MonoBehaviour
{
    public static Haptics_Manager Instance;

    [Header("Enable Haptics")]
    [SerializeField] private bool enableHaptics = true;

    [Header("Timing")]
    [SerializeField] private float lightGap = 0.035f;
    [SerializeField] private float mediumGap = 0.05f;

    private bool isPlayingPattern;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        transform.SetParent(null); // Detach from parent before DontDestroyOnLoad
        DontDestroyOnLoad(gameObject);
    }

    // -------- BASIC EVENTS --------

    public void DeathTap()
    {
        if (!enableHaptics) return;
        StartHapticPattern(DeathPattern());
    }

    public void FlipTap()
    {
        if (!enableHaptics) return;
        StartHapticPattern(FlipPattern());
    }

    public void ShieldPickup()
    {
        if (!enableHaptics) return;
        StartHapticPattern(ShieldPattern());
    }

    public void SlowTimePickup()
    {
        if (!enableHaptics) return;
        StartHapticPattern(SlowTimePattern());
    }

    public void SpeedBoostPickup()
    {
        if (!enableHaptics) return;
        StartHapticPattern(SpeedBoostPattern());
    }

    // -------- PATTERN ENGINE --------

    private void StartHapticPattern(IEnumerator pattern)
    {
        if (isPlayingPattern) return;
        StartCoroutine(PlayPattern(pattern));
    }

    private IEnumerator PlayPattern(IEnumerator pattern)
    {
        isPlayingPattern = true;
        yield return StartCoroutine(pattern);
        isPlayingPattern = false;
    }

    // -------- PATTERNS --------

    private IEnumerator DeathPattern()
    {
        Handheld.Vibrate();
        yield return new WaitForSecondsRealtime(mediumGap);
        Handheld.Vibrate();
    }

    private IEnumerator FlipPattern()
    {
        Handheld.Vibrate();
        yield return new WaitForSecondsRealtime(0.025f);
        Handheld.Vibrate();
    }

    private IEnumerator ShieldPattern()
    {
        Handheld.Vibrate();
        yield return new WaitForSecondsRealtime(0.025f);
        Handheld.Vibrate();
    }

    private IEnumerator SlowTimePattern()
    {
        Handheld.Vibrate();
        yield return new WaitForSecondsRealtime(lightGap);
        Handheld.Vibrate();
    }

    private IEnumerator SpeedBoostPattern()
    {
        Handheld.Vibrate();
        yield return new WaitForSecondsRealtime(0.02f);
        Handheld.Vibrate();
        yield return new WaitForSecondsRealtime(0.02f);
        Handheld.Vibrate();
    }
}