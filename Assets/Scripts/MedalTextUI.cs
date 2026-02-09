using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class MedalTextUI : MonoBehaviour
{
    public static MedalTextUI Instance;

    public TextMeshProUGUI medalText;
    void Awake()
    {
        Instance = this;
    }
        void OnEnable()
    {
        MoneyManager.instance.OnMedalChanged += UpdateText;

        // 初期表示（シーン入った瞬間用）
        UpdateText(MoneyManager.instance.Medal);
    }
    private void Start()
    {
        UpdateText(BattleManager.Instance.Status.MaxMedal);
    }
    void OnDisable()
    {
        MoneyManager.instance.OnMedalChanged -= UpdateText;
    }

    public void UpdateText(long medal)
    {
        medalText.text = medal.ToString("N0");

    }
}
