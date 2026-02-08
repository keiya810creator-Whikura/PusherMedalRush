using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(menuName = "Game/Monster Data")]
public class MonsterData : ScriptableObject
{
    [Header("Basic Info")]
    public string id;              // 例: "m_9"
    public string nameKey;
    public string stageId;

    public Sprite icon;
    public GameObject prefab;

    [Header("ドロップ装備")]
    public List<SoubiData> dropSoubiList = new();

    [Header("ドロップ称号")]
    public List<SyougouData> dropShogoList = new();

    [Header("ドロップ率")]
    [Range(0f, 1f)]
    public float soubiDropChance = 0.1f;

    [Range(0f, 1f)]
    public float syougouDropChance = 0.02f;

    [Header("出現ウェーブ")]
    public int minWave = 1;
    public int maxWave = 9999;

    [Header("出現率")]
    public int syutugennritu;

    [Header("出現ステージ")] 
    public string syutugennStage;
    [Header("ボス")]
    public bool isBoss;

#if UNITY_EDITOR
    private void OnValidate()
    {
        // ✅ idが空なら終了
        if (string.IsNullOrEmpty(id)) return;

        // ✅ "m_" を除去する
        string numberPart = id.Replace("m_", "");

        // ✅ 数字として変換できなければ終了
        if (!int.TryParse(numberPart, out int monsterNumber)) return;

        if (monsterNumber <= 0) return;

        string fileName = monsterNumber.ToString();

        // ✅ Sprite自動取得（フォルダ固定）
        if (icon == null)
        {
            icon = FindAssetInFolder<Sprite>(
                fileName,
                "Assets/素材/モンスター"
            );
        }

        // ✅ Prefab自動取得（フォルダ固定）
        if (prefab == null)
        {
            prefab = FindAssetInFolder<GameObject>(
                fileName,
                "Assets/Prefab/Enemy"
            );
        }

        EditorUtility.SetDirty(this);
    }

    /// <summary>
    /// ✅指定フォルダ内だけで検索する（誤爆ゼロ）
    /// </summary>
    private T FindAssetInFolder<T>(string assetName, string folderPath)
        where T : Object
    {
        string filter = $"{assetName} t:{typeof(T).Name}";
        string[] guids = AssetDatabase.FindAssets(filter, new[] { folderPath });

        if (guids.Length == 0)
        {
            Debug.LogWarning(
                $"❌ {typeof(T).Name} '{assetName}' が {folderPath} に見つかりません"
            );
            return null;
        }

        string path = AssetDatabase.GUIDToAssetPath(guids[0]);
        return AssetDatabase.LoadAssetAtPath<T>(path);
    }
#endif
}
