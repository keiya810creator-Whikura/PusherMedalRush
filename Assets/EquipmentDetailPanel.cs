using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text;
using System.Linq;

public class EquipmentDetailPanel : MonoBehaviour
{
    public static EquipmentDetailPanel Instance;

    public enum DetailMode
    {
        Inventory,
        Result
    }

    [Header("UI")]
    [SerializeField] Image icon;
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text statusText;
    [SerializeField] Button closeButton;

    [Header("Inventory Only UI")]
    [SerializeField] Button equipButton;
    [SerializeField] TMP_Text equipButtonText;
    [SerializeField] TMP_Text favoriteButtonText;
    [SerializeField] Button disassembleButton;
    [SerializeField] Button addTitleButton;

    [Header("Skill UI")]
    [SerializeField] TMP_Text skillNameText;
    [SerializeField] TMP_Text skillDescText;

    [Header("Table")]
    [SerializeField] RarityColorTable rarityColorTable;

    private SoubiInstance current;
    private DetailMode currentMode;
    public bool IsResultMode => currentMode == DetailMode.Result;
    [Header("Skill List UI")]
    [SerializeField] Transform skillButtonParent;
    [SerializeField] SkillButtonUI skillButtonPrefab;
    private void Awake()
    {
        Instance = this;
        gameObject.SetActive(false);

        if (closeButton != null)
        {
            closeButton.onClick.AddListener(Close);
        }
    }

    // ===============================
    // Public Show
    // ===============================

    public void Show(SoubiInstance soubi)
    {
        currentMode = DetailMode.Inventory;
        ShowInternal(soubi);
        RefreshEquipButton();
    }

    public void ShowResult(SoubiInstance soubi)
    {
        currentMode = DetailMode.Result;
        ShowInternal(soubi);
    }

    // ===============================
    // Core
    // ===============================

    private void ShowInternal(SoubiInstance soubi)
    {
        current = soubi;

        icon.sprite = soubi.master.icon;
        nameText.text = BuildDisplayName(soubi);
        statusText.text = BuildStatusText(soubi);

        RefreshSkillUI(soubi);

        // ★ ここで必ず最新状態を反映
        RefreshEquipButton();
        RefreshFavoriteButton();

        ApplyMode();

        gameObject.SetActive(true);
    }


    private void ApplyMode()
    {
        bool isResult = currentMode == DetailMode.Result;

        if (equipButton)
            equipButton.gameObject.SetActive(!isResult);

        if (disassembleButton)
            disassembleButton.gameObject.SetActive(!isResult);

        if (favoriteButtonText)
            favoriteButtonText.gameObject.SetActive(true);

        if (addTitleButton)
            addTitleButton.gameObject.SetActive(true);
    }



    private void SetButtonGray(Button button, bool gray)
    {
        var img = button.GetComponent<Image>();
        if (img == null) return;

        Color c = img.color;
        c.a = gray ? 0.4f : 1.0f;
        img.color = c;
    }



    // ===============================
    // Display Name
    // ===============================

    private string BuildDisplayName(SoubiInstance soubi)
    {
        // -----------------------------
        // ✅装備名（nameKey → 翻訳）
        // -----------------------------
        Color equipColor = rarityColorTable.GetColor(soubi.master.rarity);
        string equipHex = ColorUtility.ToHtmlStringRGB(equipColor);

        // ✅ここが変更点：辞書から装備名を取得
        string equipRealName = TextManager.Instance.GetEquip(soubi.master.nameKey);

        string equipName =
            $"<color=#{equipHex}>{equipRealName}</color>";

        // -----------------------------
        // ✅称号が無ければ装備名だけ
        // -----------------------------
        if (soubi.attachedTitles == null || soubi.attachedTitles.Count == 0)
            return equipName;

        // -----------------------------
        // ✅称号名（nameKey → 翻訳）
        // -----------------------------
        var title = soubi.attachedTitles
            .OrderByDescending(t => t.rarity)
            .First();

        Color titleColor = rarityColorTable.GetColor(title.rarity);
        string titleHex = ColorUtility.ToHtmlStringRGB(titleColor);

        // ✅ここが変更点：辞書から称号名を取得
        string titleRealName = TextManager.Instance.GetTitle(title.nameKey);

        string titleName =
            $"<color=#{titleHex}>《{titleRealName}》</color>";

        // -----------------------------
        // ✅表示：称号 + 装備名
        // -----------------------------
        return $"{titleName}\n{equipName}";
    }

    // ===============================
    // Status
    // ===============================

