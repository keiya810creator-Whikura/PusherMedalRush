using UnityEngine;

#region Enum Definitions

// ===============================
// 発動タイミング（いつ発動するか）
// ===============================
public enum SkillTriggerType
{
    Passive,            // 常時

    OnWaveStart,        // Wave開始時
    EveryXWave,         // 〇Waveごと

    OnEnemyDefeated,    // 敵撃破時

    OnMedalShot,        // 発射時
    OnMedalCollected,   // 獲得口

    Interval            // 〇秒ごとに発動
}

// ===============================
// スキルの処理内容（何をするか）
// ===============================
public enum SkillActionType
{
    AddStatus,          // ステータス加算

    SpawnMedal,         // メダル排出
    FireSpecialMedal,   // 特殊メダル発射

    ActivateSkill,      // アクティブスキルを発動する

    DoubleDropEquip,    // 装備2回
    DoubleDropTitle,    // 称号2回
}

// ===============================
// ステータス対象（AddStatus用）
// ===============================
public enum SkillTargetStatus
{
    Attack,
}

#endregion

// ===============================
// ✅SkillData 完全版
// ===============================
[CreateAssetMenu(menuName = "Game/Skill Data")]
public class SkillData : ScriptableObject
{
    // ===============================
    // 基本情報
    // ===============================

    [Header("Identity")]
    public int skillId;                  // ユニークID
    public string nameKey;               // 多言語キー
    public string descriptionKey;        // 多言語キー

    // ===============================
    // 発動条件
    // ===============================

    [Header("Trigger")]
    public SkillTriggerType triggerType;

    [Tooltip("○Waveごとに発動する場合の間隔")]
    public int waveInterval = 0;

    [Tooltip("○秒ごとに発動する場合の間隔")]
    public float timeInterval = 0f;

    // ===============================
    // スキル効果
    // ===============================

    [Header("Action")]
    public SkillActionType actionType;

    // ===============================
    // 効果量（量産パラメータ）
    // ===============================

    [Header("Effect Values")]

    [Tooltip("倍率効果（例：0.2 = +20%）")]
    public float percentValue = 0f;

    [Tooltip("固定値効果（例：+5、メダル10枚など）")]
    public int flatValue = 0;

    // ===============================
    // 確率系
    // ===============================

    [Header("Probability")]

    [Range(0f, 1f)]
    [Tooltip("発動確率（1.0 = 100%）")]
    public float probability = 1f;

    // ===============================
    // 特殊参照（アクティブ系）
    // ===============================

    [Header("Linked Active Skill (Optional)")]

    [Tooltip("WindCutterなどのアクティブスキル参照")]
    public ActiveSkillData activeSkill;
    [Header("Special Medal (Prefab Direct)")]
    public GameObject specialMedalPrefab;


    // ===============================
    // デバッグ用
    // ===============================

    public override string ToString()
    {
        return $"Skill[{skillId}] {nameKey} ({triggerType} → {actionType})";
    }
}