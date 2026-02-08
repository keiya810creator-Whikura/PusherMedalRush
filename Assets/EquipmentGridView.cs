using UnityEngine;
using System.Collections.Generic;
using System.Linq;
public class EquipmentGridView : MonoBehaviour
{
    public enum GridViewMode
    {
        Inventory,
        Result
    }

    private GridViewMode currentMode = GridViewMode.Inventory;

    public static EquipmentGridView Instance;

    [SerializeField] private Transform content;
    [SerializeField] private EquipmentSlotUI slotPrefab;

    public bool isDisassembleMode;
    public EquipmentFilterState filterState = new();

    public EquipmentSortType currentSortType = EquipmentSortType.Rarity;
    public SortOrder currentSortOrder = SortOrder.Descending;

    public Transform ContentTransform => content;
    private struct SortEntry
    {
        public SoubiInstance soubi;
        public float sortValue;
    }

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        InventoryManager.Instance.OnEquipmentChanged += OnInventoryChanged;

        if (currentMode == GridViewMode.Inventory)
            Refresh();
    }

    private void OnDisable()
    {
        if (InventoryManager.Instance != null)
            InventoryManager.Instance.OnEquipmentChanged -= OnInventoryChanged;
    }

    private void OnInventoryChanged()
    {
        if (currentMode == GridViewMode.Inventory)
            Refresh();
    }



    public void Refresh()
    {
        if (currentMode != GridViewMode.Inventory)
            return;

        ClearGrid();

        var entries = GetSortedEntries();

        foreach (var e in entries)
        {
            if (!PassFilter(e.soubi))
                continue;

            var slot = Instantiate(slotPrefab, content);
            slot.Set(e.soubi, OnClickSlot);
            slot.SetSortValue(currentSortType, e.sortValue);
        }
    }





    private void OnClickSlot(SoubiInstance soubi)
    {
        if (InventoryManager.Instance.isDismantleMode)
        {
            if (!InventoryManager.Instance.CanDisassemble(soubi))
                return;

            soubi.isSelectedForDismantle = !soubi.isSelectedForDismantle;
            DisassembleToolbar.Instance.Refresh();
            return;
        }

        EquipmentDetailPanel.Instance.Show(soubi);
    }

    private float GetSortValue(SoubiInstance soubi)
{
    var s = DisplayStatusUtility.Build(soubi);

    return currentSortType switch
    {
        EquipmentSortType.Rarity => soubi.master.rarity,

        EquipmentSortType.AttackAdd => s.attackAdd,
        EquipmentSortType.AttackRate => s.attackRate,
        EquipmentSortType.MaxMedalAdd => s.maxMedalAdd,
        EquipmentSortType.MaxMedalRate => s.maxMedalRate,

        EquipmentSortType.ShotCount => s.simultaneousShotRate,
        EquipmentSortType.CriticalRate => s.criticalAdd,
        EquipmentSortType.CriticalDamageRate => s.criticalDamageAdd,

        EquipmentSortType.RecoveryWaveExtendSeconds => s.recoveryWaveDurationAdd,
        EquipmentSortType.RecoveryWaveMedalValue => s.recoveryWaveMedalValueAdd,
        EquipmentSortType.MedalConsumeZeroRate => s.noMedalConsumeAdd,

        EquipmentSortType.Attack2xMedalRate => s.attackMedal2xAdd,
        EquipmentSortType.Attack5xMedalRate => s.attackMedal5xAdd,
        EquipmentSortType.Attack10xMedalRate => s.attackMedal10xAdd,

        EquipmentSortType.GoldDropRate => s.goldDropAdd,
        EquipmentSortType.EquipDropRate => s.equipmentDropAdd,
        EquipmentSortType.TitleDropRate => s.titleDropAdd,

        _ => 0f
    };
}
    private List<SortEntry> GetSortedEntries()
    {
        var entries = InventoryManager.Instance.soubiList
            .Select(s => new SortEntry
            {
                soubi = s,
                sortValue = GetSortValue(s)
            });

        return currentSortOrder == SortOrder.Descending
            ? entries.OrderByDescending(e => e.sortValue).ToList()
            : entries.OrderBy(e => e.sortValue).ToList();
    }

    private bool PassFilter(SoubiInstance s)
    {
        switch (filterState.mode)
        {
            case EquipmentFilterMode.All:
                return true;

            // ------------------
            // 装備レアリティ
            // ------------------
            case EquipmentFilterMode.EquipRarity1:
            case EquipmentFilterMode.EquipRarity2:
            case EquipmentFilterMode.EquipRarity3:
            case EquipmentFilterMode.EquipRarity4:
            case EquipmentFilterMode.EquipRarity5:
            case EquipmentFilterMode.EquipRarity6:
                int equipRarity =
                    (int)filterState.mode
                    - (int)EquipmentFilterMode.EquipRarity1 + 1;
                return s.master.rarity == equipRarity;

            // ------------------
            // ★ 称号レアリティ
            // ------------------
            case EquipmentFilterMode.TitleRarity1:
            case EquipmentFilterMode.TitleRarity2:
            case EquipmentFilterMode.TitleRarity3:
            case EquipmentFilterMode.TitleRarity4:
            case EquipmentFilterMode.TitleRarity5:
            case EquipmentFilterMode.TitleRarity6:
                if (s.attachedTitles == null || s.attachedTitles.Count == 0)
                    return false;

                int titleRarity =
                    (int)filterState.mode
                    - (int)EquipmentFilterMode.TitleRarity1 + 1;

                // ★ 1つでも該当レアリティがあればOK
                return s.attachedTitles.Any(t => t.rarity == titleRarity);

            // ------------------
            // 称号あり / なし
            // ------------------
            case EquipmentFilterMode.HasTitle:
                return s.attachedTitles != null &&
                       s.attachedTitles.Count > 0;

            case EquipmentFilterMode.NoTitle:
                return s.attachedTitles == null ||
                       s.attachedTitles.Count == 0;

            // ------------------
            // お気に入り
            // ------------------
            case EquipmentFilterMode.Favorite:
                return s.isFavorite;

            case EquipmentFilterMode.NotFavorite:   // ★ 追加
                return !s.isFavorite;
        }

        return true;
    }

    public void RefreshDisassembleMarks()
    {
        foreach (Transform c in content)
        {
            var slot = c.GetComponent<EquipmentSlotUI>();
            if (slot != null)
            {
                slot.RefreshDisassembleMark();
            }
        }
    }
    public void RefreshVisibleSlotUI()
    {
        foreach (Transform c in content)
        {
            var slot = c.GetComponent<EquipmentSlotUI>();
            if (slot != null)
            {
                slot.RefreshAllUI();
            }
        }
    }
    public void OnFilterChanged(EquipmentFilterMode newMode)
    {
        // ① フィルター更新
        filterState.mode = newMode;

        // ② 売却選択を全解除（★ここ重要）
        InventoryManager.Instance.ClearAllDismantleSelection();

        // ③ 表示中スロットのマーク更新
        RefreshDisassembleMarks();

        // ④ グリッド再構築（フィルター適用）
        Refresh();

        // ⑤ Toolbar 更新
        DisassembleToolbar.Instance.Refresh();
    }
    // Result画面用：外部リストをそのまま表示
    public void ShowResult(List<SoubiInstance> list)
    {
        currentMode = GridViewMode.Result;

        ClearGrid();

        foreach (var soubi in list)
        {
            var slot = Instantiate(slotPrefab, content);
            slot.Set(soubi, null); // 操作不可
        }
    }
    public void ShowInventory()
    {
        currentMode = GridViewMode.Inventory;
        Refresh();
    }

    private void ClearGrid()
    {
        foreach (Transform child in content)
            Destroy(child.gameObject);
    }

}
