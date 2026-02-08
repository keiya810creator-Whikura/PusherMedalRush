using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class GoldTextUI : MonoBehaviour
{
    public TextMeshProUGUI goldText;

    void OnEnable()
    {
        MoneyManager.instance.OnGoldChanged += UpdateText;

        // 初期表示（シーン入った瞬間用）
        UpdateText(MoneyManager.instance.Gold);
    }

    void OnDisable()
    {
        MoneyManager.instance.OnGoldChanged -= UpdateText;
    }

    void UpdateText(long gold)
    {
        goldText.text = gold.ToString("N0");
    }
}
