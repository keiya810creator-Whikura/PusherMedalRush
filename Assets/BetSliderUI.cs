using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BetSliderUI : MonoBehaviour
{
    [SerializeField] private Slider betSlider;
    [SerializeField] private TMP_Text betText;

    void Start()
    {
        betSlider.minValue = 1;
        betSlider.maxValue = 20;
        betSlider.wholeNumbers = true;

        // ✅復元値反映
        betSlider.SetValueWithoutNotify(BetManager.Instance.CurrentBet);
        UpdateText(BetManager.Instance.CurrentBet);

        betSlider.onValueChanged.AddListener(OnSliderChanged);
    }


    void OnSliderChanged(float value)
    {
        int bet = Mathf.RoundToInt(value);

        BetManager.Instance.SetBet(bet);
        UpdateText(bet);
    }

    void UpdateText(int bet)
    {
        betText.text =
            string.Format(
        TextManager.Instance.GetUI("ui_mainmenu_2_9"),
        bet
    );
    }
}
