using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
    [Header("Time")]
    [SerializeField] private float rampStartTime = 5f;
    [SerializeField] private float timeToMaxDifficulty = 80f;

    [Header("Global Difficulty")]
    [SerializeField]
    private AnimationCurve difficultyCurve =
        new AnimationCurve(
            new Keyframe(0f, 0f),
            new Keyframe(0.25f, 0.08f),
            new Keyframe(0.5f, 0.35f),
            new Keyframe(0.75f, 0.72f),
            new Keyframe(1f, 1f)
        );

    [Header("Obstacle Speed")]
    [SerializeField] private float baseObstacleSpeed = 4f;
    [SerializeField] private float maxObstacleSpeed = 10f;
    [SerializeField] private AnimationCurve obstacleSpeedCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    [Header("Spawn Interval")]
    [SerializeField] private float baseSpawnInterval = 1.15f;
    [SerializeField] private float minSpawnInterval = 0.42f;
    [SerializeField] private AnimationCurve spawnIntervalCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    [Header("Player Speed")]
    [SerializeField] private float basePlayerMoveSpeed = 8f;
    [SerializeField] private float maxPlayerMoveSpeed = 10f;
    [SerializeField]
    private AnimationCurve playerSpeedCurve =
        new AnimationCurve(
            new Keyframe(0f, 0f),
            new Keyframe(0.4f, 0.35f),
            new Keyframe(0.75f, 0.75f),
            new Keyframe(1f, 1f)
        );

    [Header("Optional Pulse")]
    [SerializeField] private bool enablePulse = true;
    [SerializeField] private float pulseStrength = 0.08f;
    [SerializeField] private float pulseFrequency = 0.18f;

    public float Difficulty01 { get; private set; }
    public float CurrentObstacleSpeed { get; private set; }
    public float CurrentSpawnInterval { get; private set; }
    public float CurrentPlayerMoveSpeed { get; private set; }

    private void Update()
    {
        float elapsed = Mathf.Max(0f, Time.timeSinceLevelLoad - rampStartTime);

        float rawDifficulty = timeToMaxDifficulty > 0f
            ? Mathf.Clamp01(elapsed / timeToMaxDifficulty)
            : 1f;

        Difficulty01 = Mathf.Clamp01(difficultyCurve.Evaluate(rawDifficulty));

        float pulse = 0f;
        if (enablePulse)
        {
            pulse = Mathf.Sin(Time.timeSinceLevelLoad * pulseFrequency * Mathf.PI * 2f) * pulseStrength;
        }

        float obstacleDifficulty = Mathf.Clamp01(Difficulty01 + pulse);
        float spawnDifficulty = Mathf.Clamp01(Difficulty01 + pulse * 0.75f);
        float playerDifficulty = Difficulty01;

        CurrentObstacleSpeed = Mathf.Lerp(
            baseObstacleSpeed,
            maxObstacleSpeed,
            obstacleSpeedCurve.Evaluate(obstacleDifficulty)
        );

        CurrentSpawnInterval = Mathf.Lerp(
            baseSpawnInterval,
            minSpawnInterval,
            spawnIntervalCurve.Evaluate(spawnDifficulty)
        );

        CurrentPlayerMoveSpeed = Mathf.Lerp(
            basePlayerMoveSpeed,
            maxPlayerMoveSpeed,
            playerSpeedCurve.Evaluate(playerDifficulty)
        );
    }
}