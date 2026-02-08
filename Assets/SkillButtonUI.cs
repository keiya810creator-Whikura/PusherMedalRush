using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class SkillButtonUI : MonoBehaviour
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private Button button;

    private SkillData skill;

    public void Set(SkillData skill, Action<SkillData> onClick)
    {
        this.skill = skill;

        nameText.text =
            TextManager.Instance.GetSkill(skill.nameKey);

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() =>
        {
            onClick?.Invoke(skill);
        });
    }
}