    private string BuildStatusText(SoubiInstance soubi)
    {
        var s = BuildDisplayStatus(soubi);
        var sb = new StringBuilder();

        // -------------------------
        // ✅火力系
        // -------------------------
        AddLineKey(sb, "ui_mainmenu_6_2", s.attackAdd);          // 攻撃力
        AddPercentKey(sb, "ui_mainmenu_6_3", s.attackRate);     // 攻撃力倍率
        AddPercentKey(sb, "ui_mainmenu_6_6", s.simultaneousShotRate); // 同時発射数
        AddPercentKey(sb, "ui_mainmenu_6_7", s.criticalAdd);    // クリティカル率
        AddPercentKey(sb, "ui_mainmenu_6_8", s.criticalDamageAdd); // クリティカル倍率

        // -------------------------
        // ✅攻撃メダル倍率
        // -------------------------
        AddPercentKey(sb, "ui_mainmenu_6_12", s.attackMedal2xAdd);
        AddPercentKey(sb, "ui_mainmenu_6_13", s.attackMedal5xAdd);
        AddPercentKey(sb, "ui_mainmenu_6_14", s.attackMedal10xAdd);

        // -------------------------
        // ✅耐久系
        // -------------------------
        AddLineKey(sb, "ui_mainmenu_6_4", s.maxMedalAdd);       // 最大メダル
        AddPercentKey(sb, "ui_mainmenu_6_5", s.maxMedalRate);   // 最大メダル倍率

        // -------------------------
        // ✅回収系
        // -------------------------
        AddLineKey(sb, "ui_mainmenu_6_10", s.recoveryWaveMedalValueAdd); // 回収Waveメダル価値
        AddLineKey(sb, "ui_mainmenu_6_9", s.recoveryWaveDurationAdd);    // 回収Wave秒

        // -------------------------
        // ✅経済系
        // -------------------------
        AddPercentKey(sb, "ui_mainmenu_6_15", s.goldDropAdd);        // ゴールドドロップ量
        AddPercentKey(sb, "ui_mainmenu_6_16", s.equipmentDropAdd);   // 装備ドロップ倍率
        AddPercentKey(sb, "ui_mainmenu_6_17", s.titleDropAdd);       // 称号ドロップ倍率

        // -------------------------
        // ✅消費系（確率）
        // -------------------------
        AddPercentKey(sb, "ui_mainmenu_6_11", s.noMedalConsumeAdd);  // メダル消費0確率

        return sb.ToString();
    }
    private void AddLineKey(StringBuilder sb, string key, long value)
    {
        string label = TextManager.Instance.GetUI(key);
        AddLine(sb, label, value);
    }
    private void AddPercentKey(StringBuilder sb, string key, float value)
    {
        string label = TextManager.Instance.GetUI(key);
        AddPercent(sb, label, value);
    }
    public DisplayStatus BuildDisplayStatus(SoubiInstance soubi)
    {
        var s = new DisplayStatus();

        AddFrom(s, soubi.master);

        if (soubi.attachedTitles != null)
        {
            foreach (var t in soubi.attachedTitles)
                AddFrom(s, t);
        }

        return s;
    }

    private void AddFrom(DisplayStatus s, SoubiData d)
    {
        s.attackAdd += d.attackAdd;
        s.attackRate += d.attackRate;
        s.simultaneousShotRate += d.simultaneousShotRate;
        s.criticalAdd += d.criticalAdd;
        s.criticalDamageAdd += d.criticalDamageAdd;

        s.attackMedal2xAdd += d.attackMedal2xAdd;
        s.attackMedal5xAdd += d.attackMedal5xAdd;
        s.attackMedal10xAdd += d.attackMedal10xAdd;

        s.maxMedalAdd += d.maxMedalAdd;
        s.maxMedalRate += d.maxMedalRate;
        s.noMedalConsumeAdd += d.noMedalConsumeAdd;

        s.recoveryWaveMedalValueAdd += d.recoveryWaveMedalValueAdd;
        s.recoveryWaveDurationAdd += d.recoveryWaveDurationAdd;
        s.goldDropAdd += d.goldDropAdd;

        s.equipmentDropAdd += d.equipmentDropAdd;
        s.titleDropAdd += d.titleDropAdd;
    }

    private void AddFrom(DisplayStatus s, SyougouData d)
    {
        s.attackAdd += d.attackAdd;
        s.attackRate += d.attackRate;

        s.simultaneousShotRate += d.simultaneousShotRate;
        s.criticalAdd += d.criticalAdd;
        s.criticalDamageAdd += d.criticalDamageAdd;

        s.attackMedal2xAdd += d.attackMedal2xAdd;
        s.attackMedal5xAdd += d.attackMedal5xAdd;
        s.attackMedal10xAdd += d.attackMedal10xAdd;

        s.maxMedalAdd += d.maxMedalAdd;
        s.maxMedalRate += d.maxMedalRate;
        s.noMedalConsumeAdd += d.noMedalConsumeAdd;

        s.recoveryWaveMedalValueAdd += d.recoveryWaveMedalValueAdd;
        s.recoveryWaveDurationAdd += d.recoveryWaveDurationAdd;
        s.goldDropAdd += d.goldDropAdd;

        s.equipmentDropAdd += d.equipmentDropAdd;
        s.titleDropAdd += d.titleDropAdd;
    }


