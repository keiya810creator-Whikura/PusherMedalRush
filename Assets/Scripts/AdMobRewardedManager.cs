using System;
using UnityEngine;
using GoogleMobileAds.Api;

public class AdMobRewardedManager : MonoBehaviour
{
    public static AdMobRewardedManager Instance;

#if UNITY_IOS
    // ✅まずはテスト広告ID（安全）
    private const string REWARDED_AD_UNIT_ID = "ca-app-pub-6220135529861385/5264125182";
#else
    // Androidも後で対応するならテストIDを入れる
    private const string REWARDED_AD_UNIT_ID = "ca-app-pub-6220135529861385/3509858683";
#endif

    private RewardedAd rewardedAd;
    private Action onReward;

    private bool initialized;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        // DontDestroyOnLoad(gameObject);  // 必要なら root にしてから
    }

    void Start()
    {
        // ✅起動直後に落ちにくいタイミングで初期化
        if (!initialized)
        {
            initialized = true;
            MobileAds.Initialize(_ =>
            {
                LoadRewarded();
            });
        }
    }

    public void LoadRewarded()
    {
        var request = new AdRequest();

        RewardedAd.Load(REWARDED_AD_UNIT_ID, request, (ad, error) =>
        {
            if (error != null || ad == null)
            {
                Debug.LogError($"❌ Rewarded load failed: {error}");
                rewardedAd = null;
                return;
            }

            rewardedAd = ad;

            // 使い捨てなので、閉じたら次をロード
            rewardedAd.OnAdFullScreenContentClosed += () =>
            {
                rewardedAd.Destroy();
                rewardedAd = null;
                LoadRewarded();
            };

            rewardedAd.OnAdFullScreenContentFailed += (err) =>
            {
                Debug.LogError($"❌ Fullscreen failed: {err}");
                rewardedAd.Destroy();
                rewardedAd = null;
                LoadRewarded();
            };

            Debug.Log("✅ Rewarded loaded");
        });
    }

    public bool IsReady()
    {
        return rewardedAd != null && rewardedAd.CanShowAd();
    }

    public void ShowRewarded(Action onRewardCallback)
    {
        if (!IsReady()) return;

        onReward = onRewardCallback;

        rewardedAd.Show(reward =>
        {
            // 報酬確定
            onReward?.Invoke();
            onReward = null;
        });
    }
}
