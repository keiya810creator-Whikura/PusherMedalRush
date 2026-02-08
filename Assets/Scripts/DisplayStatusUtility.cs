using UnityEngine;

public static class DisplayStatusUtility
{
    public static DisplayStatus Build(SoubiInstance soubi)
    {
        var s = new DisplayStatus();

        if (soubi == null || soubi.master == null)
            return s;

        // 装備
        AddFrom(s, soubi.master);

        // 称号（null対策）
        if (soubi.attachedTitles != null)
        {
            foreach (var t in soubi.attachedTitles)
            {
                if (t == null) continue;
                AddFrom(s, t);
            }
        }

        return s;
    }

    // ===============================
    // 装備（SoubiData）から加算
    // ===============================
    public static void AddFrom(DisplayStatus s, SoubiData d)
    {
        s.attackAdd += d.attackAdd;
        s.attackRate += d.attackRate;

        s.simultaneousShotRate += d.simultaneousShotRate;
        s.criticalAdd += d.criticalAdd;
        s.criticalDamageAdd += d.criticalDamageAdd;

        s.attackMedal2xAdd += d.attackMedal2xAdd;
        s.attackMedal5xAdd += d.attackMedal5xAdd;
        s.attackMedal10xAdd += d.attackMedal10xAdd;

        s.maxMedalAdd += d.maxMedalAdd;
        s.maxMedalRate += d.maxMedalRate;
        s.noMedalConsumeAdd += d.noMedalConsumeAdd;

        s.recoveryWaveMedalValueAdd += d.recoveryWaveMedalValueAdd;
        s.recoveryWaveDurationAdd += d.recoveryWaveDurationAdd;

        s.goldDropAdd += d.goldDropAdd;
        s.equipmentDropAdd += d.equipmentDropAdd;
        s.titleDropAdd += d.titleDropAdd;
    }

    // ===============================
    // 称号（SyougouData）から加算
    // ===============================
    public static void AddFrom(DisplayStatus s, SyougouData d)
    {
        // SoubiData と同種ステータス前提
        s.attackAdd += d.attackAdd;
        s.attackRate += d.attackRate;

        s.simultaneousShotRate += d.simultaneousShotRate;
        s.criticalAdd += d.criticalAdd;
        s.criticalDamageAdd += d.criticalDamageAdd;

        s.attackMedal2xAdd += d.attackMedal2xAdd;
        s.attackMedal5xAdd += d.attackMedal5xAdd;
        s.attackMedal10xAdd += d.attackMedal10xAdd;

        s.maxMedalAdd += d.maxMedalAdd;
        s.maxMedalRate += d.maxMedalRate;
        s.noMedalConsumeAdd += d.noMedalConsumeAdd;

        s.recoveryWaveMedalValueAdd += d.recoveryWaveMedalValueAdd;
        s.recoveryWaveDurationAdd += d.recoveryWaveDurationAdd;

        s.goldDropAdd += d.goldDropAdd;
        s.equipmentDropAdd += d.equipmentDropAdd;
        s.titleDropAdd += d.titleDropAdd;
    }
}