    // ===============================
    // Helpers
    // ===============================

    private const string PLUS_COLOR = "#FFD700";  // 金
    private const string MINUS_COLOR = "#FF4444"; // 赤

    private void AddLine(StringBuilder sb, string label, int value)
    {
        if (value == 0) return;

        string sign = value > 0 ? "+" : "-";
        string color = value > 0 ? PLUS_COLOR : MINUS_COLOR;

        sb.AppendLine(
            $"{label} <color={color}>{sign}{Mathf.Abs(value)}</color>"
        );
    }

    private void AddLine(StringBuilder sb, string label, float value)
    {
        if (Mathf.Abs(value) < 0.0001f) return;

        string sign = value > 0 ? "+" : "-";
        string color = value > 0 ? PLUS_COLOR : MINUS_COLOR;

        sb.AppendLine(
            $"{label} <color={color}>{sign}{Mathf.Abs(value):F1}</color>"
        );
    }

    private void AddPercent(StringBuilder sb, string label, float value)
    {
        if (Mathf.Abs(value) < 0.0001f) return;

        float percent = value * 100f;

        string sign = percent > 0 ? "+" : "-";
        string color = percent > 0 ? PLUS_COLOR : MINUS_COLOR;

        sb.AppendLine(
            $"{label} <color={color}>{sign}{Mathf.Abs(percent):F1}%</color>"
        );
    }

    // ===============================
    // Inventory Only
    // ===============================

    private void RefreshEquipButton()
    {
        if (!equipButton || !equipButtonText || current == null) return;

        equipButton.onClick.RemoveAllListeners();

        if (current.isEquipped)
        {
            equipButtonText.text =
                TextManager.Instance.GetUI("ui_mainmenu_6_47"); // 外す

            equipButton.onClick.AddListener(() =>
            {
                AudioManager.Instance.PlaySE(AudioManager.Instance.clickSE);
                InventoryManager.Instance.Unequip(current);
                RefreshEquipButton();
                EquippedSlotsPanel.Instance.Refresh();
            });
        }
        else
        {
            equipButtonText.text =
                TextManager.Instance.GetUI("ui_mainmenu_6_46"); // 装備する

            equipButton.onClick.AddListener(() =>
            {
                AudioManager.Instance.PlaySE(AudioManager.Instance.clickSE);
                if (!InventoryManager.Instance.Equip(current)) return;
                RefreshEquipButton();
                EquippedSlotsPanel.Instance.Refresh();
            });
        }
    }


    private void RefreshFavoriteButton()
    {
        if (!favoriteButtonText || current == null) return;

        favoriteButtonText.text =
            current.isFavorite ? TextManager.Instance.GetUI("ui_mainmenu_6_49")
        : TextManager.Instance.GetUI("ui_mainmenu_6_48");
    }

    public void ToggleFavorite()
    {
        if (current == null) return;

        current.isFavorite = !current.isFavorite;

        EquipmentGridView.Instance.RefreshVisibleSlotUI();
        InventoryManager.Instance.NotifyEquipmentChanged();
        RefreshFavoriteButton();
        if (EquippedSlotsPanel.Instance != null)
            EquippedSlotsPanel.Instance.Refresh();

    }


    public void OnClickAddTitle()
    {
        if (current == null) return;

        TitleSelectPanel.Instance.Show(
    current,
    TitleSelectPanel.TitleSelectMode.Inventory
);
        if (EquippedSlotsPanel.Instance != null)
            EquippedSlotsPanel.Instance.Refresh();

    }


    public void OnClickDisassemble()
    {
        if (current == null) return;
        if (currentMode == DetailMode.Result) return; // ★追加
        if (!InventoryManager.Instance.CanDisassemble(current)) return;

        DisassembleConfirmDialog.Instance.Show(new[] { current });

        if (EquippedSlotsPanel.Instance != null)
            EquippedSlotsPanel.Instance.Refresh();

    }


    private void RefreshSkillUI(SoubiInstance soubi)
    {
        if (!skillButtonParent || !skillButtonPrefab) return;

        // ✅既存ボタン削除
        foreach (Transform child in skillButtonParent)
            Destroy(child.gameObject);


        skillButtonParent.gameObject.SetActive(true);

        // ✅スキル分ボタン生成
        foreach (var skill in soubi.master.skills)
        {
            var btn = Instantiate(skillButtonPrefab, skillButtonParent);

            btn.Set(skill, (clickedSkill) =>
            {
                SkillDetailPanel.Instance.Show(clickedSkill);
            });
        }
    }

    public void Close()
    {
        current = null;
        gameObject.SetActive(false);
    }

}
