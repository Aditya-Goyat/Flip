using UnityEngine;

public class Collectible : MonoBehaviour
{
    [Header("Settings")]
    public int value = 1;
    public float fallSpeed = 5f; // Match this to your obstacle speed
    public float rotationSpeed = 150f; // Gives it a cool spinning effect

    [Header("Effects")]
    public AudioClip pickupSound;
    public GameObject pickupVFX; // Optional particle effect

    void Update()
    {
        // Move downwards like the obstacles
        transform.Translate(Vector3.down * fallSpeed * Time.deltaTime, Space.World);

        // Spin to look like a floating 3D object
        transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);

        // Destroy if it goes off screen to save memory
        if (transform.position.y < -10f)
        {
            Destroy(gameObject);
        }
    }

    // This triggers when the player touches it
    void OnTriggerEnter2D(Collider2D other)
    {
        // Ensure your Player GameObject has the tag "Player" in the Inspector!
        if (other.CompareTag("Player"))
        {
            // 1. Add to our bank
            if (CurrencyManager.Instance != null)
            {
                CurrencyManager.Instance.AddCore(value);
            }

            // 2. Play the sound effect
            if (AudioManager.Instance != null && pickupSound != null)
            {
                AudioManager.Instance.PlaySFX(pickupSound);
            }

            // 3. Play visual effect (if assigned)
            if (pickupVFX != null)
            {
                Instantiate(pickupVFX, transform.position, Quaternion.identity);
            }

            // 4. Destroy the collectible
            Destroy(gameObject);
        }
    }
}