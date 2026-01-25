using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float moveSpeed = 8f;
    float direction;
    bool isDead;
    public static PlayerController Instance;


    void Awake()
    {
        Instance = this;
    }


    void Update()
    {
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

        Vector3 pos = transform.position;
        pos.x += direction * moveSpeed * Time.deltaTime;
        pos.x = Mathf.Clamp(pos.x, -2.5f, 2.5f);
        transform.position = pos;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead) return;

        if (collision.gameObject.CompareTag("Obstacle"))
        {
            isDead = true;
            Die();
        }
    }


    void Die()
    {
        GameManager.Instance.OnPlayerDeath();
    }

    public void Revive()
    {
        isDead = false;

        // Safe respawn position
        transform.position = new Vector3(0f, -3.5f, 0f);

        // Brief invulnerability
        StartCoroutine(Invulnerability());
    }

    System.Collections.IEnumerator Invulnerability()
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
