using UnityEngine;

public class TextManager : MonoBehaviour
{
    public static TextManager Instance;

    public SystemLanguage currentLanguage;

    [Header("Dictionaries")]
    public TextDictionarySO monsterDict;
    public TextDictionarySO soubiDict;
    public TextDictionarySO syougouDict;
    public TextDictionarySO skillDict;
    public TextDictionarySO uiDict;

    // ✅保存キー
    private const string LANG_SAVE_KEY = "GAME_LANGUAGE";

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        //DontDestroyOnLoad(gameObject);

        // ✅言語ロード（保存があれば優先）
        LoadLanguage();

        // ✅辞書を初期化
        monsterDict.BuildDictionary();
        soubiDict.BuildDictionary();
        syougouDict.BuildDictionary();
        skillDict.BuildDictionary();
        uiDict.BuildDictionary();
    }

    // ✅言語変更（外部から呼ぶ）
    public void SetLanguage(SystemLanguage lang)
    {
        currentLanguage = lang;

        // ✅保存する
        SaveLanguage();

        // ✅全UI更新
        LocalizedTMPText.RefreshAll();
    }

    // ✅保存
    private void SaveLanguage()
    {
        PlayerPrefs.SetInt(LANG_SAVE_KEY, (int)currentLanguage);
        PlayerPrefs.Save();

        Debug.Log($"✅ Language Saved: {currentLanguage}");
    }

    // ✅ロード
    private void LoadLanguage()
    {
        if (PlayerPrefs.HasKey(LANG_SAVE_KEY))
        {
            currentLanguage = (SystemLanguage)PlayerPrefs.GetInt(LANG_SAVE_KEY);
            Debug.Log($"✅ Language Loaded: {currentLanguage}");
        }
        else
        {
            // 初回はOS言語
            currentLanguage = Application.systemLanguage;
            Debug.Log($"🌍 Default Language: {currentLanguage}");
        }
    }

    // -------------------------
    // Get 系
    // -------------------------

    public string GetMonster(string key)
        => monsterDict.Get(key, currentLanguage);

    public string GetEquip(string key)
        => soubiDict.Get(key, currentLanguage);

    public string GetTitle(string key)
        => syougouDict.Get(key, currentLanguage);

    public string GetSkill(string key)
        => skillDict.Get(key, currentLanguage);

    public string GetUI(string key)
        => uiDict.Get(key, currentLanguage);
}