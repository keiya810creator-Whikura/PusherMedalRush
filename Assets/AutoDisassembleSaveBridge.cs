using System.Collections;
using UnityEngine;

public class AutoDisassembleSaveBridge : MonoBehaviour
{
    private IEnumerator Start()
    {
        yield return null;
        LoadSetting();
    }

    // ✅セーブ
    public void SaveSetting()
    {
        var data = SaveManager.Instance.Data;

        data.autoDisassembleRarity =
            (bool[])AutoDisassembleSetting.EnableRarity.Clone();

        SaveManager.Instance.SaveToDisk();

        //Debug.Log("✅自動分解設定セーブ完了");
    }

    // ✅ロード
    public void LoadSetting()
    {
        var data = SaveManager.Instance.Data;

        if (data.autoDisassembleRarity != null &&
            data.autoDisassembleRarity.Length == 7)
        {
            AutoDisassembleSetting.EnableRarity =
                (bool[])data.autoDisassembleRarity.Clone();
        }

        //Debug.Log("✅自動分解設定ロード完了");
    }
}
