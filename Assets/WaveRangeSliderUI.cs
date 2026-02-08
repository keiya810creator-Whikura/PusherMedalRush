using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WaveRangeSliderUI : MonoBehaviour
{
    [Header("Sliders")]
    [SerializeField] private Slider startWaveSlider;
    [SerializeField] private Slider endWaveSlider;

    [Header("Texts")]
    [SerializeField] private TMP_Text startWaveText;
    [SerializeField] private TMP_Text endWaveText;

    public int StartWave => Mathf.RoundToInt(startWaveSlider.value);
    public int EndWave => Mathf.RoundToInt(endWaveSlider.value);

    private int maxClearedWave;
    public static WaveRangeSliderUI instance;
    private bool isInitializing;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }
    void Start()
    {
        isInitializing = true;

        maxClearedWave = Mathf.Max(1, GameProgressManager.Instance.HighestClearedWave);

        startWaveSlider.minValue = 1;
        startWaveSlider.maxValue = maxClearedWave;
        startWaveSlider.wholeNumbers = true;

        endWaveSlider.minValue = 1;
        endWaveSlider.maxValue = maxClearedWave + 1;
        endWaveSlider.wholeNumbers = true;

        // ✅イベント登録はまだしない！！

        // =========================
        // ✅復元（イベント暴発防止）
        // =========================

        startWaveSlider.SetValueWithoutNotify(
            Mathf.Clamp(
                AdventureSession.StartWave > 0 ? AdventureSession.StartWave : 1,
                startWaveSlider.minValue,
                startWaveSlider.maxValue
            )
        );

        if (AdventureSession.IsEndless)
        {
            endWaveSlider.SetValueWithoutNotify(endWaveSlider.maxValue);
        }
        else
        {
            endWaveSlider.SetValueWithoutNotify(
                Mathf.Clamp(
                    AdventureSession.EndWave > 0 ? AdventureSession.EndWave : startWaveSlider.value,
                    endWaveSlider.minValue,
                    endWaveSlider.maxValue
                )
            );
        }

        // =========================
        // ✅ここでイベント登録
        // =========================
        startWaveSlider.onValueChanged.AddListener(OnStartWaveChanged);
        endWaveSlider.onValueChanged.AddListener(OnEndWaveChanged);

        // ✅初期反映
        OnStartWaveChanged(startWaveSlider.value);
        OnEndWaveChanged(endWaveSlider.value);
        isInitializing = false;
    }



    public void SliderMaxSet()
    {
        endWaveSlider.value = endWaveSlider.maxValue;

    }
    void OnStartWaveChanged(float value)
    {
        int start = Mathf.RoundToInt(value);

        endWaveSlider.minValue = start;

        if (endWaveSlider.value < start)
            endWaveSlider.value = start;

        startWaveText.text = string.Format(
        TextManager.Instance.GetUI("ui_mainmenu_1_9"),
        start);

        AdventureSession.StartWave = start;

        if (isInitializing) return; // ✅復元中は保存しない

        FindAnyObjectByType<BattleSettingSaveBridge>()
            ?.SaveBattleSettings(start, AdventureSession.EndWave);
    }


    void OnEndWaveChanged(float value)
    {
        int end = Mathf.RoundToInt(value);

        bool isMax = (end == endWaveSlider.maxValue);

        if (isMax)
        {
            AdventureSession.IsEndless = true;
            AdventureSession.EndWave = end;

            endWaveText.text = string.Format(
        TextManager.Instance.GetUI("ui_mainmenu_1_13")
        );

            if (!isInitializing)
            {
                FindAnyObjectByType<BattleSettingSaveBridge>()
                    ?.SaveBattleSettings(StartWave, end);
            }

            return;
        }

        AdventureSession.IsEndless = false;
        AdventureSession.EndWave = end;

        endWaveText.text = string.Format(
        TextManager.Instance.GetUI("ui_mainmenu_1_10"),
        end);

        if (isInitializing) return;

        FindAnyObjectByType<BattleSettingSaveBridge>()
            ?.SaveBattleSettings(StartWave, end);
    }



    // ===== Start Wave Buttons =====
    public void StartMinus1() => AdjustStartWave(-1);
    public void StartMinus10() => AdjustStartWave(-10);
    public void StartPlus1() => AdjustStartWave(+1);
    public void StartPlus10() => AdjustStartWave(+10);

    void AdjustStartWave(int delta)
    {
        float newValue = startWaveSlider.value + delta;

        // Min / Max を超えない
        newValue = Mathf.Clamp(
            newValue,
            startWaveSlider.minValue,
            startWaveSlider.maxValue
        );

        startWaveSlider.value = newValue;
    }
    // ===== End Wave Buttons =====
    public void EndMinus1() => AdjustEndWave(-1);
    public void EndMinus10() => AdjustEndWave(-10);
    public void EndPlus1() => AdjustEndWave(+1);
    public void EndPlus10() => AdjustEndWave(+10);

    void AdjustEndWave(int delta)
    {
        float newValue = endWaveSlider.value + delta;

        // Min / Max を超えない
        newValue = Mathf.Clamp(
            newValue,
            endWaveSlider.minValue,
            endWaveSlider.maxValue
        );

        endWaveSlider.value = newValue;
    }

}
