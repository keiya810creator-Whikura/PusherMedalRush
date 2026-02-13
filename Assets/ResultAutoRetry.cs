using UnityEngine;
using System.Collections;
using TMPro;

public class ResultAutoRetry : MonoBehaviour
{
    [SerializeField] private float waitSeconds = 3f;
    [SerializeField] private TextMeshProUGUI countDown;

    [Header("Pause / Play Images")]
    [SerializeField] private GameObject playImage;   // ▶
    [SerializeField] private GameObject pauseImage;  // ⏸

    private Coroutine retryCoroutine;
    private bool isPaused;

    void OnEnable()
    {
        if (AdventureSession.IsAutoRun)
        {
            isPaused = false;
            UpdatePauseVisual();

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

    // =========================
    // ▶ / ⏸ 切り替え（ボタン用）
    // =========================
    public void TogglePause()
    {
        isPaused = !isPaused;
        UpdatePauseVisual();
    }

    private void UpdatePauseVisual()
    {
        if (playImage != null)
            playImage.SetActive(isPaused);      // 停止中 ▶

        if (pauseImage != null)
            pauseImage.SetActive(!isPaused);    // 自動周回中 ⏸
    }

    // =========================
    // カウントダウン処理
    // =========================
    IEnumerator RetryAfterDelay()
    {
        float remaining = waitSeconds;

        while (remaining > 0f)
        {
            if (!isPaused)
            {
                remaining -= Time.deltaTime;
                countDown.text = Mathf.CeilToInt(remaining).ToString();
            }

            yield return null;
        }

        countDown.text = "0";

        // ▶ リトライ実行
        ResultFlowController.Instance.OnClickRetry();
    }
}
