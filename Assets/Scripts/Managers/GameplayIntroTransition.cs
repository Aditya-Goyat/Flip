using System.Collections;
using UnityEngine;

public class GameplayIntroTransition : MonoBehaviour
{
    [Header("Target References")]
    [Tooltip("The actual playable ship object in your gameplay scene.")]
    public Transform gameplayShip;
    [Tooltip("Your background or grid scrolling script. It will be disabled during the intro and enabled on touchdown.")]
    public MonoBehaviour gridScrollScript;

    [Header("Juice Elements")]
    [Tooltip("Cyan dust or impact burst particle system prefab to spawn when the ship lands.")]
    public GameObject landingParticle;
    [Tooltip("Your gameplay ship's active high-speed thruster flame VFX object.")]
    public GameObject boostVFX;

    [Header("Cinematic Space Background")]
    [Tooltip("Assign the CanvasGroup component attached to your Space Background overlay image here.")]
    public CanvasGroup spaceBackground; // Catches the handoff from Main Menu (fades 1 -> 0)

    [Header("Cinematic Coordinates")]
    public Vector3 spawnScale = new Vector3(2.5f, 2.5f, 2.5f);
    public Vector3 targetScale = new Vector3(1f, 1f, 1f);
    public Vector3 spawnPosition = new Vector3(0f, 12f, 0f);
    public Vector3 targetPosition = new Vector3(0f, -3.5f, 0f);

    [Header("Timing Settings")]
    [Tooltip("How long to wait on the starry background before starting the downward drop.")]
    public float waitBeforeDrop = 0.15f;
    [Tooltip("How many seconds the ship takes to drop to its playing position.")]
    public float dropDuration = 1.0f;

    void Start()
    {
        // 1. Freeze the scrolling environment so the level doesn't move ahead of time
        if (gridScrollScript != null)
        {
            gridScrollScript.enabled = false;
        }

        // 2. Start the scene completely masked by the space background
        if (spaceBackground != null)
        {
            spaceBackground.alpha = 1f;
        }

        StartCoroutine(DropIntoGridRoutine());
    }

    private IEnumerator DropIntoGridRoutine()
    {
        // 3. Ignite thruster engines during the entry dive
        if (boostVFX != null)
        {
            boostVFX.SetActive(true);
        }

        gameplayShip.position = spawnPosition;
        gameplayShip.localScale = spawnScale;

        // Pause momentarily on the space background for a dramatic transition pause
        yield return new WaitForSeconds(waitBeforeDrop);

        float elapsed = 0f;
        while (elapsed < dropDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / dropDuration;

            // Weighted smooth-step calculation for a cinematic, physically heavy drop feeling
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            gameplayShip.position = Vector3.Lerp(spawnPosition, targetPosition, smoothT);
            gameplayShip.localScale = Vector3.Lerp(spawnScale, targetScale, smoothT);

            // Crossfade out the space background as the ship approaches its destination
            if (spaceBackground != null)
            {
                spaceBackground.alpha = Mathf.Lerp(1f, 0f, t);
            }

            yield return null;
        }

        // 4. Force exact snapping to avoid floating-point drift
        gameplayShip.position = targetPosition;
        gameplayShip.localScale = targetScale;

        if (spaceBackground != null)
        {
            spaceBackground.alpha = 0f;
        }

        // 5. Cut thrusters and trigger landing thud!
        if (boostVFX != null)
        {
            boostVFX.SetActive(false);
        }

        TriggerLandingJuice();

        // 6. Start the gameplay scrolling grid!
        if (gridScrollScript != null)
        {
            gridScrollScript.enabled = true;
        }
    }

    private void TriggerLandingJuice()
    {
        // Spawns the neon impact cloud on touchdown
        if (landingParticle != null)
        {
            Instantiate(landingParticle, targetPosition, Quaternion.identity);
        }

        // Note: Drag in haptic or screen shake triggers here to sell the impact weight!
    }
}