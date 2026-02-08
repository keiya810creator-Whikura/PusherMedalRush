using UnityEngine;

public class EquipmentInventoryPanel : MonoBehaviour
{
    [SerializeField] Transform content;
    [SerializeField] EquipmentSlotUI slotPrefab;

    public GameObject soubiMenuTutorialPanel;
    void Start()
    {
        if (TutorialManager.Instance.CanShow(TutorialID.EquipmentMenu))
            soubiMenuTutorialPanel.SetActive(true);
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
