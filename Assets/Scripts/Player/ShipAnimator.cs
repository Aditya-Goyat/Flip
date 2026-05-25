using UnityEngine;

public class ShipAnimator : MonoBehaviour
{
    [Header("Tilt Settings (Z-Axis Lean)")]
    [Tooltip("How far the ship leans left/right when moving.")]
    public float tiltAngle = 25f;
    public float tiltSpeed = 12f;

    [Header("References")]
    [Tooltip("CRITICAL: Drag the child object holding your Sprite here! DO NOT put the main Player here.")]
    public Transform shipVisuals;

    private float lastXPos;

    void Start()
    {
        lastXPos = transform.position.x;

        // Safety check to warn you if it's set up wrong
        if (shipVisuals == null || shipVisuals == this.transform)
        {
            Debug.LogError("SHIP ANIMATOR ERROR: You must assign a CHILD object to 'Ship Visuals' in the inspector, otherwise the whole hitbox will spin!");
            shipVisuals = this.transform; // Fallback
        }
    }

    void Update()
    {
        // 1. Calculate movement to figure out which way we are going
        float deltaX = transform.position.x - lastXPos;

        int moveDir = 0;
        if (deltaX > 0.02f) moveDir = 1;        // Moving Right
        else if (deltaX < -0.02f) moveDir = -1;   // Moving Left

        lastXPos = transform.position.x;

        // 2. Smooth Z-Axis Tilt (Banking)
        float targetZ = -moveDir * tiltAngle;

        // Using localRotation so it doesn't mess with world physics
        Quaternion targetRot = Quaternion.Euler(0, 0, targetZ);
        shipVisuals.localRotation = Quaternion.Lerp(shipVisuals.localRotation, targetRot, Time.deltaTime * tiltSpeed);
    }
}