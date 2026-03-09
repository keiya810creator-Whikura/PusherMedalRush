using UnityEngine;
using System.Collections;
using TMPro;
public class SlotManager : MonoBehaviour
{
    public static SlotManager Instance;

    [Header("UI")]
    [SerializeField] private GameObject slotOverlay;

    [Header("Reel Contents")]
    [SerializeField] private RectTransform reel1;
    [SerializeField] private RectTransform reel2;
    [SerializeField] private RectTransform reel3;

    [Header("Reel Settings")]
    [SerializeField] private float spinSpeed = 1500f;
    [SerializeField] private float reelStopDelay = 0.4f;

    [Header("Symbol Height")]
    [SerializeField] private float symbolHeight = 160f;

    private bool spinning;

    public bool IsSpinning => spinning;

    private int multiplier = 1;
    bool reel1Spin;
    bool reel2Spin;
    bool reel3Spin;
    const int SYMBOL_SMALL = 1;
    const int SYMBOL_MEDIUM = 2;
    const int SYMBOL_BIG = 3;
    const int SYMBOL_JACKPOT = 4;

    [SerializeField] TMP_Text multiplierText;
    bool slotCooldown;
    [SerializeField] float slotCooldownTime = 1.5f;
    Coroutine slotRoutine;

    public bool IsCooldown => slotCooldown;
    public enum SlotResult
    {
        Miss,
        Small,
        Medium,
        Big,
        Jackpot
    }

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (reel1Spin)
            SpinReel(reel1);

        if (reel2Spin)
            SpinReel(reel2);

        if (reel3Spin)
            SpinReel(reel3);
    }

    void SpinReel(RectTransform reel)
    {
        Vector2 pos = reel.anchoredPosition;

        pos.y += spinSpeed * Time.deltaTime;

        int symbolCount = reel.childCount;

        float loopHeight = symbolHeight * symbolCount;

        if (pos.y >= loopHeight)
            pos.y -= loopHeight;

        reel.anchoredPosition = pos;
    }

    // =========================
    // スロット開始
    // =========================

    public void StartSlot()
    {
        if (spinning || slotCooldown) return;

        spinning = true;

        reel1Spin = true;
        reel2Spin = true;
        reel3Spin = true;

        multiplier = 1;
        UpdateMultiplierText();

        reel1.anchoredPosition = Vector2.zero;
        reel2.anchoredPosition = Vector2.zero;
        reel3.anchoredPosition = Vector2.zero;

        slotOverlay.SetActive(true);

        AudioManager.Instance.PlaySE(AudioManager.Instance.sloaStart);

        // ★コルーチン管理
        if (slotRoutine != null)
            StopCoroutine(slotRoutine);

        slotRoutine = StartCoroutine(SlotRoutine());
    }

    // =========================
    // 倍率UP（特殊獲得口）
    // =========================

    public void AddMultiplier()
    {
        if (!spinning) return;

        multiplier++;

        if (multiplier > 20)
            multiplier = 20;

        UpdateMultiplierText();
    }
    void UpdateMultiplierText()
    {
        if (multiplierText != null)
            multiplierText.text = $"×{multiplier}";
    }
    IEnumerator SlotRoutine()
    {
        if (!spinning)
            yield break;
        SlotResult result = Roll();

        int s1, s2, s3;

        if (result == SlotResult.Miss)
        {
            GetMissSymbols(out s1, out s2, out s3);
        }
        else
        {
            int s = GetStopSymbol(result);
            s1 = s2 = s3 = s;
        }

        yield return new WaitForSeconds(1.5f);

        reel1Spin = false;
        yield return StopReel(reel1, s1);

        yield return new WaitForSeconds(reelStopDelay);

        reel2Spin = false;
        yield return StopReel(reel2, s2);

        yield return new WaitForSeconds(reelStopDelay);

        reel3Spin = false;
        yield return StopReel(reel3, s3);

        spinning = false;

        // ★ここでクールダウン開始
        slotCooldown = true;

        yield return new WaitForSeconds(0.5f);

        ResolveResult(result);

        slotOverlay.SetActive(false);

        StartCoroutine(SlotCooldown());

        slotRoutine = null;


    }
    IEnumerator SlotCooldown()
    {
        slotCooldown = true;

        yield return new WaitForSeconds(slotCooldownTime);

        slotCooldown = false;
    }
    IEnumerator StopReel(RectTransform reel, int symbolIndex)
    {
        int symbolCount = reel.childCount / 2;

        float loopHeight = symbolHeight * symbolCount;

        float target = symbolIndex * symbolHeight;

        float current = reel.anchoredPosition.y;

        // ★常に前方向で止める
        if (target < current)
            target += loopHeight;

        while (target - reel.anchoredPosition.y > 0.5f)
        {
            Vector2 pos = reel.anchoredPosition;

            pos.y += spinSpeed * Time.deltaTime;

            reel.anchoredPosition = pos;

            yield return null;
        }

        // ★正確に止める
        reel.anchoredPosition = new Vector2(
            reel.anchoredPosition.x,
            target % loopHeight
        );
        AudioManager.Instance.PlaySE(AudioManager.Instance.slotStop);
    }

    // =========================
    // 抽選
    // =========================

    SlotResult Roll()
    {
        int r = Random.Range(0, 100);

        if (r < 40) return SlotResult.Miss;
        if (r < 75) return SlotResult.Small;
        if (r < 90) return SlotResult.Medium;
        if (r < 99) return SlotResult.Big;

        return SlotResult.Jackpot;
    }

    // =========================
    // 結果処理
    // =========================

    void ResolveResult(SlotResult result)
    {
        int baseReward = 0;

        switch (result)
        {
            case SlotResult.Small:
                AudioManager.Instance.PlaySE(AudioManager.Instance.atari);
                baseReward = 1;
                break;

            case SlotResult.Medium:
                baseReward = 5;
                AudioManager.Instance.PlaySE(AudioManager.Instance.atari);
                break;

            case SlotResult.Big:
                baseReward = 10;
                AudioManager.Instance.PlaySE(AudioManager.Instance.atari);
                break;

            case SlotResult.Jackpot:
                AudioManager.Instance.PlaySE(AudioManager.Instance.jackpot);
                baseReward = 50;
                break;
        }

        int reward = baseReward * multiplier;

        if (reward > 0)
        {
            MedalGenerator.Instance.SpawnMedals(reward);
        }

        // =========================
        // トースト表示
        // =========================

        if (result == SlotResult.Jackpot)
        {
            ToastManager.Instance.ShowToast2($"JackPot!! Win {reward}");
        }
        else if (reward > 0)
        {
            ToastManager.Instance.ShowToast2($"Win {reward}");
        }
        else
        {
            ToastManager.Instance.ShowToast2($"Miss...");
        }
    }
    int GetStopSymbol(SlotResult result)
    {
        switch (result)
        {
            case SlotResult.Small:
                return SYMBOL_SMALL;

            case SlotResult.Medium:
                return SYMBOL_MEDIUM;

            case SlotResult.Big:
                return SYMBOL_BIG;

            case SlotResult.Jackpot:
                return SYMBOL_JACKPOT;
        }

        return Random.Range(1, 5);
    }
    void GetMissSymbols(out int a, out int b, out int c)
    {
        a = Random.Range(1, 5);
        b = Random.Range(1, 5);
        c = Random.Range(1, 5);

        while (a == b && b == c)
        {
            c = Random.Range(1, 5);
        }
    }
    float GetStopPosition(int symbolIndex)
    {
        return symbolHeight * symbolIndex;
    }
}