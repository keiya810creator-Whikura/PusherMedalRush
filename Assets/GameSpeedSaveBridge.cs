using UnityEngine;

public class GameSpeedSaveBridge : MonoBehaviour
{
    private void Awake()
    {
        //DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        LoadGameSpeed(); // ✅常にロードする
    }

    // ✅セーブ
    public void SaveGameSpeed(float speed)
    {
        var data = SaveManager.Instance.Data;
        data.gameSpeedRate = speed;
        SaveManager.Instance.SaveToDisk();

        // ✅即反映（WaveManagerが居れば）
        ApplyToWaveManager(speed);
    }

    // ✅ロード
    public void LoadGameSpeed()
    {
        var data = SaveManager.Instance.Data;

        // ✅設定へ反映
        GameSpeedSetting.SpeedRate = data.gameSpeedRate;
        AdventureSession.GameSpeed = data.gameSpeedRate;

        // ✅WaveManagerが居れば反映
        ApplyToWaveManager(data.gameSpeedRate);
    }

    // ✅共通適用処理
    private void ApplyToWaveManager(float speed)
    {
        if (WaveManager.Instance != null)
        {
            WaveManager.Instance.SetGameSpeed(speed);
        }
    }
}