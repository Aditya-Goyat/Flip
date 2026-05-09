using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerController : MonoBehaviour
{

    [Header("Movement")]
    [SerializeField] private float fallbackMoveSpeed = 8f;

    [Header("References")]
    [SerializeField] private DifficultyManager difficultyManager;

    [Header("Edge Padding")]
    [Tooltip("Small padding so the player sprite doesn't go half off screen.")]
    [SerializeField] private float edgePadding = 0.2f;

    [Header("Double-Tap Settings")]
    [Tooltip("Maximum seconds between two taps to count as a double-tap.")]
    [SerializeField] private float doubleTapWindow = 0.3f;

    [Tooltip("Maximum seconds a press can last and still be considered a 'tap' " +
             "(prevents a slow hold from accidentally triggering the surge).")]
    [SerializeField] private float maxTapDuration = 0.18f;

    [Header("Surge Visuals")]
    [SerializeField] private GameObject rushingVfxObject; // Drag your rushing VFX here
    [SerializeField] private GameObject shipVfxObject;    // <--- NEW: Drag your Ship FX here!
    [SerializeField] private float normalYPosition = -3.5f;
    [SerializeField] private float surgeYPosition = -1.5f;
    [SerializeField] private float surgeLerpSpeed = 5f;


    private float direction;
    private float currentVelocity;
    private bool isDead;

    private float leftLimit;
    private float rightLimit;

    // --- Double-tap tracking (completely separate from movement) ---
    private float lastTapTime = -999f;   // time of the most recent valid tap
    private float pressStartTime = -999f;   // when the current press began
    private bool pressActive = false;   // is a press currently held?


    public static PlayerController Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (difficultyManager == null)
            difficultyManager = FindFirstObjectByType<DifficultyManager>();

        CalculateScreenBounds();
    }

    private void Update()
    {
        if (isDead) return;

        HandleMovement();
        HandleDoubleTap();
    }

    private void HandleMovement()
    {
        // --- NEW: Check if we are currently surging ---
        bool isSurging = SurgeManager.Instance != null && SurgeManager.Instance.IsSurging;

        // Toggle the rushing VFX and the new Ship FX
        if (rushingVfxObject != null)
        {
            rushingVfxObject.SetActive(isSurging);
        }

        if (shipVfxObject != null)
        {
            shipVfxObject.SetActive(isSurging);
        }

        direction = 0f;

        // ── Touch ────────────────────────────────────────────────────────────
        if (Touchscreen.current != null &&
            Touchscreen.current.primaryTouch.press.isPressed)
        {
            float x = Touchscreen.current.primaryTouch.position.ReadValue().x;
            direction = x < Screen.width * 0.5f ? -1f : 1f;
        }
        // ── Mouse (editor / desktop fallback) ────────────────────────────────
        else if (Mouse.current != null &&
                 Mouse.current.leftButton.isPressed)
        {
            float x = Mouse.current.position.ReadValue().x;
            direction = x < Screen.width * 0.5f ? -1f : 1f;
        }

        // Respect screen-flip modifier if present
        if (FlipManager.IsInverted)
            direction *= -1f;

        // Resolve move speed from difficulty, then apply powerup multiplier
        float speed = difficultyManager != null
            ? difficultyManager.CurrentPlayerMoveSpeed
            : fallbackMoveSpeed;

        // ── Powerup speed modifier – commented out for Pure Skill mode ────────
        // if (PowerupManager.Instance != null)
        //     speed *= PowerupManager.Instance.PlayerSpeedMultiplier;

        // Smooth acceleration / deceleration
        float targetVelocity = direction * speed;
        currentVelocity = Mathf.Lerp(currentVelocity, targetVelocity, 12f * Time.deltaTime);

        Vector3 pos = transform.position;
        pos.x += currentVelocity * Time.deltaTime;
        pos.x = Mathf.Clamp(pos.x, leftLimit, rightLimit);

        // --- NEW: Y-Axis Forward/Backward Slide ---
        float targetY = isSurging ? surgeYPosition : normalYPosition;
        pos.y = Mathf.Lerp(pos.y, targetY, surgeLerpSpeed * Time.deltaTime);

        transform.position = pos;
    }

    private void HandleDoubleTap()
    {
        bool pressedThisFrame = WasPressedThisFrame();
        bool releasedThisFrame = WasReleasedThisFrame();

        if (pressedThisFrame && !pressActive)
        {
            pressActive = true;
            pressStartTime = Time.unscaledTime;   // unscaled so pause doesn't break it
        }

        if (releasedThisFrame && pressActive)
        {
            pressActive = false;

            float pressDuration = Time.unscaledTime - pressStartTime;

            if (pressDuration <= maxTapDuration)
            {
                // Valid tap – check for double-tap
                float timeSinceLast = Time.unscaledTime - lastTapTime;

                if (timeSinceLast <= doubleTapWindow)
                {
                    // ✅ Double-tap confirmed
                    SurgeManager.Instance?.TryActivateSurge();
                    lastTapTime = -999f; // Reset so a third tap doesn't re-fire
                }
                else
                {
                    // First tap of a potential pair – record the time
                    lastTapTime = Time.unscaledTime;
                }
            }
            else
            {
                // Long hold → not a tap, clear first-tap memory
                lastTapTime = -999f;
            }
        }
    }

    // ── Input helpers (returns true only on the frame the state changed) ─────

    private bool WasPressedThisFrame()
    {
        if (Touchscreen.current != null &&
            Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
            return true;

        if (Mouse.current != null &&
            Mouse.current.leftButton.wasPressedThisFrame)
            return true;

        return false;
    }

    private bool WasReleasedThisFrame()
    {
        if (Touchscreen.current != null &&
            Touchscreen.current.primaryTouch.press.wasReleasedThisFrame)
            return true;

        if (Mouse.current != null &&
            Mouse.current.leftButton.wasReleasedThisFrame)
            return true;

        return false;
    }


    private void CalculateScreenBounds()
    {
        Camera cam = Camera.main;
        Vector3 left = cam.ScreenToWorldPoint(new Vector3(0, 0, cam.nearClipPlane));
        Vector3 right = cam.ScreenToWorldPoint(new Vector3(Screen.width, 0, cam.nearClipPlane));
        leftLimit = left.x + edgePadding;
        rightLimit = right.x - edgePadding;
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead) return;

        if (!collision.gameObject.CompareTag("Obstacle")) return;

        if (SurgeManager.Instance != null && SurgeManager.Instance.IsSurging)
        {
            // ── Bulldozer Mode
            // Obstacle is destroyed; player keeps moving unharmed.
            Destroy(collision.gameObject);

            // Trigger the explosion we just added to the VFXManager!
            if (VFXManager.Instance != null)
            {
                VFXManager.Instance.PlayExplosion(collision.transform.position);
            }
        }
        else
        {
            // ── Normal Mode
            isDead = true;
            Die();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // ── Powerup collection – commented out for Pure Skill mode ────────────
        // if (other.CompareTag("Powerup"))
        // {
        //     Powerup powerup = other.GetComponent<Powerup>();
        //     if (powerup != null && PowerupManager.Instance != null)
        //     {
        //         PowerupManager.Instance.CollectPowerup(powerup.GetPowerupType());
        //         Destroy(other.gameObject);
        //     }
        // }
    }

    private void Die()
    {
        if (Haptics_Manager.Instance != null)
            Haptics_Manager.Instance.DeathTap();

        GameManager.Instance.OnPlayerDeath();
    }

    public void Revive()
    {
        isDead = false;
        currentVelocity = 0f;
        pressActive = false;        // clear any dangling press state
        lastTapTime = -999f;
        transform.position = new Vector3(0f, -3.5f, 0f);
        StartCoroutine(Invulnerability());
    }

    private System.Collections.IEnumerator Invulnerability()
    {
        Physics2D.IgnoreLayerCollision(
            LayerMask.NameToLayer("Player"),
            LayerMask.NameToLayer("Obstacle"),
            true
        );

        yield return new WaitForSeconds(1f);

        Physics2D.IgnoreLayerCollision(
            LayerMask.NameToLayer("Player"),
            LayerMask.NameToLayer("Obstacle"),
            false
        );
    }
}