using System.Collections;
using UnityEngine;

public class RecoveryWaveSaveBridge : MonoBehaviour
{
    private IEnumerator Start()
    {
        yield return null;
        LoadRecoverySetting();
    }

    // ✅セーブ
    public void SaveRecoverySetting(float seconds)
    {
        var data = SaveManager.Instance.Data;

        data.recoverySeconds = seconds;

        SaveManager.Instance.SaveToDisk();

        //Debug.Log($"✅回収Wave時間セーブ: {seconds}秒");
    }

    // ✅ロード
    public void LoadRecoverySetting()
    {
        var data = SaveManager.Instance.Data;

        RecoveryWaveSetting.RecoverySeconds = data.recoverySeconds;

        //Debug.Log($"✅回収Wave時間ロード: {data.recoverySeconds}秒");
    }
}
