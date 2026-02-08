using UnityEngine;
using System.Collections.Generic;

// すべて「加算倍率」
// final = base * (1f+ Soubi + shogo)

[CreateAssetMenu(menuName = "Game/Syougou Data")]
public class SyougouData : ScriptableObject
{
    [Header("Basic Info")]
    public string id;          // CSV / 内部管理用
    public string nameKey;     // 多言語対応
    public Sprite icon;

    [Range(1, 6)]
    public int rarity;

    // =========================
    // 戦闘・火力系
    // =========================

    [Header("火力系")]
    [Tooltip("攻撃力倍率（+0.2 = +20%）")]
    public int attackAdd;        // +3
    public float attackRate;     // +0.2 (=20%)

    [Tooltip("同時発射数倍率")]
    public float simultaneousShotRate;

    [Tooltip("クリティカル率")]
    public float criticalAdd;

    [Tooltip("クリティカル倍率(加算)")]
    public float criticalDamageAdd;

    // 攻撃力メダル系
    [Header("攻撃力倍率メダル出現率")]
    public float attackMedal2xAdd;
    public float attackMedal5xAdd;
    public float attackMedal10xAdd;

    // =========================
    // 生存・リソース系
    // =========================

    [Header("耐久力")]
    [Tooltip("総メダル数（HP）倍率")]
    public int maxMedalAdd;      // +20
    public float maxMedalRate;   // +0.15

    [Tooltip("メダル消費無し確率")]
    public float noMedalConsumeAdd;

    // =========================
    // 回収・経済系
    // =========================

    [Header("回収・経済")]
    [Tooltip("回収Wave時メダル価値")]
    public int recoveryWaveMedalValueAdd;

    [Tooltip("回収Wave延長秒")]
    public int recoveryWaveDurationAdd;

    [Tooltip("ゴールドドロップ量倍率(加算)")]
    public float goldDropAdd;

    // =========================
    // ハクスラ・収集系
    // =========================

    [Header("ハクスラ・収集")]
    [Tooltip("装備ドロップ率倍率(加算)")]
    public float equipmentDropAdd;

    [Tooltip("称号ドロップ率倍率(加算)")]
    public float titleDropAdd;

    // =========================
    // スキル
    // =========================
    [Tooltip("スキル")]
    public List<SkillData> skills;
}
