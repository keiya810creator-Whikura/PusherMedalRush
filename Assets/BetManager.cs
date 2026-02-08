using UnityEngine;
using System;

public class BetManager : MonoBehaviour
{
    public static BetManager Instance;

    public int CurrentBet { get; private set; } = 1;

    public event Action<int> OnBetChanged;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        //DontDestroyOnLoad(gameObject);
    }

    public void SetBet(int value)
    {
        value = Mathf.Clamp(value, 1, 20);

        CurrentBet = value;
        OnBetChanged?.Invoke(CurrentBet);

        // ✅セーブ
        FindAnyObjectByType<BetSaveBridge>()
            ?.SaveBet(CurrentBet);
    }

}
