using System.Collections;
using UnityEngine;

public class PowerupManager : MonoBehaviour
{
    public static PowerupManager Instance;

    [Header("Durations")]
    [SerializeField] private float shieldDuration = 6f;
    [SerializeField] private float slowTimeDuration = 5f;
    [SerializeField] private float speedBoostDuration = 5f;

    [Header("Slow Time")]
    [SerializeField] private float slowTimeMultiplier = 0.6f;

    [Header("Speed Boost")]
    [SerializeField] private float speedBoostMultiplier = 1.4f;

    public bool HasShield { get; private set; }

    private Coroutine shieldRoutine;
    private Coroutine slowTimeRoutine;
    private Coroutine speedBoostRoutine;

    private float obstacleSpeedMultiplier = 1f;
    private float playerSpeedMultiplier = 1f;

    public float ObstacleSpeedMultiplier => obstacleSpeedMultiplier;
    public float PlayerSpeedMultiplier => playerSpeedMultiplier;

    private void Awake()
    {
        Instance = this;
    }

    public void CollectPowerup(PowerupType type)
    {
        switch (type)
        {
            case PowerupType.Shield:
                /*if (Haptics_Manager.Instance != null)
                {
                    Haptics_Manager.Instance.ShieldPickup();
                }*/

                if (shieldRoutine != null) StopCoroutine(shieldRoutine);
                shieldRoutine = StartCoroutine(ShieldRoutine());
                break;

            case PowerupType.SlowTime:
                /*if (Haptics_Manager.Instance != null)
                {
                    Haptics_Manager.Instance.SlowTimePickup();
                }*/

                if (slowTimeRoutine != null) StopCoroutine(slowTimeRoutine);
                slowTimeRoutine = StartCoroutine(SlowTimeRoutine());
                break;

            case PowerupType.SpeedBoost:
                /*if (Haptics_Manager.Instance != null)
                {
                    Haptics_Manager.Instance.SpeedBoostPickup();
                }*/

                if (speedBoostRoutine != null) StopCoroutine(speedBoostRoutine);
                speedBoostRoutine = StartCoroutine(SpeedBoostRoutine());
                break;
        }
    }

    public bool ConsumeShield()
    {
        if (!HasShield) return false;

        HasShield = false;

        if (shieldRoutine != null)
        {
            StopCoroutine(shieldRoutine);
            shieldRoutine = null;
        }

        return true;
    }

    private IEnumerator ShieldRoutine()
    {
        HasShield = true;
        yield return new WaitForSeconds(shieldDuration);
        HasShield = false;
        shieldRoutine = null;
    }

    private IEnumerator SlowTimeRoutine()
    {
        obstacleSpeedMultiplier = slowTimeMultiplier;
        yield return new WaitForSeconds(slowTimeDuration);
        obstacleSpeedMultiplier = 1f;
        slowTimeRoutine = null;
    }

    private IEnumerator SpeedBoostRoutine()
    {
        playerSpeedMultiplier = speedBoostMultiplier;
        yield return new WaitForSeconds(speedBoostDuration);
        playerSpeedMultiplier = 1f;
        speedBoostRoutine = null;
    }
}