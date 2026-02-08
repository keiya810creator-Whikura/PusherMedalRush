using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(menuName = "Game/Soubi Data")]
public class SoubiData : ScriptableObject
{
    [Header("Basic Info")]
    public string id;          // 例: "s_9"
    public string nameKey;
    public Sprite icon;

    [Range(1, 6)]
    public int rarity;

    // =========================
    // 火力系
    // =========================

    [Header("火力系")]
    public int attackAdd;
    public float attackRate;

    public float simultaneousShotRate;
    public float criticalAdd;
    public float criticalDamageAdd;

    [Header("攻撃力倍率メダル出現率")]
    public float attackMedal2xAdd;
    public float attackMedal5xAdd;
    public float attackMedal10xAdd;

    // =========================
    // 耐久力
    // =========================

    [Header("耐久力")]
    public int maxMedalAdd;
    public float maxMedalRate;

    public float noMedalConsumeAdd;

    // =========================
    // 回収・経済
    // =========================

    [Header("回収・経済")]
    public int recoveryWaveMedalValueAdd;
    public int recoveryWaveDurationAdd;
    public float goldDropAdd;

    // =========================
    // 収集
    // =========================

    [Header("ハクスラ・収集")]
    public float equipmentDropAdd;
    public float titleDropAdd;

    // =========================
    // スキル
    // =========================

    public List<SkillData> skills;



#if UNITY_EDITOR
    private void OnValidate()
    {
        if (string.IsNullOrEmpty(id)) return;

        // ✅ iconがもうあるなら何もしない
        if (icon != null) return;

        // ✅ "s_" を除去して数字だけにする
        string numberPart = id.Replace("s_", "");

        // ✅ 数字に変換できなければ終了
        if (!int.TryParse(numberPart, out int equipNumber)) return;

        string spriteName = equipNumber.ToString();

        // ✅ Sprite検索（Assets/素材/装備限定）
        icon = FindSpriteInFolder(spriteName, "Assets/素材/装備");

        EditorUtility.SetDirty(this);
    }

    /// <summary>
    /// ✅数字名Spriteを指定フォルダから探す
    /// </summary>
    private Sprite FindSpriteInFolder(string spriteName, string folderPath)
    {
        string filter = $"{spriteName} t:Sprite";
        string[] guids = AssetDatabase.FindAssets(filter, new[] { folderPath });

        if (guids.Length == 0)
        {
            Debug.LogWarning($"❌ Sprite '{spriteName}' が {folderPath} に見つかりません");
            return null;
        }

        string path = AssetDatabase.GUIDToAssetPath(guids[0]);
        return AssetDatabase.LoadAssetAtPath<Sprite>(path);
    }
#endif
}
