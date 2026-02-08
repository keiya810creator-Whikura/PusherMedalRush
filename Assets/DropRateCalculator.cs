using UnityEngine;

public static class DropRateCalculator
{
    /// <summary>
    /// 最終称号ドロップ倍率を返す
    /// </summary>
    public static float GetFinalTitleDropMultiplier()
    {
        float multiplier = 1f;

        // ✅称号課金Boost（1.5倍）
        if (PurchaseState.HasTitleBoost)
            multiplier *= 1.5f;

        // ✅広告削除恩恵（常時2倍）
        if (PurchaseState.HasRemoveAds)
            multiplier *= 2f;

        return multiplier;
    }

    /// <summary>
    /// 最終称号ドロップ率（％表示用）
    /// </summary>
    public static float GetFinalTitleDropRate(float baseRate)
    {
        return baseRate * GetFinalTitleDropMultiplier();
    }
}
