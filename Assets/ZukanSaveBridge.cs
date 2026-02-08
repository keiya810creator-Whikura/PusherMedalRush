using System.Collections;
using UnityEngine;

public class ZukanSaveBridge : MonoBehaviour
{
    private IEnumerator Start()
    {
        yield return null;
        LoadZukan();
    }

    // ✅セーブ
    public void SaveZukan()
    {
        var data = SaveManager.Instance.Data;

        data.encounteredMonsters =
            ZukanProgressManager.Instance.GetEncounteredList();

        data.obtainedEquipments =
            ZukanProgressManager.Instance.GetEquipList();

        data.obtainedTitles =
            ZukanProgressManager.Instance.GetTitleList();

        SaveManager.Instance.SaveToDisk();

       // Debug.Log("✅図鑑フラグ保存完了");
    }

    // ✅ロード
    public void LoadZukan()
    {
        var data = SaveManager.Instance.Data;

        ZukanProgressManager.Instance.SetAll(
            data.encounteredMonsters,
            data.obtainedEquipments,
            data.obtainedTitles
        );

        //Debug.Log("✅図鑑フラグロード完了");
    }
}
