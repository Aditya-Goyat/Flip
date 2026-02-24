using UnityEngine;

public class HoverEffect : MonoBehaviour
{
    [Header("Hover Settings")]
    [Tooltip("How far up and down the ship bobs")]
    public float hoverAmount = 0.15f;

    [Tooltip("How fast the ship bobs")]
    public float hoverSpeed = 3f;

    private float startY;

    void Start()
    {
        // Remember the starting Y position so we hover around it
        startY = transform.position.y;
    }

    void Update()
    {
        // Calculate the new Y position using a smooth Sine wave
        float newY = startY + (Mathf.Sin(Time.time * hoverSpeed) * hoverAmount);

        // Apply it while preserving the X (controlled by PlayerController) and Z
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
}