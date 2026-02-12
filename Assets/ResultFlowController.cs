using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class ResultFlowController : MonoBehaviour
{
    public static ResultFlowController Instance;

    [SerializeField] private TMP_Text countdownText;
    [SerializeField] private float countdownSeconds = 5f;

    private bool triggered = false;
    private Coroutine countdownCoroutine;

    [SerializeField] private GameObject TettaiButton;

    private void Start()
    {
        MoneyManager.instance.OnMedalChanged += OnMedalChanged;
    }

    private void OnDestroy()
    {
        if (MoneyManager.instance != null)
            MoneyManager.instance.OnMedalChanged -= OnMedalChanged;
    }
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    private void OnMedalChanged(long medal)
    {
        // ✅Wave消費メダル
        long baseConsume =
            WaveManager.Instance.GetConsumeMedalPerShot();

        // ✅ベット倍率（1〜20）
        int bet =
            BetManager.Instance.CurrentBet;

        // ✅最終消費（ベット込み）
        long finalConsume =
            baseConsume * bet;

        // =========================
        // ✅撃てるなら復帰
        // =========================
        if (medal >= finalConsume)
        {
            CancelCountdown();
            return;
        }

        // =========================
        // ✅一発も撃てないなら終了
        // =========================
        if (medal < finalConsume && !triggered)
        {
            triggered = true;

            ResultManager.Instance.currentResult.clearedWave =
                WaveManager.Instance.CurrentWave;

            if (AdventureSession.IsAutoRun)
            {
                // ✅自動周回：撤退タイム待ち
                countdownCoroutine =
                    StartCoroutine(ResultCountdownRoutine());
            }
            else
            {
                // ✅通常周回：即撤退ボタン
                TettaiButton.SetActive(true);
            }
        }

    }



    public void GoToResult()
    {
        SceneManager.LoadScene("MainMenu");
    }

    // =========================
    // カウントダウン本体
    // =========================
    IEnumerator ResultCountdownRoutine()
    {
        int limitSeconds = AdventureSession.TettaiTime;

        // 撤退タイム0以下なら即終了
        if (limitSeconds <= 0)
        {
            GoToResult();
            yield break;
        }

        float timer = limitSeconds;

        countdownText.gameObject.SetActive(true);

        while (timer > 0f)
        {
            int display = Mathf.CeilToInt(timer);
            countdownText.text =
                string.Format(
                    TextManager.Instance.GetUI("ui_sentou_3"),
                    display
                );

            timer -= Time.deltaTime * WaveManager.Instance.CurrentGameSpeed;
            yield return null;
        }

        countdownText.gameObject.SetActive(false);
        GoToResult();
    }





    // =========================
    // カウントダウン中止＆リセット
    // =========================
    private void CancelCountdown()
    {
        if (!triggered)
            return;

        triggered = false;

        TettaiButton.SetActive(false);

        if (countdownCoroutine != null)
        {
            StopCoroutine(countdownCoroutine);
            countdownCoroutine = null;
        }

        countdownText.gameObject.SetActive(false);
    }

    // ★ リトライボタンもここを呼ぶようにする
    public void OnClickRetry()
    {
        SceneManager.LoadScene("Stage1");
    }

    // ついでに、手動でメニューへ戻るボタン等も分けられる
    public void OnClickBackToMenu()
    {
        AdventureSession.IsAutoRun = false; // 必要なら止める
        SceneManager.LoadScene("MainMenu");
    }
}
