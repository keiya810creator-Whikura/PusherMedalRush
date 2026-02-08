using UnityEngine;
using TMPro;

public class StorageCapacityText : MonoBehaviour
{
    [SerializeField] private TMP_Text storageText;

    private void Reset()
    {
        storageText = GetComponent<TMP_Text>();
    }

    private void OnEnable()
    {
        Refresh();

        // ✅装備変化があったら自動更新
        InventoryManager.Instance.OnEquipmentChanged += Refresh;
    }

    private void OnDisable()
    {
        if (InventoryManager.Instance != null)
            InventoryManager.Instance.OnEquipmentChanged -= Refresh;
    }

    public void Refresh()
    {
        int current = InventoryManager.Instance.TotalCount;
        int max = InventoryManager.Instance.MaxStorage;

        storageText.text = $"{current} / {max}";
    }
}
