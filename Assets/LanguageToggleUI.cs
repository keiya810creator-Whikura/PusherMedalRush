using UnityEngine;
using UnityEngine.UI;

public class LanguageToggleUI : MonoBehaviour
{
    [SerializeField] private Toggle japaneseToggle;
    [SerializeField] private Toggle englishToggle;

    // ✅ タイトル背景切り替え
    [SerializeField] private TitleBackgroundSwitcher backgroundSwitcher;

    private bool isInitializing;

    private void Start()
    {
        isInitializing = true;

        // ✅保存されている言語で初期化
        RefreshToggleState();

        japaneseToggle.onValueChanged.AddListener(isOn =>
        {
            if (!isOn || isInitializing) return;

            SetLanguage(SystemLanguage.Japanese);
        });

        englishToggle.onValueChanged.AddListener(isOn =>
        {
            if (!isOn || isInitializing) return;

            SetLanguage(SystemLanguage.English);
        });

        isInitializing = false;
    }

    // -------------------------
    // 言語変更まとめ処理
    // -------------------------
    private void SetLanguage(SystemLanguage lang)
    {
        TextManager.Instance.SetLanguage(lang);

        // ✅背景も切り替え
        if (backgroundSwitcher != null)
            backgroundSwitcher.ApplyBackground();
    }

    private void RefreshToggleState()
    {
        var lang = TextManager.Instance.currentLanguage;

        japaneseToggle.isOn = (lang == SystemLanguage.Japanese);
        englishToggle.isOn = (lang == SystemLanguage.English);

        // ✅ 初期表示時も背景反映
        if (backgroundSwitcher != null)
            backgroundSwitcher.ApplyBackground();
    }
}
