using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float fallbackMoveSpeed = 8f;

    [Header("References")]
    [SerializeField] private DifficultyManager difficultyManager;
    [Tooltip("Drag the child GameObject containing the Ship's SpriteRenderer here.")]
    [SerializeField] private SpriteRenderer shipSpriteRenderer; 

    [Header("Edge Padding")]
    [SerializeField] private float edgePadding = 0.2f;

    [Header("Double-Tap Settings")]
    [SerializeField] private float doubleTapWindow = 0.3f;
    [SerializeField] private float maxTapDuration = 0.18f;

    [Header("Surge Visuals")]
    [SerializeField] private GameObject rushingVfxObject; 
    [SerializeField] private GameObject shipVfxObject;    
    [SerializeField] private float normalYPosition = -3.5f;
    [SerializeField] private float surgeYPosition = -1.5f;
    [SerializeField] private float surgeLerpSpeed = 5f;

    [Header("Ghost Mode Settings")]
    public float invulnerabilityDuration = 2.5f;
    public float blinkSpeed = 0.15f;

    private float direction;
    private float currentVelocity;
    private bool isDead;
    
    // --- THE NEW FORCEFIELD ---
    private bool isGhostMode = false; 

    private float leftLimit;
    private float rightLimit;

    private float lastTapTime = -999f;   
    private float pressStartTime = -999f;   
    private bool pressActive = false;   

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
        bool isSurging = SurgeManager.Instance != null && SurgeManager.Instance.IsSurging;

        if (rushingVfxObject != null) rushingVfxObject.SetActive(isSurging);
        if (shipVfxObject != null) shipVfxObject.SetActive(isSurging);

        direction = 0f;

        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
        {
            float x = Touchscreen.current.primaryTouch.position.ReadValue().x;
            direction = x < Screen.width * 0.5f ? -1f : 1f;
        }
        else if (Mouse.current != null && Mouse.current.leftButton.isPressed)
        {
            float x = Mouse.current.position.ReadValue().x;
            direction = x < Screen.width * 0.5f ? -1f : 1f;
        }

        if (FlipManager.IsInverted) direction *= -1f;

        float speed = difficultyManager != null ? difficultyManager.CurrentPlayerMoveSpeed : fallbackMoveSpeed;

        float targetVelocity = direction * speed;
        currentVelocity = Mathf.Lerp(currentVelocity, targetVelocity, 12f * Time.deltaTime);

        Vector3 pos = transform.position;
        pos.x += currentVelocity * Time.deltaTime;
        pos.x = Mathf.Clamp(pos.x, leftLimit, rightLimit);

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
            pressStartTime = Time.unscaledTime;   
        }

        if (releasedThisFrame && pressActive)
        {
            pressActive = false;
            float pressDuration = Time.unscaledTime - pressStartTime;

            if (pressDuration <= maxTapDuration)
            {
                float timeSinceLast = Time.unscaledTime - lastTapTime;

                if (timeSinceLast <= doubleTapWindow)
                {
                    SurgeManager.Instance?.TryActivateSurge();
                    lastTapTime = -999f; 
                }
                else
                {
                    lastTapTime = Time.unscaledTime;
                }
            }
            else
            {
                lastTapTime = -999f;
            }
        }
    }

    private bool WasPressedThisFrame()
    {
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame) return true;
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame) return true;
        return false;
    }

    private bool WasReleasedThisFrame()
    {
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasReleasedThisFrame) return true;
        if (Mouse.current != null && Mouse.current.leftButton.wasReleasedThisFrame) return true;
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
        // --- 1. BULLETPROOF FIX: If we are a ghost, instantly ignore the collision! ---
        if (isDead || isGhostMode) return; 

        if (!collision.gameObject.CompareTag("Obstacle")) return;

        if (SurgeManager.Instance != null && SurgeManager.Instance.IsSurging)
        {
            Destroy(collision.gameObject);
            if (VFXManager.Instance != null) VFXManager.Instance.PlayExplosion(collision.transform.position);
        }
        else
        {
            isDead = true;
            Die();
        }
    }

    private void Die()
    {
        if (Haptics_Manager.Instance != null) Haptics_Manager.Instance.DeathTap();
        GameManager.Instance.OnPlayerDeath();
    }

    public void Revive()
    {
        isDead = false;
        currentVelocity = 0f;
        pressActive = false;        
        lastTapTime = -999f;
        transform.position = new Vector3(0f, -3.5f, 0f);

        StartCoroutine(Invulnerability());
    }

    // --- 2. THE UPDATED COROUTINE ---
    private System.Collections.IEnumerator Invulnerability()
    {
        // Turn ON the Code-Level Forcefield
        isGhostMode = true;

        float elapsedTime = 0f;

        if (shipSpriteRenderer != null)
        {
            while (elapsedTime < invulnerabilityDuration)
            {
                // Hard-toggle the visual so the HDR shader doesn't block the fade
                shipSpriteRenderer.enabled = !shipSpriteRenderer.enabled;
                yield return new WaitForSeconds(blinkSpeed);
                elapsedTime += blinkSpeed;
            }

            // Force it back on
            shipSpriteRenderer.enabled = true;
        }
        else
        {
            // If the SpriteRenderer wasn't assigned in the inspector, just wait safely
            yield return new WaitForSeconds(invulnerabilityDuration);
        }

        // Turn OFF the Forcefield - player is mortal again
        isGhostMode = false;
    }
}