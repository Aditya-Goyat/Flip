using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Graphic))]
public class UIPulser : MonoBehaviour
{
    [Header("Pulse Settings")]
    [Tooltip("How fast it fades in and out")]
    [SerializeField] private float pulseSpeed = 3f;

    [Tooltip("The lowest transparency (0 is invisible, 1 is solid)")]
    [SerializeField][Range(0f, 1f)] private float minAlpha = 0.3f;

    [Tooltip("The highest transparency")]
    [SerializeField][Range(0f, 1f)] private float maxAlpha = 1.0f;

    private Graphic uiElement; // Works for Images or Text

    private void Awake()
    {
        // Automatically grab the Image component on this object
        uiElement = GetComponent<Graphic>();
    }

    private void Update()
    {
        if (uiElement != null)
        {
            Color currentColor = uiElement.color;
            // Mathf.Sin creates a smooth wave between -1 and 1. We normalize it to 0 to 1.
            float wave = (Mathf.Sin(Time.unscaledTime * pulseSpeed) + 1f) / 2f;

            // Apply the wave to the Alpha (transparency)
            currentColor.a = Mathf.Lerp(minAlpha, maxAlpha, wave);
            uiElement.color = currentColor;
        }
    }
}