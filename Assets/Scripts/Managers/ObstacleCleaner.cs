using UnityEngine;

public static class ObstacleCleaner
{
    public static void ClearAll()
    {
        Obstacle[] obstacles = Object.FindObjectsByType<Obstacle>(FindObjectsSortMode.None);
        foreach (var o in obstacles)
            Object.Destroy(o.gameObject);
    }
}
