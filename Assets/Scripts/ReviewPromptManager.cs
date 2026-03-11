using System.Collections;
using UnityEngine;

#if UNITY_ANDROID
using Google.Play.Review;
#endif

public class ReviewPromptManager : MonoBehaviour
{
    public static ReviewPromptManager Instance;

    [Header("Review Settings")]
    [SerializeField] private int triggerClearCount = 3;   // 例: 3回クリアで候補
    [SerializeField] private bool onlyOnce = true;

    private const string ReviewRequestedKey = "ReviewRequested";
    private const string ReviewClearCountKey = "ReviewClearCount";

#if UNITY_ANDROID
    private ReviewManager reviewManager;
    private PlayReviewInfo playReviewInfo;
#endif

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// クリア時などに呼ぶ
    /// </summary>
    public void OnPositiveMoment()
    {
        int count = PlayerPrefs.GetInt(ReviewClearCountKey, 0);
        count++;
        PlayerPrefs.SetInt(ReviewClearCountKey, count);
        PlayerPrefs.Save();

        if (onlyOnce && PlayerPrefs.GetInt(ReviewRequestedKey, 0) == 1)
            return;

        if (count >= triggerClearCount)
        {
            TryRequestReview();
        }
    }

    public void TryRequestReview()
    {
        if (onlyOnce && PlayerPrefs.GetInt(ReviewRequestedKey, 0) == 1)
            return;

#if UNITY_IOS
        RequestIOSReview();

#elif UNITY_ANDROID
        StartCoroutine(RequestAndroidReviewCoroutine());

#else
        Debug.Log("Review prompt is not supported on this platform.");
#endif
    }

#if UNITY_IOS
    private void RequestIOSReview()
    {
        // iOSは表示回数や表示有無をOS側が制御
        UnityEngine.iOS.Device.RequestStoreReview();

        if (onlyOnce)
        {
            PlayerPrefs.SetInt(ReviewRequestedKey, 1);
            PlayerPrefs.Save();
        }
    }
#endif

#if UNITY_ANDROID
    private IEnumerator RequestAndroidReviewCoroutine()
    {
        reviewManager ??= new ReviewManager();

        var requestFlowOperation = reviewManager.RequestReviewFlow();
        yield return requestFlowOperation;

        if (requestFlowOperation.Error != ReviewErrorCode.NoError)
        {
            Debug.LogWarning("RequestReviewFlow failed: " + requestFlowOperation.Error);
            yield break;
        }

        playReviewInfo = requestFlowOperation.GetResult();

        var launchFlowOperation = reviewManager.LaunchReviewFlow(playReviewInfo);
        yield return launchFlowOperation;

        playReviewInfo = null;

        if (launchFlowOperation.Error != ReviewErrorCode.NoError)
        {
            Debug.LogWarning("LaunchReviewFlow failed: " + launchFlowOperation.Error);
            yield break;
        }

        if (onlyOnce)
        {
            PlayerPrefs.SetInt(ReviewRequestedKey, 1);
            PlayerPrefs.Save();
        }

        Debug.Log("Review flow finished.");
    }
#endif
}