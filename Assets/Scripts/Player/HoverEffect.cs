using UnityEngine;

public class HoverEffect : MonoBehaviour
{
    [Header("Hover Settings")]
    [Tooltip("How far up and down the ship bobs")]
    public float hoverAmount = 15f; // UI pixels usually require larger values than world units!

    [Tooltip("How fast the ship bobs")]
    public float hoverSpeed = 3f;

    private RectTransform rectTransform;
    private float startY;

    void Start()
    {
        // Grab the RectTransform component safely
        rectTransform = GetComponent<RectTransform>();

        // Remember the starting anchored Y position on the canvas
        startY = rectTransform.anchoredPosition.y;
    }

    void Update()
    {
        if (rectTransform == null) return;

        // Calculate the new Y position using a smooth Sine wave
        float newY = startY + (Mathf.Sin(Time.time * hoverSpeed) * hoverAmount);

        // Apply it safely to the UI anchoredPosition, keeping X intact
        rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, newY);
    }
}