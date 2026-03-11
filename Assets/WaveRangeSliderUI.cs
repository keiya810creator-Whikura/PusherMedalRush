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
        startWaveSlider.onValueChanged.AddListener(OnStartWaveChanged);
        endWaveSlider.onValueChanged.AddListener(OnEndWaveChanged);
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
    public void RefreshFromSave()
    {
        isInitializing = true;

        int stage = SaveManager.Instance.Data.currentStage;

        maxClearedWave =
            stage == 1
            ? Mathf.Max(1, GameProgressManager.Instance.Stage1HighestWave)
            : Mathf.Max(1, GameProgressManager.Instance.Stage2HighestWave);

        startWaveSlider.minValue = 1;
        startWaveSlider.maxValue = Mathf.Max(1, maxClearedWave - 10);

        endWaveSlider.minValue = 1;
        endWaveSlider.maxValue = maxClearedWave + 1;

        var save = SaveManager.Instance.Data;

        int start;
        int end;

        if (stage == 1)
        {
            start = save.stage1StartWave;
            end = save.stage1EndWave;
        }
        else
        {
            start = save.stage2StartWave;
            end = save.stage2EndWave;
        }

        startWaveSlider.SetValueWithoutNotify(start);
        endWaveSlider.SetValueWithoutNotify(end);

        AdventureSession.StartWave = start;
        AdventureSession.EndWave = end;

        // ★ StartWaveText更新
        startWaveText.text = string.Format(
            TextManager.Instance.GetUI("ui_mainmenu_1_9"),
            start
        );

        bool isEndless =
    stage == 1
    ? save.stage1Endless
    : save.stage2Endless;

        if (isEndless)
        {
            endWaveSlider.SetValueWithoutNotify(endWaveSlider.maxValue);

            AdventureSession.IsEndless = true;
            AdventureSession.EndWave = (int)endWaveSlider.maxValue;

            endWaveText.text =
                TextManager.Instance.GetUI("ui_mainmenu_1_13");
        }
        else
        {
            endWaveSlider.SetValueWithoutNotify(end);

            AdventureSession.IsEndless = false;
            AdventureSession.EndWave = end;

            endWaveText.text = string.Format(
                TextManager.Instance.GetUI("ui_mainmenu_1_10"),
                end);
        }

        isInitializing = false;
    }
    public void RebuildSliderRange()
    {
        int stage = SaveManager.Instance.Data.currentStage;

        int highest =
            stage == 1
            ? GameProgressManager.Instance.Stage1HighestWave
            : GameProgressManager.Instance.Stage2HighestWave;

        highest = Mathf.Max(1, highest);

        startWaveSlider.minValue = 1;
        startWaveSlider.maxValue = Mathf.Max(1, highest - 10);

        endWaveSlider.minValue = 1;
        endWaveSlider.maxValue = highest + 1;
    }
}
