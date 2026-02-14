using UnityEngine;
using UnityEngine.UI;

public class VolumeSettings : MonoBehaviour
{
    [SerializeField] private Slider volumeSlider;

    void Start()
    {
        // 1. Get the current volume from the Manager
        // We use a check in case you started directly in the Settings scene for testing
        if (AudioManager.Instance != null)
        {
            float currentVol = AudioManager.Instance.GetSavedVolume();

            // 2. Set the slider visual position
            volumeSlider.value = currentVol;

            // 3. Add the listener to detect changes
            volumeSlider.onValueChanged.AddListener(OnSliderChanged);
        }
        else
        {
            Debug.LogWarning("AudioManager not found! Start from Main Menu.");
        }
    }

    // Called dynamically by the Slider
    public void OnSliderChanged(float value)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetMasterVolume(value);
        }
    }
}