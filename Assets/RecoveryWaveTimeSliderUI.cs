using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RecoveryWaveTimeSliderUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Slider slider;
    [SerializeField] private TMP_Text valueText;

    private float maxValue;

    void Start()
    {
        maxValue = BattleStatusBuilder.Build().RecoveryWaveExtendSeconds;

        slider.minValue = 5f;
        slider.maxValue = maxValue;
        slider.wholeNumbers = true;

        slider.onValueChanged.AddListener(OnValueChanged);

        // ✅前回値を復元
        float saved = RecoveryWaveSetting.RecoverySeconds;

        if (saved <= 0)
            saved = maxValue;

        slider.SetValueWithoutNotify(
            Mathf.Clamp(saved, slider.minValue, slider.maxValue)
        );

        // ✅初期表示更新
        valueText.text = string.Format(
        TextManager.Instance.GetUI("ui_mainmenu_2_8"),
        slider.value
    ); ;
    }


    void OnValueChanged(float value)
    {
        RecoveryWaveSetting.RecoverySeconds = value;
        valueText.text =
    string.Format(
        TextManager.Instance.GetUI("ui_mainmenu_2_8"),
        value
    );

        // ✅変更したら保存
        FindAnyObjectByType<RecoveryWaveSaveBridge>()
            ?.SaveRecoverySetting(value);
    }


    // =========================
    // Button操作
    // =========================

    public void Minus1() => Adjust(-1);
    public void Minus10() => Adjust(-10);
    public void Plus1() => Adjust(+1);
    public void Plus10() => Adjust(+10);

    void Adjust(int delta)
    {
        float newValue = slider.value + delta;

        // ★ 下限と上限を超えない
        newValue = Mathf.Clamp(newValue, slider.minValue, slider.maxValue);

        slider.value = newValue;
    }
}
