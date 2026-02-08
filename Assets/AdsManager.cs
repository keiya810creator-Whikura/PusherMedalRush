using UnityEngine;
using UnityEngine.Advertisements;
using System;

public class AdsManager : MonoBehaviour
{ 
   /* IUnityAdsInitializationListener,
    IUnityAdsLoadListener,
    IUnityAdsShowListener
{
    public static AdsManager Instance;

#if UNITY_ANDROID
    private const string GAME_ID = "YOUR_ANDROID_GAME_ID";
    private const string AUTO_RUN_PLACEMENT_ID = "Rewarded_Android";
#elif UNITY_IOS
    private const string GAME_ID = "6030281";
    private const string AUTO_RUN_PLACEMENT_ID = "Rewarded_iOS";
    private const string REWARD_BOOST_PLACEMENT_ID = "Rewarded_Boost";

#endif
    private bool isAutoRunAdLoaded;
    private bool isRewardBoostAdLoaded;

    private bool isInitialized;
    private bool isAdLoaded;
    private Action onRewardCallback;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        //DontDestroyOnLoad(gameObject);

        Advertisement.Initialize(GAME_ID, false, this);
    }

    // =========================
    // Public API
    // =========================

    public void ShowAutoRunAd(Action onReward)
    {
        if (!isInitialized || !isAutoRunAdLoaded)
            return;

        onRewardCallback = onReward;
        Advertisement.Show(AUTO_RUN_PLACEMENT_ID, this);
    }

    public void ShowRewardBoostAd(Action onReward)
    {
        if (!isInitialized || !isRewardBoostAdLoaded)
            return;

        onRewardCallback = onReward;
        Advertisement.Show(REWARD_BOOST_PLACEMENT_ID, this);
    }

    // =========================
    // Initialization
    // =========================

    public void OnInitializationComplete()
    {
        Debug.Log("Unity Ads initialized");
        isInitialized = true;

        // Åö èâä˙âªäÆóπå„Ç… Load
        Advertisement.Load(AUTO_RUN_PLACEMENT_ID, this);
        Advertisement.Load(REWARD_BOOST_PLACEMENT_ID, this); // Åöí«â¡
    }

    public void OnInitializationFailed(
        UnityAdsInitializationError error,
        string message)
    {
        Debug.LogError($"Ads init failed: {error} / {message}");
    }

    // =========================
    // Load
    // =========================

    public void OnUnityAdsAdLoaded(string placementId)
    {
        if (placementId == AUTO_RUN_PLACEMENT_ID)
        {
            isAutoRunAdLoaded = true;
            Debug.Log("AutoRun ad loaded");
        }
        else if (placementId == REWARD_BOOST_PLACEMENT_ID)
        {
            isRewardBoostAdLoaded = true;
            Debug.Log("RewardBoost ad loaded");
        }
    }


    public void OnUnityAdsFailedToLoad(
        string placementId,
        UnityAdsLoadError error,
        string message)
    {
        Debug.LogError($"Ad load failed: {error} / {message}");
    }

    // =========================
    // Show
    // =========================

    public void OnUnityAdsShowComplete(
    string placementId,
    UnityAdsShowCompletionState showCompletionState)
    {
        if (showCompletionState != UnityAdsShowCompletionState.COMPLETED)
            return;

        onRewardCallback?.Invoke();
        onRewardCallback = null;

        if (placementId == AUTO_RUN_PLACEMENT_ID)
        {
            isAutoRunAdLoaded = false;
            Advertisement.Load(AUTO_RUN_PLACEMENT_ID, this);
        }
        else if (placementId == REWARD_BOOST_PLACEMENT_ID)
        {
            isRewardBoostAdLoaded = false;
            Advertisement.Load(REWARD_BOOST_PLACEMENT_ID, this);
        }
    }


    public void OnUnityAdsShowFailure(
        string placementId,
        UnityAdsShowError error,
        string message)
    {
        Debug.LogError($"Ad show failed: {error} / {message}");
    }

    public void OnUnityAdsShowStart(string placementId) { }
    public void OnUnityAdsShowClick(string placementId) { }
   */
}
