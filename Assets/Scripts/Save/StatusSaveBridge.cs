using UnityEngine;

public class StatusSaveBridge : MonoBehaviour
{
    private void Start()
    {
        LoadStatus();
    }

    // ============================
    // ✅セーブ
    // ============================
    public void SaveStatus()
    {
        var save = SaveManager.Instance.Data;
        save.statusUpgrades.Clear();

        foreach (var type in PlayerStatusDatabase.Instance.GetAllStatusTypes())
        {
            save.statusUpgrades.Add(new StatusUpgradeSaveData
            {
                statusType = (int)type,
                upgradeCount = PlayerStatusDatabase.Instance.GetUpgradeCount(type)
            });
        }

        SaveManager.Instance.SaveToDisk();
        //Debug.Log("✅ステータス強化セーブ完了");
    }

    // ============================
    // ✅ロード
    // ============================
    public void LoadStatus()
    {
        var save = SaveManager.Instance.Data;

        foreach (var entry in save.statusUpgrades)
        {
            var type = (PlayerStatusDatabase.StatusType)entry.statusType;
            PlayerStatusDatabase.Instance.SetUpgradeCount(type, entry.upgradeCount);
        }

        // ✅UI更新
        //PlayerStatusDatabase.Instance.OnStatusChanged?.Invoke();

        // ✅装備倍率も含めて再計算
        PlayerStatusDatabase.Instance.RecalculateFromEquipments();
        PlayerStatusDatabase.Instance.NotifyStatusChanged();
        //Debug.Log("✅ステータス強化ロード完了");
    }

}
