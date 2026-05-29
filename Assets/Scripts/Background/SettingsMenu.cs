using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Toggle soundToggle;
    [SerializeField] private Toggle hapticsToggle;

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
        volumeSlider.value = AudioManager.Instance.GetSavedVolume();
        volumeSlider.onValueChanged.AddListener(OnVolumeSliderChanged);
    }

    public void OnVolumeSliderChanged(float value)
    {
        if (AudioManager.Instance != null) AudioManager.Instance.SetMasterVolume(value);
    }

    // --- 2. SOUND TOGGLE ---
    private void InitializeSound()
    {
        if (AudioManager.Instance == null) return;

        // Sound ON = Not Muted
        bool isSoundOn = !AudioManager.Instance.IsMuted();
        soundToggle.isOn = isSoundOn;

        soundToggle.onValueChanged.AddListener(OnSoundToggleChanged);
    }

    private void OnSoundToggleChanged(bool isOn)
    {
        AudioManager.Instance.SetMute(!isOn);
    }

    // --- 3. HAPTICS TOGGLE ---
    private void InitializeHaptics()
    {
        bool isHapticsOn = HapticsManager.IsHapticsEnabled();
        hapticsToggle.isOn = isHapticsOn;

        hapticsToggle.onValueChanged.AddListener(OnHapticsToggleChanged);
    }

    private void OnHapticsToggleChanged(bool isOn)
    {
        HapticsManager.SetHaptics(isOn);
    }
}