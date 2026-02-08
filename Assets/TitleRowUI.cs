using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Text;

public class TitleRowUI : MonoBehaviour
{
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text statusText;
    [SerializeField] TMP_Text countText;
    [SerializeField] Button button;

    private SyougouData data;
    private Action<SyougouData> onClick;
    private void AddLine(StringBuilder sb, string label, float value)
    {
        if (Mathf.Abs(value) > 0.0001f)
            sb.Append($"{label}+{value:F1} ");
    }
    public enum TitleSelectMode
    {
        Equip,      // 称号を付与する
        Remove      // 称号を外す（必要なら）
    }

    public void Set(
    InventoryManager.SyougouStack stack,
    Action<SyougouData> onClick,
    RarityColorTable rarityTable,
    TitleSelectPanel.TitleSelectMode mode
)
    {
        data = stack.data;
        this.onClick = onClick;

        // ✅称号名（翻訳）
        string titleName = TextManager.Instance.GetTitle(data.nameKey);
        nameText.text = $"《{titleName}》";
        nameText.color = rarityTable.GetColor(data.rarity);

        // ✅所持数
        countText.text = $"×{stack.count}";

        // ✅ステータス表示（翻訳済み）
        statusText.text = BuildStatusText(data);

        bool canAttach = mode == TitleSelectPanel.TitleSelectMode.Inventory;
        button.gameObject.SetActive(canAttach);

        button.onClick.RemoveAllListeners();
        if (canAttach)
        {
            button.onClick.AddListener(() =>
            {
                this.onClick?.Invoke(data);
            });
        }
    }


    // -------------------------------
    // ステータス文字列生成（0は出さない）
    // -------------------------------
    private string BuildStatusText(SyougouData d)
    {
        var sb = new StringBuilder();

        // =========================
        // 火力系
        // =========================
        AddLineKey(sb, "ui_mainmenu_6_2", d.attackAdd);
        AddPercentKey(sb, "ui_mainmenu_6_3", d.attackRate);

        AddPercentKey(sb, "ui_mainmenu_6_6", d.simultaneousShotRate);
        AddPercentKey(sb, "ui_mainmenu_6_7", d.criticalAdd);
        AddPercentKey(sb, "ui_mainmenu_6_8", d.criticalDamageAdd);

        // ✅攻撃メダル倍率
        AddPercentKey(sb, "ui_mainmenu_6_12", d.attackMedal2xAdd);
        AddPercentKey(sb, "ui_mainmenu_6_13", d.attackMedal5xAdd);
        AddPercentKey(sb, "ui_mainmenu_6_14", d.attackMedal10xAdd);

        // =========================
        // 耐久系
        // =========================
        AddLineKey(sb, "ui_mainmenu_6_4", d.maxMedalAdd);
        AddPercentKey(sb, "ui_mainmenu_6_5", d.maxMedalRate);

        AddPercentKey(sb, "ui_mainmenu_6_11", d.noMedalConsumeAdd);

        // =========================
        // 回収・経済系
        // =========================
        AddLineKey(sb, "ui_mainmenu_6_10", d.recoveryWaveMedalValueAdd);
        AddLineKey(sb, "ui_mainmenu_6_9", (int)d.recoveryWaveDurationAdd);
        AddPercentKey(sb, "ui_mainmenu_6_15", d.goldDropAdd);

        // =========================
        // 収集系
        // =========================
        AddPercentKey(sb, "ui_mainmenu_6_16", d.equipmentDropAdd);
        AddPercentKey(sb, "ui_mainmenu_6_17", d.titleDropAdd);

        return sb.ToString();
    }


    private void AddLine(StringBuilder sb, string label, int value)
    {
        if (value != 0)
            sb.Append($"{label}+{value} ");
    }

    private void AddPercent(StringBuilder sb, string label, float value)
    {
        if (Mathf.Abs(value) > 0.0001f)
            sb.Append($"{label}+{value * 100f:F1}% ");
    }
    private const string PLUS_COLOR = "#FFD700";   // 金
    private const string MINUS_COLOR = "#FF4444";  // 赤

    private void AddLineKey(StringBuilder sb, string key, int value)
    {
        if (value == 0) return;

        string label = TextManager.Instance.GetUI(key);

        string sign = value > 0 ? "+" : "-";
        string color = value > 0 ? PLUS_COLOR : MINUS_COLOR;

        sb.Append(
            $"{label} <color={color}>{sign}{Mathf.Abs(value)}</color> "
        );
    }

    private void AddPercentKey(StringBuilder sb, string key, float value)
    {
        if (Mathf.Abs(value) < 0.0001f) return;

        string label = TextManager.Instance.GetUI(key);

        float percent = value * 100f;

        string sign = percent > 0 ? "+" : "-";
        string color = percent > 0 ? PLUS_COLOR : MINUS_COLOR;

        sb.Append(
            $"{label} <color={color}>{sign}{Mathf.Abs(percent):F1}%</color> "
        );
    }
}
