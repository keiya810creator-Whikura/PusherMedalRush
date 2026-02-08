using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Collections;
public class LocalizedTMPText : MonoBehaviour
{
    public enum DictionaryType
    {
        UI,
        Monster,
        Equip,
        Title
    }

    [Header("Dictionary")]
    public DictionaryType dictionaryType = DictionaryType.UI;

    [Header("Key")]
    public string key;

    private TMP_Text text;

    // ✅全LocalizedTMPTextを管理するリスト
    private static List<LocalizedTMPText> allTexts = new();

    private void Awake()
    {
        text = GetComponent<TMP_Text>();
    }

    private void OnEnable()
{
    if (!allTexts.Contains(this))
        allTexts.Add(this);

    StartCoroutine(WaitAndRefresh());
}

private IEnumerator WaitAndRefresh()
{
    while (TextManager.Instance == null)
        yield return null;

    Refresh();
}

    private void OnDisable()
    {
        // ✅リスト解除
        if (allTexts.Contains(this))
            allTexts.Remove(this);
    }

    // ✅辞書から取得して反映
    public void Refresh()
    {
        if (TextManager.Instance == null)
        {
            Debug.LogError("❌ TextManagerがシーンに存在しません");
            return;
        }

        if (string.IsNullOrEmpty(key))
        {
            text.text = "<NoKey>";
            return;
        }

        string value = dictionaryType switch
        {
            DictionaryType.UI => TextManager.Instance.GetUI(key),
            DictionaryType.Monster => TextManager.Instance.GetMonster(key),
            DictionaryType.Equip => TextManager.Instance.GetEquip(key),
            DictionaryType.Title => TextManager.Instance.GetTitle(key),
            _ => "<UnknownDict>"
        };

        text.text = value;
    }

    // ✅全UI一括更新（言語切り替え用）
    public static void RefreshAll()
    {
        foreach (var t in allTexts)
        {
            if (t != null)
                t.Refresh();
        }

        Debug.Log($"✅ RefreshAll 完了 : {allTexts.Count} texts updated");
    }
}