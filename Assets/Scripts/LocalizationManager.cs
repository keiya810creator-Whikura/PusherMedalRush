using UnityEngine;
using System.Collections.Generic;
public enum LanguageType
{
    Japanese,
    English
}

public class LocalizationManager : MonoBehaviour
{
    public static LocalizationManager Instance;

    [SerializeField] private LanguageType currentLanguage = LanguageType.Japanese;

    private Dictionary<string, string> japaneseTexts = new Dictionary<string, string>()
    {
        { "wave", "WAVE {0}" },
        { "recovery", "‰ñŽûƒ^ƒCƒ€ {0}" }
    };

    private Dictionary<string, string> englishTexts = new Dictionary<string, string>()
    {
        { "wave", "WAVE {0}" },
        { "recovery", "RECOVERY {0}" }
    };

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        //DontDestroyOnLoad(gameObject);
    }

    public string GetText(string key, params object[] args)
    {
        string baseText = key;

        Dictionary<string, string> table =
            currentLanguage == LanguageType.Japanese
            ? japaneseTexts
            : englishTexts;

        if (table.TryGetValue(key, out baseText))
        {
            return string.Format(baseText, args);
        }

        return key;
    }

    public void SetLanguage(LanguageType language)
    {
        currentLanguage = language;
    }
}
