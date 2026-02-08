#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;
using System.Globalization;

public static class SyougouCsvImporter
{
    private const string CSV_PATH = "Assets/Data/CSV/称号.csv";
    private const string ASSET_DIR = "Assets/Data/称号/";

    [MenuItem("Tools/Import Syougou CSV")]
    public static void Import()
    {
        // ===== フォルダ保証 =====
        EnsureFolder("Assets/Data");
        EnsureFolder("Assets/Data/称号");

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

            string id = c[1];
            string nameKey = c[2];
            int rarity = ParseInt(c[3]);

            string assetPath = $"{ASSET_DIR}{id}.asset";
            SyougouData data =
                AssetDatabase.LoadAssetAtPath<SyougouData>(assetPath);

            if (data == null)
            {
                data = ScriptableObject.CreateInstance<SyougouData>();
                data.id = id;
                AssetDatabase.CreateAsset(data, assetPath);
            }

            // ===== 基本 =====
            data.nameKey = nameKey;
            data.rarity = rarity;

            // ===== 火力系 =====
            data.attackAdd = ParseInt(c[4]);
            data.attackRate = ParseFloat(c[5]);
            data.simultaneousShotRate = ParseFloat(c[6]);
            data.criticalAdd = ParseFloat(c[7]);
            data.criticalDamageAdd = ParseFloat(c[8]);

            data.attackMedal2xAdd = ParseFloat(c[9]);
            data.attackMedal5xAdd = ParseFloat(c[10]);
            data.attackMedal10xAdd = ParseFloat(c[11]);

            // ===== 耐久 =====
            data.maxMedalAdd = ParseInt(c[12]);
            data.maxMedalRate = ParseFloat(c[13]);
            data.noMedalConsumeAdd = ParseFloat(c[14]);

            // ===== 回収・経済 =====
            data.recoveryWaveMedalValueAdd = ParseInt(c[15]);
            data.recoveryWaveDurationAdd = ParseInt(c[16]);
            data.goldDropAdd = ParseFloat(c[17]);

            // ===== ハクスラ =====
            data.equipmentDropAdd = ParseFloat(c[18]);
            data.titleDropAdd = ParseFloat(c[19]);

            EditorUtility.SetDirty(data);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Syougou CSV Import Completed");
    }

    // =========================
    // Utility
    // =========================

    private static void EnsureFolder(string path)
    {
        if (AssetDatabase.IsValidFolder(path))
            return;

        string parent = Path.GetDirectoryName(path);
        string folder = Path.GetFileName(path);
        AssetDatabase.CreateFolder(parent, folder);
    }

    private static int ParseInt(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return 0;
        return int.Parse(value);
    }

    private static float ParseFloat(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return 0f;
        return float.Parse(value, CultureInfo.InvariantCulture);
    }
}
#endif
