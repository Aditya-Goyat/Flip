using UnityEngine;

public class GridScroller : MonoBehaviour
{
    public float scrollSpeed = 0.5f;
    private Material mat;
    private Vector2 offset;

    void Start()
    {
        mat = GetComponent<Renderer>().material;
    }

    void Update()
    {
        // Scroll Y axis over time
        float y = Mathf.Repeat(Time.time * scrollSpeed, 1);
        offset = new Vector2(0, y);
        mat.mainTextureOffset = offset;
    }
}
