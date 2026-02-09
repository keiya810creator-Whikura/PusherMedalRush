using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public static SkillManager Instance;

    // ===============================
    // ✅装備中スキル一覧
    // ===============================
    private List<SkillData> equippedSkills = new();

    // Intervalスキル用
    private Dictionary<SkillData, float> intervalTimers = new();

    // ===============================
    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // ✅装備変更イベント購読
        if (InventoryManager.Instance != null)
            InventoryManager.Instance.OnEquipmentChanged += RefreshEquippedSkills;

        // ✅初回ロード
        RefreshEquippedSkills();
    }

    // ===============================
    // ✅装備中Skillを再構築する
    // ===============================
    public void RefreshEquippedSkills()
    {
        equippedSkills.Clear();
        intervalTimers.Clear();

        // ✅ID管理用
        HashSet<int> addedIds = new();

        var equippedSoubi = InventoryManager.Instance.soubiList
            .Where(s => s.isEquipped)
            .ToList();

        foreach (var soubi in equippedSoubi)
        {
            if (soubi.master == null) continue;
            if (soubi.master.skills == null) continue;

            foreach (var skill in soubi.master.skills)
            {
                if (skill == null) continue;

                // ✅同じskillIdは1回だけ
                if (addedIds.Contains(skill.skillId))
                    continue;

                addedIds.Add(skill.skillId);

                equippedSkills.Add(skill);

                // ✅IntervalならTimer登録
                if (skill.triggerType == SkillTriggerType.Interval)
                {
                    intervalTimers[skill] = 0f;
                }
            }
        }

        //Debug.Log($"✅重複排除後スキル数：{equippedSkills.Count}");
    }

    // ===============================
    // ✅Interval処理
    // ===============================
    void Update()
    {
        HandleIntervalSkills();
    }

    private void HandleIntervalSkills()
    {
        if (intervalTimers.Count == 0) return;

        foreach (var skill in intervalTimers.Keys.ToList())
        {
            intervalTimers[skill] += Time.deltaTime;

            if (intervalTimers[skill] >= skill.timeInterval)
            {
                intervalTimers[skill] = 0f;
                TryActivateSkill(skill);
            }
        }
    }

    // ===============================
    // ✅Trigger API（外部から呼ぶ）
    // ===============================

    public void TriggerWaveStart(int wave)
    {
        foreach (var skill in equippedSkills)
        {
            if (skill.triggerType == SkillTriggerType.OnWaveStart)
                TryActivateSkill(skill);

            if (skill.triggerType == SkillTriggerType.EveryXWave)
            {
                if (skill.waveInterval > 0 &&
                    wave % skill.waveInterval == 0)
                    TryActivateSkill(skill);
            }
        }
    }

    public void TriggerEnemyDefeated()
    {
        ExecuteTrigger(SkillTriggerType.OnEnemyDefeated);
    }

    public void TriggerMedalShot()
    {
        ExecuteTrigger(SkillTriggerType.OnMedalShot);
    }

    public void TriggerMedalCollected(Medal medal)
    {
        ExecuteTrigger(SkillTriggerType.OnMedalCollected);

        // ✅特殊メダルなら linkedSkill 発動
        if (medal != null && medal.linkedSkill != null)
        {
            ActivateActiveSkill(medal.linkedSkill);
        }
    }

    private void ExecuteTrigger(SkillTriggerType trigger)
    {
        foreach (var skill in equippedSkills)
        {
            if (skill.triggerType == trigger)
                TryActivateSkill(skill);
        }
    }

    // ===============================
    // ✅確率判定つき発動
    // ===============================
    private void TryActivateSkill(SkillData skill)
    {
        if (skill == null) return;

        // ✅確率判定
        if (Random.value > skill.probability)
            return;

        //SkillToastManager.Instance.ShowSkillToast(TextManager.Instance.GetSkill(skill.nameKey), 1f, () => true);
        ExecuteSkill(skill);
    }

    // ===============================
    // ✅スキル効果本体
    // ===============================
    private void ExecuteSkill(SkillData skill)
    {
        switch (skill.actionType)
        {
            case SkillActionType.AddStatus:
                ApplyStatusBonus(skill);
                SkillToastManager.Instance.ShowSkillToast(TextManager.Instance.GetSkill(skill.nameKey), 0.3f, () => true);
                break;

            case SkillActionType.FireSpecialMedal:
                // ✅特殊弾は MedalGenerator.Fire() 側で抽選するだけ
                break;

            case SkillActionType.ActivateSkill:
                ActivateActiveSkill(skill.activeSkill);
                break;

            case SkillActionType.SpawnMedal:
                //Debug.Log($"🪙スキル排出：{skill.flatValue}枚");
                MedalGenerator.Instance?.SpawnMedals(skill.flatValue);
                break;
        }
    }

    // ===============================
    // ✅ドロップ判定回数増加
    // ===============================
    public int GetEquipDropRollCount()
    {
        int extra = 0;

        foreach (var skill in equippedSkills)
        {
            if (skill.actionType != SkillActionType.DoubleDropEquip)
                continue;

            if (Random.value < skill.probability)
            {
                extra += skill.flatValue;
                AudioManager.Instance.PlaySE(AudioManager.Instance.skillHatudou);
                SkillToastManager.Instance.ShowSkillToast(TextManager.Instance.GetSkill(skill.nameKey), 0.3f, () => true);
            }
        }

        return 1 + extra;
    }

    public int GetTitleDropRollCount()
    {
        int extra = 0;

        foreach (var skill in equippedSkills)
        {
            if (skill.actionType != SkillActionType.DoubleDropTitle)
                continue;

            if (Random.value < skill.probability)
            {
                extra += skill.flatValue;
                AudioManager.Instance.PlaySE(AudioManager.Instance.skillHatudou);
                SkillToastManager.Instance.ShowSkillToast(TextManager.Instance.GetSkill(skill.nameKey), 0.3f, () => true);
            }
        }

        return 1 + extra;
    }

    // ===============================
    // ✅ステータス強化（Battle開始時に反映）
    // ===============================
    private void ApplyStatusBonus(SkillData skill)
    {
        AudioManager.Instance.PlaySE(AudioManager.Instance.skillHatudou);

        BattleManager.Instance.AddAttackRate(skill.percentValue);
    }

    // ===============================
    // ✅アクティブスキル発動
    // ===============================
    private void ActivateActiveSkill(ActiveSkillData activeSkill)
    {
        if (activeSkill == null) return;

        AudioManager.Instance.PlaySE(AudioManager.Instance.skillHatudou);

        activeSkill.Execute();
    }

    // ===============================
    // ✅特殊弾スキル抽選用
    // ===============================
    public SkillData GetRandomFireSpecialSkill()
    {
        var list = equippedSkills
            .FindAll(s => s.actionType == SkillActionType.FireSpecialMedal);

        if (list.Count == 0) return null;

        return list[Random.Range(0, list.Count)];
    }
    // ===============================
    // ✅特殊弾スキル抽選（確率込み）
    // ===============================
    public SkillData TryGetFireSpecialSkill()
    {
        var list = equippedSkills
            .Where(s => s.actionType == SkillActionType.FireSpecialMedal)
            .ToList();

        if (list.Count == 0) return null;

        // ✅全スキルを順番に判定
        foreach (var skill in list)
        {
            if (skill.specialMedalPrefab == null) continue;

            if (Random.value < skill.probability)
            {
                return skill; // ✅当選したら即返す
            }
        }

        return null; // ✅全部外れた
    }
}