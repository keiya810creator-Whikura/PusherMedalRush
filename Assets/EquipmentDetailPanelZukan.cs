using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text;
using System.Collections.Generic;

public class EquipmentDetailPanelZukan : MonoBehaviour
{
    [Header("UI Parts")]
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text statText;

    [Header("Skill UI")]
    [SerializeField] private TMP_Text skillText;

    [Header("Rarity Color")]
    [SerializeField] private RarityColorTable rarityColorTable;
    [Header("Optional Roots")]
    [SerializeField] private GameObject iconRoot;
    //[SerializeField] private GameObject skillRoot;
    [Header("Skill Button List")]
    [SerializeField] private Transform skillButtonParent;
    [SerializeField] private SkillButtonUI skillButtonPrefab;
    void Awake()
    {
        gameObject.SetActive(false);
    }

    // ============================
    // ✅装備表示
    // ============================
    public void ShowSoubi(SoubiData data)
    {
        gameObject.SetActive(true);
        AudioManager.Instance.PlaySE(AudioManager.Instance.clickSE);

        iconRoot.SetActive(true);
        //skillRoot.SetActive(true);

        iconImage.sprite = data.icon;

        nameText.text = TextManager.Instance.GetEquip(data.nameKey);
        nameText.color = rarityColorTable.GetColor(data.rarity);

        statText.text = BuildStatText(data);

        // ✅ここが変更点
        RefreshSkillButtons(data.skills);

        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(statText.rectTransform);
    }

    private void RefreshSkillButtons(List<SkillData> skills)
    {
        if (!skillButtonParent || !skillButtonPrefab) return;

        // ✅既存削除
        foreach (Transform child in skillButtonParent)
            Destroy(child.gameObject);

                //skillRoot.SetActive(true);

        // ✅スキルボタン生成
        foreach (var skill in skills)
        {
            var btn = Instantiate(skillButtonPrefab, skillButtonParent);

            btn.Set(skill, clicked =>
            {
                SkillDetailPanelZukan.Instance.Show(clicked);
            });
        }
    }
    // ============================
    // ✅称号表示（同じPanelで流用）
    // ============================
    public void ShowTitle(SyougouData data)
    {
        gameObject.SetActive(true);

        AudioManager.Instance.PlaySE(AudioManager.Instance.clickSE);

        iconRoot.SetActive(false);
        //skillRoot.SetActive(false);

        nameText.text = TextManager.Instance.GetTitle(data.nameKey);
        nameText.color = rarityColorTable.GetColor(data.rarity);

        statText.text = BuildStatText(data);

        RefreshSkillButtons(data.skills);

        // ✅更新強制
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(statText.rectTransform);
    }


    // ============================
    // ✅ステータス表示（装備・称号共通）
    // ============================
    string BuildStatText(SoubiData data)
    {
        return BuildStatTextCore(
            data.attackAdd,
            data.attackRate,
            data.simultaneousShotRate,
            data.criticalAdd,
            data.criticalDamageAdd,
            data.attackMedal2xAdd,
            data.attackMedal5xAdd,
            data.attackMedal10xAdd,
            data.maxMedalAdd,
            data.maxMedalRate,
            data.noMedalConsumeAdd,
            data.recoveryWaveMedalValueAdd,
            data.recoveryWaveDurationAdd,
            data.goldDropAdd,
            data.equipmentDropAdd,
            data.titleDropAdd
        );
    }
    private const string PLUS_COLOR = "#FFD700";   // 金
    private const string MINUS_COLOR = "#FF4444";  // 赤
    string BuildStatText(SyougouData data)
    {
        return BuildStatTextCore(
            data.attackAdd,
            data.attackRate,
            data.simultaneousShotRate,
            data.criticalAdd,
            data.criticalDamageAdd,
            data.attackMedal2xAdd,
            data.attackMedal5xAdd,
            data.attackMedal10xAdd,
            data.maxMedalAdd,
            data.maxMedalRate,
            data.noMedalConsumeAdd,
            data.recoveryWaveMedalValueAdd,
            data.recoveryWaveDurationAdd,
            data.goldDropAdd,
            data.equipmentDropAdd,
            data.titleDropAdd
        );
    }

