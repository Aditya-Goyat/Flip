using UnityEngine;

public class HapticsManager : MonoBehaviour
{
    // Key for saving preference
    private const string HAPTICS_KEY = "HapticsEnabled";

    public static bool IsHapticsEnabled()
    {
        // Default to 1 (True) if not set
        return PlayerPrefs.GetInt(HAPTICS_KEY, 1) == 1;
    }

    public static void SetHaptics(bool enabled)
    {
        PlayerPrefs.SetInt(HAPTICS_KEY, enabled ? 1 : 0);

        // Optional: Vibrate once quickly to confirm it's ON
        if (enabled) Vibrate(50);
    }

    // Call this method everywhere in your game instead of Handheld.Vibrate()
    public static void Vibrate(long milliseconds = 100)
    {
        if (IsHapticsEnabled())
        {
            // Simple vibration
#if UNITY_ANDROID || UNITY_IOS
            Handheld.Vibrate();
#endif
        }
    }
}
