using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float fallbackMoveSpeed = 8f;

    [Header("References")]
    [SerializeField] private DifficultyManager difficultyManager;

    [Header("Edge Padding")]
    [Tooltip("Small padding so the player sprite doesn't go half off screen.")]
    [SerializeField] private float edgePadding = 0.2f;

    private float direction;
    private float currentVelocity;
    private bool isDead;

    private float leftLimit;
    private float rightLimit;

    public static PlayerController Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (difficultyManager == null)
        {
            difficultyManager = FindFirstObjectByType<DifficultyManager>();
        }

        CalculateScreenBounds();
    }

    private void CalculateScreenBounds()
    {
        Camera cam = Camera.main;

        Vector3 left = cam.ScreenToWorldPoint(new Vector3(0, 0, cam.nearClipPlane));
        Vector3 right = cam.ScreenToWorldPoint(new Vector3(Screen.width, 0, cam.nearClipPlane));

        leftLimit = left.x + edgePadding;
        rightLimit = right.x - edgePadding;
    }

    private void Update()
    {
        if (isDead) return;

        direction = 0f;

        if (Touchscreen.current != null &&
            Touchscreen.current.primaryTouch.press.isPressed)
        {
            float x = Touchscreen.current.primaryTouch.position.ReadValue().x;
            direction = x < Screen.width / 2f ? -1f : 1f;
        }
        else if (Mouse.current != null &&
                 Mouse.current.leftButton.isPressed)
        {
            float x = Mouse.current.position.ReadValue().x;
            direction = x < Screen.width / 2f ? -1f : 1f;
        }

        if (FlipManager.IsInverted)
            direction *= -1f;

        float currentMoveSpeed = fallbackMoveSpeed;

        if (difficultyManager != null)
        {
            currentMoveSpeed = difficultyManager.CurrentPlayerMoveSpeed;
        }

        if (PowerupManager.Instance != null)
        {
            currentMoveSpeed *= PowerupManager.Instance.PlayerSpeedMultiplier;
        }

        Vector3 pos = transform.position;

        float targetVelocity = direction * currentMoveSpeed;
        currentVelocity = Mathf.Lerp(currentVelocity, targetVelocity, 12f * Time.deltaTime);
        pos.x += currentVelocity * Time.deltaTime;

        pos.x = Mathf.Clamp(pos.x, leftLimit, rightLimit);

        transform.position = pos;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead) return;

        if (collision.gameObject.CompareTag("Obstacle"))
        {
            if (PowerupManager.Instance != null && PowerupManager.Instance.ConsumeShield())
            {
                Destroy(collision.gameObject);
                return;
            }

            isDead = true;
            Die();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Powerup"))
        {
            Powerup powerup = other.GetComponent<Powerup>();
            if (powerup != null && PowerupManager.Instance != null)
            {
                PowerupManager.Instance.CollectPowerup(powerup.GetPowerupType());
                Destroy(other.gameObject);
            }
        }
    }

    private void Die()
    {
        if (Haptics_Manager.Instance != null)
        {
            Haptics_Manager.Instance.DeathTap();
        }

        GameManager.Instance.OnPlayerDeath();
    }

    public void Revive()
    {
        isDead = false;
        currentVelocity = 0f;
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