    string BuildStatTextCore(
    int attackAdd,
    float attackRate,
    float shotRate,
    float crit,
    float critDmg,
    float medal2x,
    float medal5x,
    float medal10x,
    int maxMedalAdd,
    float maxMedalRate,
    float noConsume,
    int recoveryValue,
    float recoveryTime,
    float gold,
    float equipDrop,
    float titleDrop)
    {
        StringBuilder sb = new StringBuilder();

        // =========================
        // ✅辞書キー対応 Add関数
        // =========================

        void AddIntKey(string key, int v)
        {
            if (v == 0) return;

            string label = TextManager.Instance.GetUI(key);

            string sign = v > 0 ? "+" : "-";
            string color = v > 0 ? PLUS_COLOR : MINUS_COLOR;

            sb.AppendLine(
                $"{label} <color={color}>{sign}{Mathf.Abs(v)}</color>"
            );
        }

        void AddPctKey(string key, float v)
        {
            if (Mathf.Approximately(v, 0)) return;

            string label = TextManager.Instance.GetUI(key);

            float percent = v * 100f;

            string sign = percent > 0 ? "+" : "-";
            string color = percent > 0 ? PLUS_COLOR : MINUS_COLOR;

            sb.AppendLine(
                $"{label} <color={color}>{sign}{Mathf.Abs(percent):F1}%</color>"
            );
        }

        void AddSecKey(string key, float v)
        {
            if (Mathf.Approximately(v, 0)) return;

            string label = TextManager.Instance.GetUI(key);

            string sign = v > 0 ? "+" : "-";
            string color = v > 0 ? PLUS_COLOR : MINUS_COLOR;

            sb.AppendLine(
                $"{label} <color={color}>{sign}{Mathf.Abs(v):F1}s</color>"
            );
        }

        // =========================
        // ✅火力系
        // =========================

        AddIntKey("ui_mainmenu_6_2", attackAdd);       // 攻撃力
        AddPctKey("ui_mainmenu_6_3", attackRate);      // 攻撃力倍率
        AddPctKey("ui_mainmenu_6_6", shotRate);        // 同時発射数

        AddPctKey("ui_mainmenu_6_7", crit);            // クリ率
        AddPctKey("ui_mainmenu_6_8", critDmg);         // クリ倍率

        // =========================
        // ✅攻撃メダル倍率
        // =========================

        AddPctKey("ui_mainmenu_6_12", medal2x);
        AddPctKey("ui_mainmenu_6_13", medal5x);
        AddPctKey("ui_mainmenu_6_14", medal10x);

        // =========================
        // ✅耐久系
        // =========================

        AddIntKey("ui_mainmenu_6_4", maxMedalAdd);     // 最大メダル
        AddPctKey("ui_mainmenu_6_5", maxMedalRate);    // 最大メダル倍率

        AddPctKey("ui_mainmenu_6_11", noConsume);      // メダル消費0確率

        // =========================
        // ✅回収系
        // =========================

        AddIntKey("ui_mainmenu_6_10", recoveryValue);  // 回収Waveメダル価値
        AddSecKey("ui_mainmenu_6_9", recoveryTime);    // 回収Wave秒

        // =========================
        // ✅経済系
        // =========================

        AddPctKey("ui_mainmenu_6_15", gold);           // ゴールドドロップ量

        // =========================
        // ✅収集系
        // =========================

        AddPctKey("ui_mainmenu_6_16", equipDrop);      // 装備ドロップ倍率
        AddPctKey("ui_mainmenu_6_17", titleDrop);      // 称号ドロップ倍率

        // =========================
        // ✅補正なし
        // =========================

        if (sb.Length == 0)
        {
            sb.AppendLine(TextManager.Instance.GetUI("ui_mainmenu_no_bonus"));
            // 例：ui_mainmenu_no_bonus = 補正なし / No Bonus
        }

        return sb.ToString();
    }

    // ============================
    // ✅スキル表示（skills List）
    // ============================
    string BuildSkillText(List<SkillData> skills)
    {
        if (skills == null || skills.Count == 0)
            return "スキル：なし";

        StringBuilder sb = new StringBuilder();
        sb.AppendLine("スキル");

        foreach (var s in skills)
        {
            sb.AppendLine($"・{s.nameKey}");
            sb.AppendLine($"{s.descriptionKey}");

            if (s.probability < 1f)
                sb.AppendLine($"発動率：{s.probability * 100f:F1}%");

            sb.AppendLine();
        }

        return sb.ToString();
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}
