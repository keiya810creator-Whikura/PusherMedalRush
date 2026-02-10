using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Linq;

public class TitleSortUIController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Dropdown sortDropdown;
    [SerializeField] private Button orderButton;
    [SerializeField] private TMP_Text orderButtonText;

    // ✅Dropdown index → SortType 対応表（順番固定）
    private List<TitleSortType> dropdownTypes = new();

    // ✅表示キー対応表（辞書で管理）
    private Dictionary<TitleSortType, string> sortKeyMap = new()
    {
        { TitleSortType.None, "ui_sort_none" },

        { TitleSortType.AttackAdd, "ui_mainmenu_6_2" },
        { TitleSortType.AttackRate, "ui_mainmenu_6_3" },

        { TitleSortType.MaxMedalAdd, "ui_mainmenu_6_4" },
        { TitleSortType.MaxMedalRate, "ui_mainmenu_6_5" },

        { TitleSortType.SimultaneousShot, "ui_mainmenu_6_6" },
        { TitleSortType.CriticalRate, "ui_mainmenu_6_7" },
        { TitleSortType.CriticalDamage, "ui_mainmenu_6_8" },

        { TitleSortType.RecoveryDuration, "ui_mainmenu_6_9" },
        { TitleSortType.RecoveryValue, "ui_mainmenu_6_10" },

        { TitleSortType.NoMedalConsume, "ui_mainmenu_6_11" },

        { TitleSortType.AttackMedal2x, "ui_mainmenu_6_12" },
        { TitleSortType.AttackMedal5x, "ui_mainmenu_6_13" },
        { TitleSortType.AttackMedal10x, "ui_mainmenu_6_14" },

        { TitleSortType.GoldDrop, "ui_mainmenu_6_15" },
        { TitleSortType.EquipmentDrop, "ui_mainmenu_6_16" },
        { TitleSortType.TitleDrop, "ui_mainmenu_6_17" },

        { TitleSortType.Rarity, "ui_sort_rarity" }
    };

    private void Awake()
    {
        if (orderButton != null && orderButtonText == null)
            orderButtonText = orderButton.GetComponentInChildren<TMP_Text>();
    }

    private void Start()
    {
        BuildDropdownOptions();
        SyncUIFromPanel();

        sortDropdown.onValueChanged.RemoveAllListeners();
        sortDropdown.onValueChanged.AddListener(OnChangeSortType);

        orderButton.onClick.RemoveAllListeners();
        orderButton.onClick.AddListener(OnClickToggleOrder);
    }

    // ✅Dropdownを辞書キーから生成（多言語対応）
    private void BuildDropdownOptions()
    {
        sortDropdown.ClearOptions();
        dropdownTypes.Clear();

        List<string> labels = new();

        foreach (var pair in sortKeyMap)
        {
            TitleSortType type = pair.Key;
            string key = pair.Value;

            // ✅UI辞書から取得
            string label = TextManager.Instance.GetUI(key);

            dropdownTypes.Add(type);
            labels.Add(label);
        }

        sortDropdown.AddOptions(labels);
    }

    // ✅言語切替時に呼ぶ（Dropdown再構築）
    public void RefreshLanguage()
    {
        int currentIndex = sortDropdown.value;

        BuildDropdownOptions();

        sortDropdown.value = currentIndex;
        sortDropdown.RefreshShownValue();
    }

    // ✅UI初期同期
    private void SyncUIFromPanel()
    {
        var panel = TitleSelectPanel.Instance;
        if (panel == null) return;

        // ✅現在SortTypeがどのindexか探す
        int index = dropdownTypes.IndexOf(panel.currentSortType);
        if (index < 0) index = 0;

        sortDropdown.value = index;
        sortDropdown.RefreshShownValue();

        orderButtonText.text =
            panel.currentSortOrder == SortOrder.Descending ? TextManager.Instance.GetUI("ui_mainmenu_6_53") : TextManager.Instance.GetUI("ui_mainmenu_6_54");
    }

    // ✅Dropdown変更
    private void OnChangeSortType(int index)
    {
        var panel = TitleSelectPanel.Instance;
        if (panel == null) return;

        if (index < 0 || index >= dropdownTypes.Count)
            return;

        panel.currentSortType = dropdownTypes[index];
        panel.RefreshList();
    }

    // ✅昇順/降順切替
    private void OnClickToggleOrder()
    {
        var panel = TitleSelectPanel.Instance;
        if (panel == null) return;

        panel.currentSortOrder =
            panel.currentSortOrder == SortOrder.Descending
                ? SortOrder.Ascending
                : SortOrder.Descending;

        orderButtonText.text =
            panel.currentSortOrder == SortOrder.Descending ? TextManager.Instance.GetUI("ui_mainmenu_6_53") : TextManager.Instance.GetUI("ui_mainmenu_6_54");

        panel.RefreshList();
    }
}