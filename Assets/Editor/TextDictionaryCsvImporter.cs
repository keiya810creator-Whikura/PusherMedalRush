#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;

public static class TextDictionaryCsvImporter
{
    [MenuItem("Tools/Text/Import CSV to TextDictionary")]
    public static void ImportCsv()
    {
        string csvPath = EditorUtility.OpenFilePanel("CSVを選択", "", "csv");
        if (string.IsNullOrEmpty(csvPath)) return;

        string assetPath = EditorUtility.SaveFilePanelInProject(
            "TextDictionarySO 保存先",
            "TextDictionary",
            "asset",
            ""
        );
        if (string.IsNullOrEmpty(assetPath)) return;

        var so = ScriptableObject.CreateInstance<TextDictionarySO>();

        var lines = File.ReadAllLines(csvPath);

        // 1行目はヘッダなのでスキップ
        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            var cols = lines[i].Split(',');

            if (cols.Length < 3) continue;

            so.entries.Add(new TextEntry
            {
                key = cols[0],
                ja = cols[1],
                en = cols[2]
            });
        }

        AssetDatabase.CreateAsset(so, assetPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("TextDictionary CSV Import 完了");
    }
}
#endif
