using UnityEngine;
using TMPro;

public class SkillDetailPanel : MonoBehaviour
{
    public static SkillDetailPanel Instance;

    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text descText;

    private void Awake()
    {
        Instance = this;
        gameObject.SetActive(false);
    }

    public void Show(SkillData skill)
    {
        nameText.text =
            TextManager.Instance.GetSkill(skill.nameKey);

        descText.text =
            TextManager.Instance.GetSkill(skill.descriptionKey);

        gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}