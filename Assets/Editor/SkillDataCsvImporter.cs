#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;

public class SkillDataCsvImporter
{
    [MenuItem("Tools/Update SkillData From CSV")]
    public static void ImportSkillData()
    {
        string csvPath = "Assets/Data/CSV/スキル.csv";

        if (!File.Exists(csvPath))
        {
            Debug.LogError("CSVが見つかりません: " + csvPath);
            return;
        }

        string outputFolder = "Assets/Data/スキル/";
        Directory.CreateDirectory(outputFolder);

        string[] lines = File.ReadAllLines(csvPath, Encoding.UTF8);

        string[] header = lines[0].Split(',');
        int idIndex = System.Array.IndexOf(header, "ID");

        if (idIndex < 0)
        {
            Debug.LogError("CSVに ID 列が見つかりません！");
            return;
        }

        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            string[] cols = lines[i].Split(',');
            if (cols.Length <= idIndex) continue;

            int skillId = int.Parse(cols[idIndex + 0]);

            // ✅既存アセットを探す
            string assetPath = outputFolder + $"Skill_{skillId}.asset";
            SkillData skill = AssetDatabase.LoadAssetAtPath<SkillData>(assetPath);

            // ✅なければ新規作成
            if (skill == null)
            {
                skill = ScriptableObject.CreateInstance<SkillData>();
                AssetDatabase.CreateAsset(skill, assetPath);
                Debug.Log($"🆕 新規作成: {assetPath}");
            }
            else
            {
                Debug.Log($"♻ 更新: {assetPath}");
            }

            // ✅中身を上書き（参照は維持される）
            skill.skillId = skillId;
            skill.nameKey = cols[idIndex + 1];
            skill.descriptionKey = cols[idIndex + 2];

            skill.triggerType = ConvertTrigger(cols[idIndex + 3]);
            skill.actionType = ConvertAction(cols[idIndex + 4]);

            skill.timeInterval = ParseFloat(cols[idIndex + 5]);
            skill.waveInterval = ParseInt(cols[idIndex + 6]);

            skill.percentValue = ParseFloat(cols[idIndex + 7]);
            skill.flatValue = ParseInt(cols[idIndex + 8]);

            skill.probability = ParseFloat(cols[idIndex + 9], 1f);

            // ✅Unityに更新通知
            EditorUtility.SetDirty(skill);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("✅SkillData 更新完了（参照は切れません）");
    }

    static SkillTriggerType ConvertTrigger(string raw)
    {
        return raw switch
        {
            "敵撃破時" => SkillTriggerType.OnEnemyDefeated,
            "発射時" => SkillTriggerType.OnMedalShot,
            "獲得口" => SkillTriggerType.OnMedalCollected,
            "Wave開始時" => SkillTriggerType.OnWaveStart,
            "〇Waveごと" => SkillTriggerType.EveryXWave,
            "〇秒ごとに発動" =>SkillTriggerType.Interval,
            _ => SkillTriggerType.Passive
        };
    }

    static SkillActionType ConvertAction(string raw)
    {
        return raw switch
        {
            "メダル排出" => SkillActionType.SpawnMedal,
            "特殊メダル発射" => SkillActionType.FireSpecialMedal,
            "装備〇回" => SkillActionType.DoubleDropEquip,
            "称号〇回" => SkillActionType.DoubleDropTitle,
            _ => SkillActionType.AddStatus
        };
    }

    static int ParseInt(string raw, int defaultValue = 0)
    {
        if (int.TryParse(raw, out int v)) return v;
        return defaultValue;
    }

    static float ParseFloat(string raw, float defaultValue = 0f)
    {
        if (float.TryParse(raw, out float v)) return v;
        return defaultValue;
    }
}
#endif