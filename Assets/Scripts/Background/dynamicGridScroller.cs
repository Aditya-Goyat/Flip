using UnityEngine;

public class dynamicGridScroller : MonoBehaviour
{
    [Header("Speed Settings")]
    public float minSpeed = 0.5f;   // Start speed
    public float maxSpeed = 3.0f;   // Top speed (High speed feel)
    public float accelerationDuration = 60f; // Time in seconds to reach max speed

    [Header("Scroll Direction")]
    [Tooltip("Check this to scroll downward (top to bottom)")]
    public bool scrollDown = true;

    private Renderer rend; // Works for both SpriteRenderer and MeshRenderer
    private Material materialInstance; // Instance to avoid modifying shared material
    private float scrollY;

    void Start()
    {
        // Get the Renderer component
        rend = GetComponent<Renderer>();

        if (rend == null)
        {
            Debug.LogError("DynamicGridScroller: No Renderer component found on " + gameObject.name);
            enabled = false;
            return;
        }

        // Create a material instance to avoid modifying the shared material
        materialInstance = rend.material;

        // Verify the material has a texture
        if (materialInstance.mainTexture == null)
        {
            Debug.LogError("DynamicGridScroller: No texture assigned to material on " + gameObject.name);
            enabled = false;
            return;
        }

        scrollY = 0f;

        Debug.Log("DynamicGridScroller: Initialized successfully on " + gameObject.name);
    }

    void Update()
    {
        if (materialInstance == null) return;

        // 1. Calculate how far along the 'acceleration curve' we are (0 to 1)
        float t = Mathf.Clamp01(Time.timeSinceLevelLoad / accelerationDuration);

        // 2. Interpolate speed based on that time
        float currentSpeed = Mathf.Lerp(minSpeed, maxSpeed, t);

        // 3. Integrate position (Speed * Time)
        // Negative value for downward scroll, positive for upward
        float direction = scrollDown ? -1f : 1f;
        scrollY += currentSpeed * Time.deltaTime * direction;

        // 4. Apply texture offset
        // NOTE: This requires the Texture Import Settings 'Wrap Mode' to be set to 'Repeat'
        materialInstance.mainTextureOffset = new Vector2(0, scrollY);
    }

    void OnDestroy()
    {
        // Clean up the material instance to prevent memory leaks
        if (materialInstance != null)
        {
            Destroy(materialInstance);
        }
    }
}