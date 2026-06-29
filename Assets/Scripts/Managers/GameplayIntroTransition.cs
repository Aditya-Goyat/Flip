using System.Collections;
using UnityEngine;

public class GameplayIntroTransition : MonoBehaviour
{
    [Header("References")]
    public Transform gameplayShip;
    public MonoBehaviour gridScrollScript;

    [Header("Positions")]
    public Vector3 spawnPosition = new Vector3(0f, -12f, 0f);  // Below screen
    public Vector3 targetPosition = new Vector3(0f, -3.5f, 0f); // Play position

    [Header("Timing")]
    public float dropDuration = 0.8f;

    void Start()
    {
        if (gameplayShip == null) return;

        if (gridScrollScript != null)
            gridScrollScript.enabled = false;

        gameplayShip.position = spawnPosition;
        StartCoroutine(SlideUp());
    }

    private IEnumerator SlideUp()
    {
        float elapsed = 0f;

        while (elapsed < dropDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / dropDuration);
            gameplayShip.position = Vector3.Lerp(spawnPosition, targetPosition, t);
            yield return null;
        }

        gameplayShip.position = targetPosition;

        if (gridScrollScript != null)
            gridScrollScript.enabled = true;
    }
}