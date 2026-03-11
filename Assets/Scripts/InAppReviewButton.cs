using UnityEngine;
using System.Collections;

#if UNITY_ANDROID
using Google.Play.Review;
#endif

public class InAppReviewButton : MonoBehaviour
{

#if UNITY_ANDROID
    private ReviewManager reviewManager;
    private PlayReviewInfo playReviewInfo;
#endif

    public void OpenReview()
    {

#if UNITY_IOS

        // iOS アプリ内レビュー
        UnityEngine.iOS.Device.RequestStoreReview();

#elif UNITY_ANDROID

        // Android アプリ内レビュー
        StartCoroutine(RequestAndroidReview());

#endif

    }

#if UNITY_ANDROID
    IEnumerator RequestAndroidReview()
    {
        if (reviewManager == null)
            reviewManager = new ReviewManager();

        var request = reviewManager.RequestReviewFlow();
        yield return request;

        if (request.Error != ReviewErrorCode.NoError)
        {
            Debug.Log("Review request error: " + request.Error);
            yield break;
        }

        playReviewInfo = request.GetResult();

        var launch = reviewManager.LaunchReviewFlow(playReviewInfo);
        yield return launch;

        playReviewInfo = null;

        if (launch.Error != ReviewErrorCode.NoError)
        {
            Debug.Log("Review launch error: " + launch.Error);
        }
        else
        {
            Debug.Log("Review flow finished");
        }
    }
#endif

}