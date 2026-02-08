using UnityEngine;
using System;
using System.Collections.Generic;

public class PlayerStatusDatabase : MonoBehaviour
{
    public static PlayerStatusDatabase Instance;
    public event Action OnStatusChanged;

    // 強化回数
    private Dictionary<StatusType, int> upgradeCountTable =
        new Dictionary<StatusType, int>();

    [Header("DEBUG : Final Status (Inspector View)")]
    [SerializeField] private List<DebugStatusView> debugFinalStatus = new();
    // =========================
    // Skill Bonus（スキルで増える分）
    // =========================

    [Header("Skill Bonus")]
    public float skillAttackRateBonus = 0f;
    public float skillMaxMedalRateBonus = 0f;
    void Awake()
    {
        Instance = this;

        foreach (var type in GetAllStatusTypes())
        {
            if (!upgradeCountTable.ContainsKey(type))
                upgradeCountTable[type] = 0;
        }
    }

    public int GetUpgradeCount(StatusType type)
    {
        return upgradeCountTable[type];
    }
    // ===============================
    // ステータス種別（UI並び順＝Enum順）
    // ===============================

    public enum StatusType
    {
        // --- 基礎 ---
        MaxMedal,          // メダル量（HP）
        Attack,            // 攻撃力

        // --- 戦闘 ---
        CriticalRate,
        CriticalDamageRate,
        ShotCount,

        // --- 回収 ---
        RecoveryWaveExtendSeconds,
        RecoveryWaveMedalValue,
        MedalConsumeZeroRate,

        // --- 倍率メダル ---
        Attack2xMedalRate,
        Attack5xMedalRate,
        Attack10xMedalRate,

        // --- ドロップ / 経済 ---
        GoldDropRate,
        EquipDropRate,
        TitleDropRate,
    }

    // ===============================
    // 基礎ステータス（仮値）
    // ===============================

    [SerializeField]
    private Dictionary<StatusType, float> baseStatusTable =
        new Dictionary<StatusType, float>()
    {
        { StatusType.MaxMedal, 50 },
        { StatusType.Attack, 1 },

        { StatusType.CriticalRate, 0 },
        { StatusType.CriticalDamageRate, 2 },
        { StatusType.ShotCount, 1 },

        { StatusType.RecoveryWaveExtendSeconds, 30 },
        { StatusType.RecoveryWaveMedalValue, 1 },
        { StatusType.MedalConsumeZeroRate, 0 },

        { StatusType.Attack2xMedalRate, 0 },
        { StatusType.Attack5xMedalRate, 0 },
        { StatusType.Attack10xMedalRate, 0 },

        { StatusType.GoldDropRate, 1 },
        { StatusType.EquipDropRate, 1 },
        { StatusType.TitleDropRate, 1 },
    };

    // ===============================
    // Getter（UI / ロジック共通）
    // ===============================

    public float GetStatusValue(StatusType type)
    {
        if (!baseStatusTable.ContainsKey(type))
        {
            Debug.LogWarning($"Status not found: {type}");
            return 0;
        }

        return baseStatusTable[type];
    }

    // ===============================
    // 強化処理（StatRowから呼ぶ想定）
    // ===============================

    public void AddStatusValue(StatusType type, float addValue)
    {
        baseStatusTable[type] += addValue;
        upgradeCountTable[type]++;

        OnStatusChanged?.Invoke(); // ★これ必須
        FindAnyObjectByType<StatusSaveBridge>()?.SaveStatus();

    }


    // ===============================
    // UI用：全ステータス取得
    // ===============================

    public IEnumerable<StatusType> GetAllStatusTypes()
    {
        return Enum.GetValues(typeof(StatusType)) as StatusType[];
    }
    public void AddStatus(StatusType type, float value)
    {
        if (!baseStatusTable.ContainsKey(type))
        {
            baseStatusTable[type] = 0;
        }

        baseStatusTable[type] += value;
    }
    public void ResetAll()
    {
        Debug.LogError("🔥 ResetAll() が呼ばれました！！\n" + Environment.StackTrace);
        var keys = new List<StatusType>(baseStatusTable.Keys);
        foreach (var key in keys)
        {
            baseStatusTable[key] = GetBaseDefaultValue(key);
        }
    }
    public void ResetAllForNewGame()
    {
        foreach (var key in GetAllStatusTypes())
        {
            baseStatusTable[key] = GetBaseDefaultValue(key);
            upgradeCountTable[key] = 0;
        }

        NotifyStatusChanged();
    }
    public void ResetBaseValuesOnly()
    {
        foreach (var key in GetAllStatusTypes())
        {
            baseStatusTable[key] = GetBaseDefaultValue(key);
        }
    }

    private float GetBaseDefaultValue(StatusType type)
    {
        return type switch
        {
            StatusType.MaxMedal => 100,
            StatusType.Attack => 1,

            StatusType.CriticalRate => 0,
            StatusType.CriticalDamageRate => 2,
            StatusType.ShotCount => 1,

            StatusType.RecoveryWaveExtendSeconds => 30,
            StatusType.RecoveryWaveMedalValue => 1,
            StatusType.MedalConsumeZeroRate => 0,

            StatusType.Attack2xMedalRate => 0,
            StatusType.Attack5xMedalRate => 0,
            StatusType.Attack10xMedalRate => 0,

            StatusType.GoldDropRate => 1,
            StatusType.EquipDropRate => 1,
            StatusType.TitleDropRate => 1,

            _ => 0
        };
    }
    public void RecalculateFromEquipments()
    {
        // ✅強化回数を消さない初期化
        ResetBaseValuesOnly();

        // ✅強化回数ぶんを反映
        foreach (var type in GetAllStatusTypes())
        {
            int count = upgradeCountTable[type];

            float addValue = StatusUpgradeConfig.GetUpgradeValue(type);

            baseStatusTable[type] += addValue * count;
        }

        NotifyStatusChanged();
    }


