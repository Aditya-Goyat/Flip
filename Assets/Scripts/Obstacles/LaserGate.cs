using UnityEngine;

public class LaserGate : MonoBehaviour
{
    [Header("Gate Settings")]
    [Tooltip("The total width of the safe gap between the walls.")]
    [SerializeField] private float gapSize = 2.5f;

    [Header("Wall References")]
    [SerializeField] private Transform leftWall;
    [SerializeField] private Transform rightWall;

    void Start()
    {
        SetupGate();
    }

    // You can also call this in OnValidate if you want to see the gap adjust in the editor
    void SetupGate()
    {
        if (leftWall == null || rightWall == null)
        {
            Debug.LogWarning("LaserGate: Please assign the Left and Right wall transforms!");
            return;
        }

        // Assuming the walls are scaled to X: 15
        float wallWidth = leftWall.localScale.x;

        // Calculate how far from the center each wall needs to be
        // We move it half the gap size, plus half the wall's own width so the edge aligns perfectly
        float offset = (gapSize / 2f) + (wallWidth / 2f);

        // Position the walls relative to this parent object
        leftWall.localPosition = new Vector3(-offset, 0f, 0f);
        rightWall.localPosition = new Vector3(offset, 0f, 0f);
    }
}