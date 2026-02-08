using UnityEngine;
using TMPro;
using System.Linq;

public class DisassembleToolbar : MonoBehaviour
{
    [SerializeField] private TMP_Text toggleText;
    [SerializeField] private TMP_Text selectedCountText;
    public static DisassembleToolbar Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        Refresh();
    }

    public void ToggleDisassembleMode()
    {
        bool next = !InventoryManager.Instance.isDismantleMode;
        InventoryManager.Instance.SetDismantleMode(next);

        // ★ OFFにした瞬間、表示を完全同期
        if (!next)
        {
            EquipmentGridView.Instance.RefreshDisassembleMarks();
        }

        Refresh();
    }

    public void SelectAll()
    {
        var grid = EquipmentGridView.Instance;

        foreach (Transform c in grid.ContentTransform)
        {
            var slot = c.GetComponent<EquipmentSlotUI>();
            if (slot == null) continue;

            var soubi = slot.Instance;
            if (soubi == null) continue;

            if (InventoryManager.Instance.CanDisassemble(soubi))
            {
                soubi.isSelectedForDismantle = true;
                slot.RefreshDisassembleMark();
            }
        }

        Refresh();
    }



    public void DisassembleSelected()
    {
        var targets = InventoryManager.Instance.soubiList
        .Where(s => s.isSelectedForDismantle)
        .ToList();

        if (targets.Count == 0)
            return;

        DisassembleConfirmDialog.Instance.Show(targets);
    }


    public void Refresh()
    {
        bool isMode = InventoryManager.Instance.isDismantleMode;
        toggleText.text = isMode
            ? TextManager.Instance.GetUI("ui_mainmenu_6_36")
    : TextManager.Instance.GetUI("ui_mainmenu_6_37");


        int count = InventoryManager.Instance.soubiList
            .FindAll(s => s.isSelectedForDismantle).Count;

        selectedCountText.text = string.Format(
    TextManager.Instance.GetUI("ui_mainmenu_6_41"),
    count
);
    }
    public void OnClickDisassembleSelected()
    {
        var targets = InventoryManager.Instance.soubiList
            .Where(s => s.isSelectedForDismantle)
            .ToList();

        if (targets.Count == 0)
            return;

        DisassembleConfirmDialog.Instance.Show(targets);
    }


}
