using System.Collections;
using UnityEngine;

public class BattleSettingSaveBridge : MonoBehaviour
{
    private IEnumerator Start()
    {
        yield return null;
        LoadBattleSettings();
    }

    // ============================
    // ✅セーブ（開始・終了Wave）
    // ============================
    public void SaveBattleSettings(int startWave, int endWave)
    {
        var data = SaveManager.Instance.Data;

        int stage = data.currentStage;

        if (stage == 1)
        {
            data.stage1StartWave = startWave;
            data.stage1EndWave = endWave;
        }
        else
        {
            data.stage2StartWave = startWave;
            data.stage2EndWave = endWave;
        }

        SaveManager.Instance.SaveToDisk();
    }

    // ============================
    // ✅ロード（開始・終了Wave）
    // ============================
    public void LoadBattleSettings()
    {
        var data = SaveManager.Instance.Data;

        if (data.currentStage == 1)
        {
            AdventureSession.StartWave = data.stage1StartWave;
            AdventureSession.EndWave = data.stage1EndWave;
        }
        else
        {
            AdventureSession.StartWave = data.stage2StartWave;
            AdventureSession.EndWave = data.stage2EndWave;
        }
    }

}
