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


    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (!PlayerPrefs.HasKey("Language"))
        {
            var sysLang = Application.systemLanguage;

            if (sysLang == SystemLanguage.Japanese)
                SetLanguage(LanguageType.Japanese);
            else
                SetLanguage(LanguageType.English);

            PlayerPrefs.SetInt("Language", (int)currentLanguage);
        }
        else
        {
            currentLanguage = (LanguageType)PlayerPrefs.GetInt("Language");
        }
    }

    public void SetLanguage(LanguageType language)
    {
        currentLanguage = language;
    }
}
