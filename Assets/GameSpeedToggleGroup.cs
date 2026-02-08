using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameSpeedToggleGroup : MonoBehaviour
{
    [Header("Toggles")]
    [SerializeField] private Toggle toggle1;
    [SerializeField] private Toggle toggle2;
    [SerializeField] private Toggle toggle3;

    private GameSpeedSaveBridge saveBridge;

    private IEnumerator Start()
    {
        // ✅Bridge取得
        saveBridge = FindAnyObjectByType<GameSpeedSaveBridge>();

        // ✅ロード
        saveBridge?.LoadGameSpeed();

        // ✅UI初期化が終わるまで2フレーム待つ（重要）
        yield return null;
        yield return null;

        // ✅Speed取得
        float speed = GameSpeedSetting.SpeedRate;

        Debug.Log("✅Loaded Speed = " + speed);

        // ✅Toggle初期化（通知なしで確実に反映）
        if (speed == 1f)
        {
            toggle1.SetIsOnWithoutNotify(true);
            toggle2.SetIsOnWithoutNotify(false);
            toggle3.SetIsOnWithoutNotify(false);
        }
        else if (speed == 2f)
        {
            toggle1.SetIsOnWithoutNotify(false);
            toggle2.SetIsOnWithoutNotify(true);
            toggle3.SetIsOnWithoutNotify(false);
        }
        else if (speed == 3f)
        {
            toggle1.SetIsOnWithoutNotify(false);
            toggle2.SetIsOnWithoutNotify(false);
            toggle3.SetIsOnWithoutNotify(true);
        }

        // ✅GameSpeedSettingを再代入（念のため）
        GameSpeedSetting.SpeedRate = speed;
        AdventureSession.GameSpeed = speed;

        Debug.Log("✅Toggle Applied");
    }

    // ============================
    // Toggleから呼ばれる
    // ============================
    public void OnSelectSpeed(float speed)
    {
        // ✅設定保存
        GameSpeedSetting.SpeedRate = speed;
        AdventureSession.GameSpeed = speed;

        saveBridge?.SaveGameSpeed(speed);

        Debug.Log($"✅Speed Saved: x{speed}");
    }
}