using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Text/Text Dictionary")]
public class TextDictionarySO : ScriptableObject
{
    [SerializeField]
    public List<TextEntry> entries = new List<TextEntry>();

    // 外に出さない（重要）
    private Dictionary<string, TextEntry> _dict;

    // =========================
    // 辞書構築
    // =========================
    public void BuildDictionary()
    {
        // ✅ 参照を切らない
        if (_dict == null)
            _dict = new Dictionary<string, TextEntry>();
        else
            _dict.Clear();

        foreach (var entry in entries)
        {
            if (string.IsNullOrEmpty(entry.key))
                continue;

            // 後勝ち or 先勝ちを明示（ここでは先勝ち）
            if (!_dict.ContainsKey(entry.key))
            {
                _dict.Add(entry.key, entry);
            }
            else
            {
                Debug.LogWarning(
                    $"[TextDictionarySO] Duplicate key ignored: {entry.key}",
                    this
                );
            }
        }
    }

    // =========================
    // 取得
    // =========================
    public string Get(string key, SystemLanguage language)
    {
        if (string.IsNullOrEmpty(key))
            return "<NullKey>";

        // ✅ 未初期化なら構築
        if (_dict == null || _dict.Count == 0)
            BuildDictionary();

        if (!_dict.TryGetValue(key, out var entry))
        {
            return $"<Missing:{key}>";
        }

        return language switch
        {
            SystemLanguage.Japanese => entry.ja,
            _ => entry.en
        };
    }

#if UNITY_EDITOR
    // =========================
    // Editor上での安全対策
    // =========================
    private void OnValidate()
    {
        // Inspector編集時も参照を切らず再構築
        BuildDictionary();
    }
#endif
}
