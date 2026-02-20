using UnityEngine;
using System.Collections;

public class Camerashake : MonoBehaviour
{
    public static Camerashake Instance;

    private Vector3 originalPos;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // Store the starting position of the camera so it knows where to return to
        originalPos = transform.localPosition;
    }

    /// <summary>
    /// Call this to shake the camera. 
    /// Example: CameraShake.Instance.Shake(0.3f, 0.5f);
    /// </summary>
    public void Shake(float duration, float magnitude)
    {
        // Stop any active shakes before starting a new one
        StopAllCoroutines();
        StartCoroutine(ShakeRoutine(duration, magnitude));
    }

    private IEnumerator ShakeRoutine(float duration, float magnitude)
    {
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            // Generate random X and Y offsets
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = new Vector3(originalPos.x + x, originalPos.y + y, originalPos.z);

            // CRITICAL: unscaledDeltaTime allows shaking even when TimeScale is 0
            elapsed += Time.unscaledDeltaTime;

            yield return null;
        }

        // Snap back to the exact center when done
        transform.localPosition = originalPos;
    }
}
