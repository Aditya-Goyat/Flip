using UnityEngine;
using System.Collections;

public class FlipManager : MonoBehaviour
{
    public static bool IsInverted { get; private set; }

    [Header("Base Timing")]
    [SerializeField] float startMinFlipTime = 4f;
    [SerializeField] float startMaxFlipTime = 7f;

    [Header("Endgame Timing")]
    [SerializeField] float endMinFlipTime = 1.5f;
    [SerializeField] float endMaxFlipTime = 3f;

    [Header("Ramp")]
    [SerializeField] float rampStartTime = 15f;
    [SerializeField] float fullRampTime = 90f;

    [Header("Safety")]
    [SerializeField] float minGapBetweenFlips = 1f;

    float lastFlipTime;

    public static FlipManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        IsInverted = false;
        lastFlipTime = -minGapBetweenFlips;
        StartCoroutine(FlipRoutine());
    }

    IEnumerator FlipRoutine()
    {
        while (true)
        {
            float t = GetRampT();
            float minTime = Mathf.Lerp(startMinFlipTime, endMinFlipTime, t);
            float maxTime = Mathf.Lerp(startMaxFlipTime, endMaxFlipTime, t);

            float wait = Random.Range(minTime, maxTime);
            yield return new WaitForSeconds(wait);

            if (Time.time - lastFlipTime < minGapBetweenFlips)
                continue;

            DoFlip();
        }
    }

    float GetRampT()
    {
        float elapsed = Time.timeSinceLevelLoad - rampStartTime;
        return Mathf.Clamp01(elapsed / fullRampTime);
    }

    void DoFlip()
    {
        IsInverted = !IsInverted;
        lastFlipTime = Time.time;

        ScreenFlash.Instance.Flash();
    }

    public void FreezeFlips(float duration)
    {
        StopAllCoroutines();
        StartCoroutine(FreezeRoutine(duration));
    }

    System.Collections.IEnumerator FreezeRoutine(float duration)
    {
        bool previous = IsInverted;
        IsInverted = false;

        yield return new WaitForSeconds(duration);

        StartCoroutine(FlipRoutine());
    }

}
