using UnityEngine;
using System;

public class MoneyManager : MonoBehaviour
{
    public static MoneyManager instance;

    public long medal;
    [SerializeField] private long gold;

    public long Medal => medal;
    public long Gold => gold;

    //public long MaxMedal => PlayerStatusDatabase.Instance.GetMaxMedal();

    public event Action<long> OnMedalChanged;
    public event Action<long> OnGoldChanged;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }
    private GoldSaveBridge goldBridge;

    void Start()
    {
        goldBridge = FindAnyObjectByType<GoldSaveBridge>();
    }

    // =====================
    // Medal（HP）
    // =====================

    public void ResetMedalToMax()
    {
        //medal = MaxMedal;
        OnMedalChanged?.Invoke(medal);
    }

    public void AddMedal(long value)
    {
        medal += value;
        AudioManager.Instance.MedalKakutokuSE();
        OnMedalChanged?.Invoke(medal);
    }

    public void ConsumeMedal(long value)
    {
        // 回収中は消費しない
        if (WaveManager.Instance != null &&
            WaveManager.Instance.CurrentState == WaveState.Recovery)
        {
            return;
        }

        medal -= value;
        if (medal < 0)
            medal = 0;

        OnMedalChanged?.Invoke(medal);

        if (medal == 0)
        {
            // GameOver
        }
    }


    // =====================
    // Gold
    // =====================

    /// <summary>
    /// ゴールドを加算
    /// </summary>
    public void AddGold(long value)
    {
        if (value <= 0) return;

        AudioManager.Instance.PlaySE(AudioManager.Instance.goldKakutokuSE);
        gold += value;
        ResultManager.Instance.currentResult.gainedGold += value;
        OnGoldChanged?.Invoke(gold);

        // ✅追加：セーブ
        goldBridge?.SaveGold();
    }


    /// <summary>
    /// ゴールドを消費できるか判定して消費
    /// </summary>
    public bool TrySpendGold(long cost)
    {
        if (cost <= 0) return true;

        if (gold < cost)
            return false;

        gold -= cost;
        OnGoldChanged?.Invoke(gold);

        // ✅追加：セーブ
        goldBridge?.SaveGold();

        return true;
    }


    /// <summary>
    /// ゴールドを直接設定（ロード用）
    /// </summary>
    public void SetGold(long value)
    {
        gold = Math.Max(0L, value);
        OnGoldChanged?.Invoke(gold);
    }
}
