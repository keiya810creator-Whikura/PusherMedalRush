public static class StatusSumUtility
{
    public static DisplayStatus Sum(SoubiInstance soubi)
    {
        var s = new DisplayStatus();

        // ëïîıñ{ëÃ
        AddFrom(s, soubi.master);

        // ïtó^èÃçÜ
        foreach (var t in soubi.attachedTitles)
        {
            AddFrom(s, t);
        }

        return s;
    }

    private static void AddFrom(DisplayStatus s, SoubiData d)
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

    private static void AddFrom(DisplayStatus s, SyougouData d)
    {
        // SoubiData Ç∆äÆëSìØå^Ç»ÇÃÇ≈ìØÇ∂èàóù
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
