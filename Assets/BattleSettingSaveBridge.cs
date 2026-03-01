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

        data.startWave = startWave;
        data.endWave = endWave;

        // ✅Max判定（EndWaveが最大ならtrue）
        int maxWave = GameProgressManager.Instance.HighestClearedWave + 1;
        data.isEndWaveMax = (endWave >= maxWave);

        SaveManager.Instance.SaveToDisk();

        Debug.Log($"✅BattleSettings Save: {startWave} → {endWave}, MaxFlag={data.isEndWaveMax}");
    }

    // ============================
    // ✅ロード（開始・終了Wave）
    // ============================
    public void LoadBattleSettings()
    {
        var data = SaveManager.Instance.Data;

        AdventureSession.StartWave = data.startWave;

        // ✅MaxだったならEndWaveもMax扱いにする
        if (data.isEndWaveMax)
        {
            AdventureSession.IsEndless = true;
            AdventureSession.EndWave = GameProgressManager.Instance.HighestClearedWave + 1;
        }
        else
        {
            AdventureSession.IsEndless = false;
            AdventureSession.EndWave = data.endWave;
        }

        //Debug.Log($"✅BattleSettings Load: {AdventureSession.StartWave} → {AdventureSession.EndWave}");
    }

}
