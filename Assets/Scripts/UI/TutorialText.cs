using UnityEngine;
using TMPro;
using System.Collections;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TutorialText : MonoBehaviour
{
    [Header("Timing Settings")]
    [Tooltip("How long the text stays fully visible")]
    [SerializeField] private float displayTime = 3f;
    [Tooltip("How long it takes to fade out to invisible")]
    [SerializeField] private float fadeTime = 1f;

    [Header("Hover Animation")]
    [Tooltip("How fast the text bobs up and down")]
    [SerializeField] private float hoverSpeed = 2f;
    [Tooltip("How far the text moves up and down (in pixels)")]
    [SerializeField] private float hoverAmount = 10f;

    private TextMeshProUGUI textMesh;
    private RectTransform rectTransform;
    private Vector2 originalPosition;

    private void Awake()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
        rectTransform = GetComponent<RectTransform>();
        originalPosition = rectTransform.anchoredPosition;
    }

    private void Start()
    {
        // Ensure the text starts fully opaque
        Color c = textMesh.color;
        c.a = 1f;
        textMesh.color = c;

        // Start the fade countdown
        StartCoroutine(DisplayAndFadeRoutine());
    }

    private void Update()
    {
        // Subtle hover effect using a Sine wave
        float newY = originalPosition.y + Mathf.Sin(Time.time * hoverSpeed) * hoverAmount;
        rectTransform.anchoredPosition = new Vector2(originalPosition.x, newY);
    }

    private IEnumerator DisplayAndFadeRoutine()
    {
        // 1. Wait for the display duration
        yield return new WaitForSeconds(displayTime);

        // 2. Fade out over time
        float elapsedTime = 0f;
        Color originalColor = textMesh.color;

        while (elapsedTime < fadeTime)
        {
            elapsedTime += Time.deltaTime;

            // Lerp gradually shifts the alpha from 1 (solid) to 0 (invisible)
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeTime);
            textMesh.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);

            yield return null; // Wait for the next frame
        }

        // 3. Disable the object once it is fully transparent to save performance
        gameObject.SetActive(false);
    }
}