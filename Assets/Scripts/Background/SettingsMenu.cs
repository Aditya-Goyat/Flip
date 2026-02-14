using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Slider volumeSlider;

    [Header("Sound Toggle")]
    [SerializeField] private Button soundButton;
    [SerializeField] private SpriteToggle soundVisuals; // Drag the button here too

    [Header("Haptics Toggle")]
    [SerializeField] private Button hapticsButton;
    [SerializeField] private SpriteToggle hapticsVisuals; // Drag the button here too

    void Start()
    {
        InitializeVolume();
        InitializeSound();
        InitializeHaptics();
    }

    // --- 1. VOLUME ---
    private void InitializeVolume()
    {
        if (AudioManager.Instance == null) return;
        float currentVol = AudioManager.Instance.GetSavedVolume();
        volumeSlider.value = currentVol;
        volumeSlider.onValueChanged.AddListener(OnVolumeSliderChanged);
    }

    public void OnVolumeSliderChanged(float value)
    {
        if (AudioManager.Instance != null) AudioManager.Instance.SetMasterVolume(value);
    }

    // --- 2. SOUND BUTTON ---
    private void InitializeSound()
    {
        if (AudioManager.Instance == null) return;

        // Get State (Sound ON means NOT Muted)
        bool isSoundOn = !AudioManager.Instance.IsMuted();

        // Update visual sprite
        soundVisuals.SetState(isSoundOn);

        // Listen for clicks
        soundButton.onClick.AddListener(OnSoundClicked);
    }

    private void OnSoundClicked()
    {
        // 1. Toggle Visuals
        soundVisuals.Toggle();

        // 2. Logic: If manager is Muted, we are turning sound ON.
        bool isCurrentlyMuted = AudioManager.Instance.IsMuted();
        AudioManager.Instance.SetMute(!isCurrentlyMuted); // Toggle mute state
    }

    // --- 3. HAPTICS BUTTON ---
    private void InitializeHaptics()
    {
        // Get State
        bool isHapticsOn = HapticsManager.IsHapticsEnabled();

        // Update visual sprite
        hapticsVisuals.SetState(isHapticsOn);

        // Listen for clicks
        hapticsButton.onClick.AddListener(OnHapticsClicked);
    }

    private void OnHapticsClicked()
    {
        // 1. Toggle Visuals
        hapticsVisuals.Toggle();

        // 2. Logic: Get new state from the Visuals (or invert previous)
        // Since we just toggled visuals, let's grab the stored pref or just invert.
        bool newHapticState = !HapticsManager.IsHapticsEnabled();
        HapticsManager.SetHaptics(newHapticState);
    }
}