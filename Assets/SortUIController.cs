using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.Linq;

public class SortUIController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Dropdown sortDropdown;
    [SerializeField] private Button orderButton;
    [SerializeField] private TMP_Text orderButtonText;

    private void Awake()
    {
        // 未設定なら自動取得（保険）
        if (orderButton != null && orderButtonText == null)
            orderButtonText = orderButton.GetComponentInChildren<TMP_Text>();
    }

    private void Start()
    {
        // 1) Dropdownに選択肢を自動生成
        BuildDropdownOptions();

        // 2) 初期状態をUIに反映
        SyncUIFromGrid();

        // 3) イベント登録
        sortDropdown.onValueChanged.RemoveAllListeners();
        sortDropdown.onValueChanged.AddListener(OnChangeSortType);

        orderButton.onClick.RemoveAllListeners();
        orderButton.onClick.AddListener(OnClickToggleOrder);

   }

    private void BuildDropdownOptions()
    {
        sortDropdown.ClearOptions();

        var types = Enum.GetValues(typeof(EquipmentSortType))
            .Cast<EquipmentSortType>()
            .ToList();

        var options = types.Select(GetSortDisplayName).ToList();
        sortDropdown.AddOptions(options);
    }

    private string GetSortDisplayName(EquipmentSortType type)
    {
        // ※ まずは最低限。あとで辞書SOに置き換えOK
        return type switch
        {
            EquipmentSortType.Rarity =>     
            string.Format(
        TextManager.Instance.GetUI("ui_mainmenu_6_1")
    ),
            EquipmentSortType.AttackAdd => string.Format(
        TextManager.Instance.GetUI("ui_mainmenu_6_2")
    ),
            EquipmentSortType.AttackRate => string.Format(
        TextManager.Instance.GetUI("ui_mainmenu_6_3")
    ),
            EquipmentSortType.MaxMedalAdd => string.Format(
        TextManager.Instance.GetUI("ui_mainmenu_6_4")
    ),
            EquipmentSortType.MaxMedalRate => string.Format(
        TextManager.Instance.GetUI("ui_mainmenu_6_5")
    ),

            EquipmentSortType.ShotCount => string.Format(
        TextManager.Instance.GetUI("ui_mainmenu_6_6")
    ),
            EquipmentSortType.CriticalRate => string.Format(
        TextManager.Instance.GetUI("ui_mainmenu_6_7")
    ),
            EquipmentSortType.CriticalDamageRate => string.Format(
        TextManager.Instance.GetUI("ui_mainmenu_6_8")
    ),

            EquipmentSortType.RecoveryWaveExtendSeconds => string.Format(
        TextManager.Instance.GetUI("ui_mainmenu_6_9")
    ),
            EquipmentSortType.RecoveryWaveMedalValue => string.Format(
        TextManager.Instance.GetUI("ui_mainmenu_6_10")
    ),
            EquipmentSortType.MedalConsumeZeroRate => string.Format(
        TextManager.Instance.GetUI("ui_mainmenu_6_11")
    ),

            EquipmentSortType.Attack2xMedalRate => string.Format(
        TextManager.Instance.GetUI("ui_mainmenu_6_12")
    ),
            EquipmentSortType.Attack5xMedalRate => string.Format(
        TextManager.Instance.GetUI("ui_mainmenu_6_13")
    ),
            EquipmentSortType.Attack10xMedalRate => string.Format(
        TextManager.Instance.GetUI("ui_mainmenu_6_14")
    ),

            EquipmentSortType.GoldDropRate => string.Format(
        TextManager.Instance.GetUI("ui_mainmenu_6_15")
    ),
            EquipmentSortType.EquipDropRate => string.Format(
        TextManager.Instance.GetUI("ui_mainmenu_6_16")
    ),
            EquipmentSortType.TitleDropRate => string.Format(
        TextManager.Instance.GetUI("ui_mainmenu_6_17")
    ),
            EquipmentSortType.damy => string.Format(
                TextManager.Instance.GetUI("ui_mainmenu_6_17")
            ),

            _ => type.ToString()
        };
    }

    private void SyncUIFromGrid()
    {
        var grid = EquipmentGridView.Instance;
        if (grid == null) return;

        // enum順がDropdownの順と一致する前提
        sortDropdown.value = (int)grid.currentSortType;
        sortDropdown.RefreshShownValue();

        orderButtonText.text =
            grid.currentSortOrder == SortOrder.Descending ? TextManager.Instance.GetUI("ui_mainmenu_6_53") : TextManager.Instance.GetUI("ui_mainmenu_6_54");
    }

    // Dropdown選択変更
    private void OnChangeSortType(int index)
    {
        var grid = EquipmentGridView.Instance;
        if (grid == null) return;

        grid.currentSortType = (EquipmentSortType)index;
        grid.Refresh();
    }

    // 昇順/降順切替
    private void OnClickToggleOrder()
    {
        var grid = EquipmentGridView.Instance;
        if (grid == null) return;

        grid.currentSortOrder =
            grid.currentSortOrder == SortOrder.Descending
                ? SortOrder.Ascending
                : SortOrder.Descending;

        orderButtonText.text =
            grid.currentSortOrder == SortOrder.Descending ? TextManager.Instance.GetUI("ui_mainmenu_6_53") : TextManager.Instance.GetUI("ui_mainmenu_6_54");

        grid.Refresh();
    }
}
