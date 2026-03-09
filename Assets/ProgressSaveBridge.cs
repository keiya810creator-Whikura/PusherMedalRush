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

        data.stage1HighestWave = GameProgressManager.Instance.Stage1HighestWave;
        data.stage2HighestWave = GameProgressManager.Instance.Stage2HighestWave;

        SaveManager.Instance.SaveToDisk();
    }

    // ✅ロード
    public void LoadProgress()
    {
        var data = SaveManager.Instance.Data;

        // 旧セーブデータ救済
        if (data.stage1HighestWave <= 1 && data.highestClearedWave > 1)
        {
            data.stage1HighestWave = data.highestClearedWave;
        }

        GameProgressManager.Instance.Stage1HighestWave = data.stage1HighestWave;
        GameProgressManager.Instance.Stage2HighestWave = data.stage2HighestWave;

        Debug.Log($"Progress Load: Stage1={data.stage1HighestWave} Stage2={data.stage2HighestWave}");
    }
}
