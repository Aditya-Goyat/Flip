using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Components")]
    [SerializeField] private AudioMixer audioMixer;

    [Header("Audio Sources")]
    [Tooltip("Audio Source for looping background music")]
    [SerializeField] private AudioSource bgmSource;
    [Tooltip("Audio Source for general sound effects (UI clicks, etc)")]
    [SerializeField] private AudioSource sfxSource;

    private const string MIXER_MASTER = "MasterVolume";

    void Awake()
    {
        // Singleton Pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Load saved volume and mute state
        float savedVol = PlayerPrefs.GetFloat("MasterVolume", 0.75f);
        bool isMuted = PlayerPrefs.GetInt("IsMuted", 0) == 1;

        if (isMuted) audioMixer.SetFloat(MIXER_MASTER, -80f);
        else SetMasterVolume(savedVol);
    }

    // --- VOLUME CONTROL ---

    public void SetMasterVolume(float sliderValue)
    {
        PlayerPrefs.SetFloat("MasterVolume", sliderValue);
        if (PlayerPrefs.GetInt("IsMuted", 0) == 1) return;

        float dbVolume = Mathf.Log10(Mathf.Clamp(sliderValue, 0.0001f, 1f)) * 20;
        audioMixer.SetFloat(MIXER_MASTER, dbVolume);
    }

    public void SetMute(bool isMuted)
    {
        PlayerPrefs.SetInt("IsMuted", isMuted ? 1 : 0);

        if (isMuted) audioMixer.SetFloat(MIXER_MASTER, -80f);
        else SetMasterVolume(GetSavedVolume());
    }

    public float GetSavedVolume() => PlayerPrefs.GetFloat("MasterVolume", 0.75f);
    public bool IsMuted() => PlayerPrefs.GetInt("IsMuted", 0) == 1;

    // --- NEW: PLAYBACK METHODS ---

    /// <summary>
    /// Plays background music. Automatically loops it.
    /// </summary>
    public void PlayBGM(AudioClip clip)
    {
        if (bgmSource != null && clip != null)
        {
            bgmSource.clip = clip;
            bgmSource.loop = true;
            bgmSource.Play();
        }
    }

    /// <summary>
    /// Plays a sound effect once. Call this for UI clicks, obstacle passes, etc.
    /// </summary>
    public void PlaySFX(AudioClip clip)
    {
        if (sfxSource != null && clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }
}