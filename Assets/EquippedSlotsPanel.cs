using UnityEngine;
using System.Linq;

using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class EquippedSlotsPanel : MonoBehaviour
{
    public static EquippedSlotsPanel Instance;

    [Header("UI")]
    [SerializeField] private Transform content;
    [SerializeField] private EquipmentSlotUI slotPrefab;

    [Header("Slot Visual")]
    [SerializeField] private EquipSlotVisual[] slotVisuals;

    private Dictionary<EquipSlotId, EquipmentSlotUI> slotUIs
        = new();

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        BuildSlots();
        Refresh();
    }

    // -------------------------------
    // 5スロットを最初に固定生成
    // -------------------------------
    private void BuildSlots()
    {
        if (slotUIs.Count > 0) return;

        foreach (var v in slotVisuals)
        {
            var slot = Instantiate(slotPrefab, content);
            slot.SetEmpty();
            slot.SetSlotFrameColor(v.slotColor);

            slotUIs.Add(v.slotId, slot);
        }
    }

    // -------------------------------
    // 中身の更新
    // -------------------------------
    public void Refresh()
    {
        // 全スロットを空に
        foreach (var slot in slotUIs.Values)
        {
            slot.SetEmpty();
            slot.SetPulse(false);
        }

        foreach (var soubi in InventoryManager.Instance.soubiList)
        {
            if (!soubi.isEquipped || !soubi.equippedSlotId.HasValue)
                continue;

            var slotUI = slotUIs[soubi.equippedSlotId.Value];

            // ★ ここで Set() を必ず通す
            slotUI.Set(soubi, OnClickSlot);
            slotUI.SetPulse(true);
        }
    }
    private void OnClickSlot(SoubiInstance soubi)
    {
        EquipmentDetailPanel.Instance.Show(soubi);
    }

    private void OnClickEquippedSlot(SoubiInstance soubi)
    {
        EquipmentDetailPanel.Instance.Show(soubi);
    }

    // -------------------------------
    // 外部参照用（インベントリ側）
    // -------------------------------
    public Color GetSlotColor(EquipSlotId id)
    {
        return slotVisuals
            .First(v => v.slotId == id)
            .slotColor;
    }
}

[System.Serializable]
public class EquipSlotVisual
{
    public EquipSlotId slotId;
    public Color slotColor;

    [SerializeField]
    private EquipSlotVisual[] slotVisuals;

}

