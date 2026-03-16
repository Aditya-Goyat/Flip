using UnityEngine;

public class Powerup : MonoBehaviour
{
    [Header("Powerup Settings")]
    [SerializeField] private PowerupType powerupType;
    [SerializeField] private float minFallSpeed = 2f;
    [SerializeField] private float maxFallSpeed = 5f;
    [SerializeField] private float destroyY = -6f;

    private float currentFallSpeed;

    private void Start()
    {
        currentFallSpeed = Random.Range(minFallSpeed, maxFallSpeed);
    }

    private void Update()
    {
        transform.Translate(Vector2.down * currentFallSpeed * Time.deltaTime, Space.World);

        if (transform.position.y < destroyY)
        {
            Destroy(gameObject);
        }
    }

    public PowerupType GetPowerupType()
    {
        return powerupType;
    }
}