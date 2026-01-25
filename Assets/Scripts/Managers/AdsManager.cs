using UnityEngine;
using UnityEngine.Advertisements;

public class AdsManager : MonoBehaviour,
    IUnityAdsInitializationListener,
    IUnityAdsShowListener
{
    [SerializeField] private string gameId = "YOUR_ANDROID_GAME_ID";
    private const string INTERSTITIAL_ID = "Interstitial_Android";
    private const string REWARDED_ID = "Rewarded_Android";

    public static AdsManager Instance;

    private System.Action rewardCallback;

    // ================= UNITY =================
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        Advertisement.Initialize(gameId, true, this); // testMode = true
    }

    // ================= INIT =================
    public void OnInitializationComplete()
    {
        // Preload ads (fire and forget)
        Advertisement.Load(INTERSTITIAL_ID);
        Advertisement.Load(REWARDED_ID);
    }

    public void OnInitializationFailed(
        UnityAdsInitializationError error,
        string message)
    {
        Debug.LogError($"[ADS] Init failed: {error} | {message}");
    }

    // ================= SHOW =================
    public void ShowInterstitial()
    {
        Advertisement.Show(INTERSTITIAL_ID, this);
    }

    public void ShowRewarded(System.Action onReward)
    {
        rewardCallback = onReward;
        Advertisement.Show(REWARDED_ID, this);
    }

    // ================= SHOW CALLBACKS =================
    public void OnUnityAdsShowStart(string placementId) { }

    public void OnUnityAdsShowClick(string placementId) { }

    public void OnUnityAdsShowFailure(
        string placementId,
        UnityAdsShowError error,
        string message)
    {
        Debug.Log($"[ADS] Show failed: {placementId} | {error}");

        // Reload on failure
        Advertisement.Load(placementId);
    }

    public void OnUnityAdsShowComplete(
        string placementId,
        UnityAdsShowCompletionState state)
    {
        if (placementId == REWARDED_ID &&
            state == UnityAdsShowCompletionState.COMPLETED)
        {
            rewardCallback?.Invoke();
        }

        // Always reload after showing
        Advertisement.Load(placementId);
    }
}
