using UnityEngine;

public static class ObstacleCleaner
{
    public static void ClearAll()
    {
        Obstacle[] obstacles = Object.FindObjectsOfType<Obstacle>();
        foreach (var o in obstacles)
            Object.Destroy(o.gameObject);
    }
}
