using UnityEngine;
using System.Collections;

public class Camerashake : MonoBehaviour
{
    public static Camerashake Instance;

    private Camera cam;
    private Vector3 originalPos;

    [Header("Boost Zoom Settings")]
    [Tooltip("Target size/FOV when moving normally.")]
    public float normalSize = 5f;
    [Tooltip("Target size/FOV when Surge/Boost is active.")]
    public float boostSize = 6.5f;
    [Tooltip("How fast the camera shifts between normal and boost views.")]
    public float transitionSpeed = 5f;

    private float currentTargetSize;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        cam = GetComponent<Camera>();

        // Store the starting position of the camera so it knows where to return to
        originalPos = transform.localPosition;

        // Initialize target size to whatever your baseline view is set to
        currentTargetSize = normalSize;
    }

    void Update()
    {
        // Smoothly glide toward our target camera zoom factor using Lerp
        if (cam.orthographic)
        {
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, currentTargetSize, Time.unscaledDeltaTime * transitionSpeed);
        }
        else
        {
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, currentTargetSize, Time.unscaledDeltaTime * transitionSpeed);
        }
    }

    // Call this from your Surge script when boost activates
    public void TriggerBoostZoom()
    {
        currentTargetSize = boostSize;
    }

    // Call this from your Surge script when boost runs out
    public void ResetZoom()
    {
        currentTargetSize = normalSize;
    }

    /// <summary>
    /// Call this to shake the camera. 
    /// Example: Camerashake.Instance.Shake(0.3f, 0.5f);
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

        // Define how far the camera is allowed to drift from center
        float maxOffset = 0.15f; // Tweak this — keep it small

        while (elapsed < duration)
        {
            float x = Mathf.Clamp(Random.Range(-1f, 1f) * magnitude, -maxOffset, maxOffset);
            float y = Mathf.Clamp(Random.Range(-1f, 1f) * magnitude, -maxOffset, maxOffset);

            transform.localPosition = new Vector3(originalPos.x + x, originalPos.y + y, originalPos.z);

            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        transform.localPosition = originalPos;
    }
}