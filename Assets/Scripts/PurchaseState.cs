using UnityEngine;

public static class PurchaseState
{
    // =========================
    // Shortcut
    // =========================
    static SaveData Data => SaveManager.Instance.Data;

    // =========================
    // 課金状態
    // =========================
    public static void UnlockBundlePack()
    {
        HasRemoveAds = true;
        HasSpeed3x = true;
        HasTitleBoost = true;

        Debug.Log("✅まとめパック購入：広告削除・3倍速・称号ブースト解放！");
    }

    public static bool HasRemoveAds
    {
        get => Data.hasRemoveAds;
        set
        {
            Data.hasRemoveAds = value;
            SaveManager.Instance.SaveToDisk();
        }
    }

    public static bool HasSpeed3x
    {
        get => Data.hasSpeed3x;
        set
        {
            Data.hasSpeed3x = value;
            SaveManager.Instance.SaveToDisk();
        }
    }

    public static bool HasTitleBoost
    {
        get => Data.hasTitleDropBoost;
        set
        {
            Data.hasTitleDropBoost = value;
            SaveManager.Instance.SaveToDisk();
        }
    }

    // =========================
    // 倉庫サイズ
    // =========================

    public static int StorageSize
    {
        get => Data.storageSize;
        set
        {
            Data.storageSize = value;
            SaveManager.Instance.SaveToDisk();
        }
    }

    public static void ExpandStorage()
    {
        Data.storageExpandCount++;

        Data.storageSize =
            300 + Data.storageExpandCount * 100;

        SaveManager.Instance.SaveToDisk();

        Debug.Log("✅倉庫拡張回数: " + Data.storageExpandCount);
    }

}
