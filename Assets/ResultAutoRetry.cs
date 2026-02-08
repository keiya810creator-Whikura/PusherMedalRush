using UnityEngine;
using System.Collections;
using TMPro;

public class ResultAutoRetry : MonoBehaviour
{
    [SerializeField] private float waitSeconds = 3f;
    [SerializeField] private TextMeshProUGUI countDown;

    private Coroutine retryCoroutine;

    void OnEnable()
    {
        if (AdventureSession.IsAutoRun)
        {
            countDown.gameObject.SetActive(true);
            retryCoroutine = StartCoroutine(RetryAfterDelay());
        }
        
    }

    void OnDisable()
    {
        if (retryCoroutine != null)
        {
            StopCoroutine(retryCoroutine);
            retryCoroutine = null;
        }
    }

    IEnumerator RetryAfterDelay()
    {
        float remaining = waitSeconds;

        while (remaining > 0f)
        {
            // ★ 表示は切り上げ（3,2,1）
            countDown.text = Mathf.CeilToInt(remaining).ToString();

            yield return null;
            remaining -= Time.deltaTime;
        }

        countDown.text = "0";

        // ★ リトライ実行
        ResultFlowController.Instance.OnClickRetry();
    }
}
