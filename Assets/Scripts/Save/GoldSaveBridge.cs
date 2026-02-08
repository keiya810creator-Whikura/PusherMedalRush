using UnityEngine;

/// <summary>
/// GoldだけをJSONに保存・復元するブリッジ
/// Medalは保存しない（戦闘ごと初期化）
/// </summary>
public class GoldSaveBridge : MonoBehaviour
{
    void Start()
    {
        LoadGold();
    }

    // ============================
    // ✅ロード（ゲーム起動時）
    // ============================
    public void LoadGold()
    {
        long savedGold = SaveManager.Instance.Data.gold;

        MoneyManager.instance.SetGold(savedGold);

        //Debug.Log("✅Goldロード完了: " + savedGold);
    }

    // ============================
    // ✅セーブ（Gold変動時）
    // ============================
    public void SaveGold()
    {
        SaveManager.Instance.Data.gold = MoneyManager.instance.Gold;
        SaveManager.Instance.SaveToDisk();

        //Debug.Log("✅Goldセーブ: " + MoneyManager.instance.Gold);
    }
}
