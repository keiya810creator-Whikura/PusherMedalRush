using UnityEngine;

public class EquipmentFilterPanel : MonoBehaviour
{
    [SerializeField] private EquipmentGridView grid;

    // š Toggle ‚Ì OnValueChanged ‚©‚çŒÄ‚Ô
    public void SetFilterMode(int mode)
    {
        // ToggleGroup ‚È‚Ì‚Å ON ‚Ì‚µ‚©—ˆ‚È‚¢‘O’ñ
        grid.filterState.mode = (EquipmentFilterMode)mode;
        InventoryManager.Instance.ClearAllDismantleSelection();
        EquipmentGridView.Instance.RefreshDisassembleMarks();
        EquipmentGridView.Instance.Refresh();
        DisassembleToolbar.Instance.Refresh();

    }
}
