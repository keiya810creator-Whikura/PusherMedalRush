using System.Collections.Generic;
using UnityEngine;
public static class StatusUpgradeConfig
{

    // ===============================
    // 強化値
    // ===============================

    private static Dictionary<PlayerStatusDatabase.StatusType, float>
        upgradeValueTable =
        new Dictionary<PlayerStatusDatabase.StatusType, float>()
    {
        { PlayerStatusDatabase.StatusType.MaxMedal, 5 },
        { PlayerStatusDatabase.StatusType.Attack, 1 },

        { PlayerStatusDatabase.StatusType.CriticalRate, 0.001f },
        { PlayerStatusDatabase.StatusType.CriticalDamageRate, 0.001f },
        { PlayerStatusDatabase.StatusType.ShotCount, 1 },

        { PlayerStatusDatabase.StatusType.RecoveryWaveExtendSeconds, 1 },
        { PlayerStatusDatabase.StatusType.RecoveryWaveMedalValue, 1 },
        { PlayerStatusDatabase.StatusType.MedalConsumeZeroRate, 0.0005f },

        { PlayerStatusDatabase.StatusType.Attack2xMedalRate, 0.001f },
        { PlayerStatusDatabase.StatusType.Attack5xMedalRate, 0.0005f },
        { PlayerStatusDatabase.StatusType.Attack10xMedalRate, 0.0001f },

        { PlayerStatusDatabase.StatusType.GoldDropRate, 0.005f },
        { PlayerStatusDatabase.StatusType.EquipDropRate, 0.005f },
        { PlayerStatusDatabase.StatusType.TitleDropRate, 0.005f },
    };

    // ===============================
    // 強化コスト
    // ===============================

    private static Dictionary<PlayerStatusDatabase.StatusType, int>
        baseCostTable =
        new Dictionary<PlayerStatusDatabase.StatusType, int>()
    {
        { PlayerStatusDatabase.StatusType.MaxMedal, 100 },
        { PlayerStatusDatabase.StatusType.Attack, 100 },

        { PlayerStatusDatabase.StatusType.CriticalRate, 200 },
        { PlayerStatusDatabase.StatusType.CriticalDamageRate, 200 },
        { PlayerStatusDatabase.StatusType.ShotCount, 300 },

        { PlayerStatusDatabase.StatusType.RecoveryWaveExtendSeconds, 150 },
        { PlayerStatusDatabase.StatusType.RecoveryWaveMedalValue, 150 },
        { PlayerStatusDatabase.StatusType.MedalConsumeZeroRate, 300 },

        { PlayerStatusDatabase.StatusType.Attack2xMedalRate, 400 },
        { PlayerStatusDatabase.StatusType.Attack5xMedalRate, 600 },
        { PlayerStatusDatabase.StatusType.Attack10xMedalRate, 1000 },

        { PlayerStatusDatabase.StatusType.GoldDropRate, 200 },
        { PlayerStatusDatabase.StatusType.EquipDropRate, 300 },
        { PlayerStatusDatabase.StatusType.TitleDropRate, 300 },
    };

    // ===============================
    // 公開API
    // ===============================

    public static float GetUpgradeValue(PlayerStatusDatabase.StatusType type)
    {
        return type switch
        {
            PlayerStatusDatabase.StatusType.MaxMedal => 5,
            PlayerStatusDatabase.StatusType.Attack => 1,

            PlayerStatusDatabase.StatusType.ShotCount => 1,
            PlayerStatusDatabase.StatusType.CriticalRate => 0.001f,
            PlayerStatusDatabase.StatusType.CriticalDamageRate => 0.001f,

            PlayerStatusDatabase.StatusType.RecoveryWaveExtendSeconds => 1,
            PlayerStatusDatabase.StatusType.RecoveryWaveMedalValue => 1,
            PlayerStatusDatabase.StatusType.MedalConsumeZeroRate => 0.0005f,

            PlayerStatusDatabase.StatusType.Attack2xMedalRate => 0.001f,
            PlayerStatusDatabase.StatusType.Attack5xMedalRate => 0.0005f,
            PlayerStatusDatabase.StatusType.Attack10xMedalRate => 0.0001f,

            PlayerStatusDatabase.StatusType.GoldDropRate => 0.005f,
            PlayerStatusDatabase.StatusType.EquipDropRate => 0.005f,
            PlayerStatusDatabase.StatusType.TitleDropRate => 0.005f,
            _ => 0
        };
    }