    public float GetFinalStatusValue(StatusType type)
    {
        float baseValue = GetBaseValue(type);
        float equipAdd = GetEquipAdd(type);
        float equipRate = GetEquipRate(type);

        // ✅Skill倍率を追加
        float skillRate = GetSkillRate(type);

        switch (type)
        {
            case StatusType.Attack:
            case StatusType.MaxMedal:
                return (baseValue + equipAdd) * (1f + equipRate + skillRate);

            default:
                return baseValue + equipAdd;
        }
    }
    private float GetBaseValue(StatusType type)
    {
        // baseStatusTable には
        // 「基礎値 ＋ 強化分」がすでに入っている
        return GetStatusValue(type);
    }
    private float GetEquipAdd(StatusType type)
    {
        float sum = 0f;

        foreach (var s in InventoryManager.Instance.soubiList)
        {
            if (!s.isEquipped) continue;

            sum += GetAddFromSoubi(type, s.master);

            foreach (var t in s.attachedTitles)
                sum += GetAddFromSyougou(type, t);
        }

        return sum;
    }
    private float GetEquipRate(StatusType type)
    {
        float sum = 0f;

        foreach (var s in InventoryManager.Instance.soubiList)
        {
            if (!s.isEquipped) continue;

            sum += GetRateFromSoubi(type, s.master);

            foreach (var t in s.attachedTitles)
                sum += GetRateFromSyougou(type, t);
        }

        return sum;
    }
    private float GetAddFromSoubi(StatusType type, SoubiData d)
    {
        return type switch
        {
            StatusType.Attack => d.attackAdd,
            StatusType.MaxMedal => d.maxMedalAdd,

            StatusType.CriticalRate => d.criticalAdd,
            StatusType.CriticalDamageRate => d.criticalDamageAdd,
            StatusType.ShotCount => d.simultaneousShotRate,

            StatusType.Attack2xMedalRate => d.attackMedal2xAdd,
            StatusType.Attack5xMedalRate => d.attackMedal5xAdd,
            StatusType.Attack10xMedalRate => d.attackMedal10xAdd,

            StatusType.MedalConsumeZeroRate => d.noMedalConsumeAdd,
            StatusType.RecoveryWaveMedalValue => d.recoveryWaveMedalValueAdd,
            StatusType.RecoveryWaveExtendSeconds => d.recoveryWaveDurationAdd,

            StatusType.GoldDropRate => d.goldDropAdd,
            StatusType.EquipDropRate => d.equipmentDropAdd,
            StatusType.TitleDropRate => d.titleDropAdd,

            _ => 0f
        };
    }


    private float GetAddFromSyougou(StatusType type, SyougouData d)
    {
        return type switch
        {
            StatusType.Attack => d.attackAdd,
            StatusType.MaxMedal => d.maxMedalAdd,

            StatusType.CriticalRate => d.criticalAdd,
            StatusType.CriticalDamageRate => d.criticalDamageAdd,
            StatusType.ShotCount => d.simultaneousShotRate,

            StatusType.Attack2xMedalRate => d.attackMedal2xAdd,
            StatusType.Attack5xMedalRate => d.attackMedal5xAdd,
            StatusType.Attack10xMedalRate => d.attackMedal10xAdd,

            StatusType.MedalConsumeZeroRate => d.noMedalConsumeAdd,
            StatusType.RecoveryWaveMedalValue => d.recoveryWaveMedalValueAdd,
            StatusType.RecoveryWaveExtendSeconds => d.recoveryWaveDurationAdd,

            StatusType.GoldDropRate => d.goldDropAdd,
            StatusType.EquipDropRate => d.equipmentDropAdd,
            StatusType.TitleDropRate => d.titleDropAdd,

            _ => 0f
        };
    }
    private float GetRateFromSoubi(StatusType type, SoubiData d)
    {
        return type switch
        {
            StatusType.Attack => d.attackRate,
            StatusType.MaxMedal => d.maxMedalRate,
            _ => 0f
        };
    }

    private float GetRateFromSyougou(StatusType type, SyougouData d)
    {
        return type switch
        {
            StatusType.Attack => d.attackRate,
            StatusType.MaxMedal => d.maxMedalRate,
            _ => 0f
        };
    }
    [System.Serializable]
    public class DebugStatusView
    {
        public PlayerStatusDatabase.StatusType type;
        public float value;
    }
    private void UpdateDebugView()
    {
        debugFinalStatus.Clear();

        foreach (var type in GetAllStatusTypes())
        {
            debugFinalStatus.Add(new DebugStatusView
            {
                type = type,
                value = GetFinalStatusValue(type)
            });
        }
    }
    public void SetUpgradeCount(StatusType type, int count)
    {
        // ✅強化回数をセット
        upgradeCountTable[type] = count;

        // ✅基礎値に戻す
        baseStatusTable[type] = GetBaseDefaultValue(type);

        // ✅強化回数ぶん加算し直す
        float addValue = StatusUpgradeConfig.GetUpgradeValue(type);
        baseStatusTable[type] += addValue * count;
        //Debug.Log($"LOAD {type} count={count} value={baseStatusTable[type]}");

    }
    public void NotifyStatusChanged()
    {
        OnStatusChanged?.Invoke();
    }
    public void ResetSkillBonus()
    {
        skillAttackRateBonus = 0f;
        skillMaxMedalRateBonus = 0f;
    }
    private float GetSkillRate(StatusType type)
    {
        return type switch
        {
            StatusType.Attack => skillAttackRateBonus,
            StatusType.MaxMedal => skillMaxMedalRateBonus,
            _ => 0f
        };
    }
}
