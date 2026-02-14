using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Components")]
    [SerializeField] private AudioMixer audioMixer;

    // The exact name you gave the Exposed Parameter in Step 1
    private const string MIXER_MASTER = "MasterVolume";

    void Awake()
    {
        // Singleton Pattern: Ensure only one AudioManager exists
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate if we reload the menu
        }
    }

    void Start()
    {
        // Load saved volume and mute state
        float savedVol = PlayerPrefs.GetFloat("MasterVolume", 0.75f);
        bool isMuted = PlayerPrefs.GetInt("IsMuted", 0) == 1;

        // Apply initial settings
        if (isMuted)
        {
            audioMixer.SetFloat(MIXER_MASTER, -80f);
        }
        else
        {
            SetMasterVolume(savedVol);
        }
    }

    public void SetMasterVolume(float sliderValue)
    {
        // Save the setting so it remembers next time
        PlayerPrefs.SetFloat("MasterVolume", sliderValue);

        // If we are currently muted, do NOT update the mixer, just save the value for later
        if (PlayerPrefs.GetInt("IsMuted", 0) == 1) return;

        float dbVolume = Mathf.Log10(Mathf.Clamp(sliderValue, 0.0001f, 1f)) * 20;
        audioMixer.SetFloat(MIXER_MASTER, dbVolume);
    }

    public void SetMute(bool isMuted)
    {
        PlayerPrefs.SetInt("IsMuted", isMuted ? 1 : 0);

        if (isMuted)
        {
            audioMixer.SetFloat(MIXER_MASTER, -80f); // Mute
        }
        else
        {
            // Unmute: Restore the saved slider value
            float savedVol = GetSavedVolume();
            SetMasterVolume(savedVol);
        }
    }

    public float GetSavedVolume()
    {
        return PlayerPrefs.GetFloat("MasterVolume", 0.75f);
    }

    public bool IsMuted()
    {
        return PlayerPrefs.GetInt("IsMuted", 0) == 1;
    }
}