    public static int GetUpgradeCost(PlayerStatusDatabase.StatusType type, int upgradeCount)
    {
        float baseCost;
        float rate;

        switch (type)
        {
            case PlayerStatusDatabase.StatusType.Attack:
            case PlayerStatusDatabase.StatusType.MaxMedal:
                baseCost = 5; rate = 1.016f; break;

            case PlayerStatusDatabase.StatusType.ShotCount:
                baseCost = 50000; rate = 5.2f; break;

            case PlayerStatusDatabase.StatusType.CriticalRate:
                baseCost = 150; rate = 1.0425f; break;

            case PlayerStatusDatabase.StatusType.CriticalDamageRate:
                baseCost = 300; rate = 1.0118f; break;

            case PlayerStatusDatabase.StatusType.RecoveryWaveMedalValue:
                baseCost = 10000;rate = 1.208f; break;

            case PlayerStatusDatabase.StatusType.RecoveryWaveExtendSeconds:
                baseCost = 5; rate = 1.309f; break;

            case PlayerStatusDatabase.StatusType.MedalConsumeZeroRate:
            case PlayerStatusDatabase.StatusType.Attack10xMedalRate:
            case PlayerStatusDatabase.StatusType.Attack5xMedalRate:
            case PlayerStatusDatabase.StatusType.Attack2xMedalRate:
                baseCost = 5; rate = 1.0545f; break;

            case PlayerStatusDatabase.StatusType.GoldDropRate:
            case PlayerStatusDatabase.StatusType.EquipDropRate:
            case PlayerStatusDatabase.StatusType.TitleDropRate:
                baseCost = 5; rate = 1.017f; break;

            default:
                baseCost = 5; rate = 1.0545f; break;
        }

        return Mathf.CeilToInt(baseCost * Mathf.Pow(rate, upgradeCount));
    }
    // ===============================
    // 強化上限回数
    // ===============================

    private static Dictionary<PlayerStatusDatabase.StatusType, int>
        maxUpgradeCountTable =
        new Dictionary<PlayerStatusDatabase.StatusType, int>()
    {
    // ✅1000回
    { PlayerStatusDatabase.StatusType.Attack, 999 },
    { PlayerStatusDatabase.StatusType.MaxMedal, 999 },
    { PlayerStatusDatabase.StatusType.CriticalDamageRate, 1000 },

    { PlayerStatusDatabase.StatusType.GoldDropRate, 1000 },
    { PlayerStatusDatabase.StatusType.EquipDropRate, 1000 },
    { PlayerStatusDatabase.StatusType.TitleDropRate, 1000 },

    // ✅5回
    { PlayerStatusDatabase.StatusType.ShotCount, 5 },

    // ✅300回
    { PlayerStatusDatabase.StatusType.CriticalRate, 300 },
    { PlayerStatusDatabase.StatusType.MedalConsumeZeroRate, 300 },

    { PlayerStatusDatabase.StatusType.Attack2xMedalRate, 300 },
    { PlayerStatusDatabase.StatusType.Attack5xMedalRate, 300 },
    { PlayerStatusDatabase.StatusType.Attack10xMedalRate, 300 },

    // ✅60回
    { PlayerStatusDatabase.StatusType.RecoveryWaveExtendSeconds, 60 },

    // ✅500回
    { PlayerStatusDatabase.StatusType.RecoveryWaveMedalValue, 30 },
    };
    public static int GetMaxUpgradeCount(PlayerStatusDatabase.StatusType type)
    {
        if (maxUpgradeCountTable.TryGetValue(type, out int max))
            return max;

        return 999; // デフォルト
    }
    public static bool IsMax(PlayerStatusDatabase.StatusType type, int currentCount)
    {
        return currentCount >= GetMaxUpgradeCount(type);
    }

}
