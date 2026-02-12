using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RetreatTimeSliderUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Slider slider;
    [SerializeField] private TMP_Text valueText;

    [Header("Seconds Setting")]
    [SerializeField] private int minSeconds = 0;
    [SerializeField] private int maxSeconds = 300;
    [SerializeField] private int stepSeconds = 10;

    private bool isInitializing;

    void Start()
    {
        isInitializing = true;

        // Slider 基本設定
        slider.minValue = minSeconds;
        slider.maxValue = maxSeconds;
        slider.wholeNumbers = true;

        // セーブから復元
        int saved = SaveManager.Instance.Data.tettaiTime;

        AdventureSession.TettaiTime = saved;
        slider.value = saved;

        RefreshText(saved);

        slider.onValueChanged.AddListener(OnValueChanged);

        isInitializing = false;
    }

    private void OnValueChanged(float rawValue)
    {
        if (isInitializing) return;

        int seconds =
            Mathf.RoundToInt(rawValue / stepSeconds) * stepSeconds;

        slider.SetValueWithoutNotify(seconds);

        // セッション反映
        AdventureSession.TettaiTime = seconds;

        // セーブ反映
        SaveManager.Instance.Data.tettaiTime = seconds;
        SaveManager.Instance.SaveToDisk();

        RefreshText(seconds);
    }

    private void RefreshText(int seconds)
    {
        if (valueText == null) return;
        valueText.text = string.Format(
                    TextManager.Instance.GetUI("ui_mainmenu_1_15"),
                    seconds
                ); ;
    }
}
