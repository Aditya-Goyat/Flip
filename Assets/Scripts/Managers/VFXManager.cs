using System.Collections;
using UnityEditor.Rendering.LookDev;
using UnityEngine;

public class VFXManager : MonoBehaviour
{
    public static VFXManager Instance;

    [Header("Prefabs")]
    [Tooltip("Assign the 'ShipDebris' Particle System prefab")]
    [SerializeField] private ParticleSystem shipDebrisPrefab;

    [Header("Colors")]
    [SerializeField] private Color normalColor = new Color(0f, 1f, 1f); // Cyan
    [SerializeField] private Color glitchColor = new Color(1f, 0f, 0.33f); // Red/Magenta
    [SerializeField] private Color playerGlitchColor = new Color(1f, 0.9f, 0.2f); // Yellow

    [Header("Audio")]
    [Tooltip("Assign the explosion/shatter sound effect")]
    [SerializeField] private AudioClip deathSound;
    [Tooltip("Assign an AudioSource component attached to this GameObject")]
    [SerializeField] private AudioSource vfxAudioSource;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // Auto-grab AudioSource if not manually assigned
        if (vfxAudioSource == null) vfxAudioSource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// Starts the death sequence: Explodes ship -> Pauses -> Shows UI
    /// </summary>
    public void TriggerDeathSequence(Vector3 position, bool isGlitchMode)
    {
        StartCoroutine(DeathSequenceRoutine(position, isGlitchMode));
    }

    private IEnumerator DeathSequenceRoutine(Vector3 position, bool isGlitchMode)
    {
        // 1. Determine Color
        Color targetColor = isGlitchMode ? playerGlitchColor : normalColor;

        // 2. Hide the actual player (so it looks like it shattered)
        if (PlayerController.Instance != null)
        {
            PlayerController.Instance.gameObject.SetActive(false);
        }

        // 3. Play Death Sound
        if (deathSound != null && vfxAudioSource != null)
        {
            // PlayOneShot allows it to play fully even if triggered rapidly
            vfxAudioSource.PlayOneShot(deathSound);
        }

        // 4. Spawn the "Debris" Particles
        if (shipDebrisPrefab != null)
        {
            // Force Z to -5 so it physically sits in front of the background/player
            Vector3 spawnPos = new Vector3(position.x, position.y, -5f);

            ParticleSystem p = Instantiate(shipDebrisPrefab, spawnPos, Quaternion.identity);

            // --- CRITICAL FIX: Loop through ALL children ---
            var allParticles = p.GetComponentsInChildren<ParticleSystem>();
            foreach (var ps in allParticles)
            {
                var main = ps.main;
                main.useUnscaledTime = true; // This allows them to move while game is paused
                main.startColor = new ParticleSystem.MinMaxGradient(targetColor);
            }

            var allRenderers = p.GetComponentsInChildren<ParticleSystemRenderer>();
            foreach (var r in allRenderers)
            {
                r.sortingOrder = 100; // Force every part of the effect to the front
            }
            // -----------------------------------------------

            Destroy(p.gameObject, 3.0f);
        }

        // Trigger Camera Shake right after the crash
        if (Camerashake.Instance != null)
        {
            // Shakes for 0.4 seconds with a strength of 0.3
            Camerashake.Instance.Shake(0.4f, 0.3f);
        }

        // 5. "Hit Stop" - Freeze the game logic immediately
        Time.timeScale = 0f;

        // 6. Wait for the explosion to play out (e.g., 1.5 seconds real-time)
        // Since TimeScale is 0, we must use WaitForSecondsRealtime
        yield return new WaitForSecondsRealtime(1.5f);

        // 7. Tell GameManager to show the Death Screen
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ShowDeathScreen();
        }
    }
}