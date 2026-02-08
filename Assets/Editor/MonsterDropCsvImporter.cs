#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;

public static class MonsterDropCsvImporter
{
    private const string CSV_PATH = "Assets/Data/CSV/モンスタードロップ.csv";

    [MenuItem("Tools/Import Monster Drop CSV")]
    public static void Import()
    {
        if (!File.Exists(CSV_PATH))
        {
            Debug.LogError($"CSV not found: {CSV_PATH}");
            return;
        }

        // ===== 全SOを取得 =====
        var monsterDict = AssetDatabase.FindAssets("t:MonsterData")
            .Select(guid => AssetDatabase.LoadAssetAtPath<MonsterData>(
                AssetDatabase.GUIDToAssetPath(guid)))
            .Where(m => m != null)
            .ToDictionary(m => m.id);

        var soubiDict = AssetDatabase.FindAssets("t:SoubiData")
            .Select(guid => AssetDatabase.LoadAssetAtPath<SoubiData>(
                AssetDatabase.GUIDToAssetPath(guid)))
            .Where(s => s != null)
            .ToDictionary(s => s.id);

        var syougouDict = AssetDatabase.FindAssets("t:SyougouData")
            .Select(guid => AssetDatabase.LoadAssetAtPath<SyougouData>(
                AssetDatabase.GUIDToAssetPath(guid)))
            .Where(s => s != null)
            .ToDictionary(s => s.id);

        var lines = File.ReadAllLines(CSV_PATH);

        foreach (var line in lines.Skip(1))
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            var c = line.Split(',');

            string monsterId = c[0];
            string soubiId = c[1];
            string syougouId = c[2];

            if (!monsterDict.TryGetValue(monsterId, out var monster))
            {
                Debug.LogWarning($"Monster not found: {monsterId}");
                continue;
            }

            // ===== クリア =====
            monster.dropSoubiList.Clear();
            monster.dropShogoList.Clear();

            // ===== 装備 =====
            if (!string.IsNullOrWhiteSpace(soubiId) &&
                soubiDict.TryGetValue(soubiId, out var soubi))
            {
                monster.dropSoubiList.Add(soubi);
            }

            // ===== 称号 =====
            if (!string.IsNullOrWhiteSpace(syougouId) &&
                syougouDict.TryGetValue(syougouId, out var syougou))
            {
                monster.dropShogoList.Add(syougou);
            }

            EditorUtility.SetDirty(monster);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Monster Drop CSV Import Completed");
    }
}
#endif
