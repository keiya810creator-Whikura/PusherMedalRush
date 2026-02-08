#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;
using System.Globalization;

public static class MonsterCsvReader
{
    private const string CSV_PATH = "Assets/Data/CSV/モンスター.csv";
    private const string ASSET_DIR = "Assets/Data/モンスター/";

    [MenuItem("Tools/Import Monster CSV")]
    public static void Import()
    {
        if (!File.Exists(CSV_PATH))
        {
            Debug.LogError($"CSV not found: {CSV_PATH}");
            return;
        }

        var lines = File.ReadAllLines(CSV_PATH);

        foreach (var line in lines.Skip(1))
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            var c = line.Split(',');

            // CSV列対応
            // 0: 名前（未使用）
            string id = c[1];
            string nameKey = c[2];
            float soubiDropRatePercent = ParseFloat(c[3]);
            float syougouDropRatePercent = ParseFloat(c[4]);
            string stageId = c[5];
            int minWave = int.Parse(c[6]);
            int maxWave = int.Parse(c[7]);
            int syutugennritu = int.Parse(c[8]);
            bool isBoss = ParseBool(c[9]);

            string assetPath = $"{ASSET_DIR}{id}.asset";
            // ★ フォルダが無ければ作成
            if (!AssetDatabase.IsValidFolder("Assets/Data"))
            {
                AssetDatabase.CreateFolder("Assets", "Data");
            }

            if (!AssetDatabase.IsValidFolder("Assets/Data/モンスター"))
            {
                AssetDatabase.CreateFolder("Assets/Data", "モンスター");
            }

            MonsterData data =
                AssetDatabase.LoadAssetAtPath<MonsterData>(assetPath);

            if (data == null)
            {
                data = ScriptableObject.CreateInstance<MonsterData>();
                data.id = id;
                AssetDatabase.CreateAsset(data, assetPath);
            }

            // ===== 反映 =====
            data.nameKey = nameKey;
            data.stageId = stageId;
            data.minWave = minWave;
            data.maxWave = maxWave;
            data.syutugennritu = syutugennritu;
            data.isBoss = isBoss;

            // % → 0–1 変換
            data.soubiDropChance = soubiDropRatePercent / 100f;
            data.syougouDropChance = syougouDropRatePercent / 100f;

            EditorUtility.SetDirty(data);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Monster CSV Import Completed");
    }

    // =========================
    // Utility
    // =========================

    private static bool ParseBool(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;

        value = value.Trim().ToLower();
        return value == "true" || value == "1";
    }

    private static float ParseFloat(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return 0f;

        return float.Parse(value, CultureInfo.InvariantCulture);
    }
}
#endif
