using System.Collections;
using UnityEngine;

public class ProgressSaveBridge : MonoBehaviour
{
    private IEnumerator Start()
    {
        yield return null;
        LoadProgress();
    }

    // ✅セーブ
    public void SaveProgress()
    {
        var data = SaveManager.Instance.Data;
        data.highestClearedWave = GameProgressManager.Instance.HighestClearedWave;

        SaveManager.Instance.SaveToDisk();

        //Debug.Log($"✅Progress Save: HighestWave={data.highestClearedWave}");
    }

    // ✅ロード
    public void LoadProgress()
    {
        var data = SaveManager.Instance.Data;

        GameProgressManager.Instance.SetHighestClearedWave(data.highestClearedWave);

        //Debug.Log($"✅Progress Load: HighestWave={data.highestClearedWave}");
    }
}
