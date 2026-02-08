using UnityEngine;

public class EquipmentInventoryPanel : MonoBehaviour
{
    [SerializeField] Transform content;
    [SerializeField] EquipmentSlotUI slotPrefab;

    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        
    }


    public void Refresh()
    {
        foreach (Transform c in content)
            Destroy(c.gameObject);

        foreach (var soubi in InventoryManager.Instance.soubiList)
        {
            var slot = Instantiate(slotPrefab, content);
            slot.Set(soubi, OnClickSlot);
        }
    }

    private void OnClickSlot(SoubiInstance soubi)
    {
        EquipmentDetailPanel.Instance.Show(soubi);
    }
}
