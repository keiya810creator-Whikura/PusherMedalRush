using UnityEngine;
using TMPro;

public class StorageInfoUI : MonoBehaviour
{
    [SerializeField] private TMP_Text storageText;

    void Update()
    {
        Refresh();
    }

    public void Refresh()
    {
        storageText.text =
                    string.Format(
                        TextManager.Instance.GetUI("ui_mainmenu_7_10"), InventoryManager.Instance.TotalCount, PurchaseState.StorageSize);


        //$"ëqå…óeó ÅF{InventoryManager.Instance.TotalCount} / {PurchaseState.StorageSize}";
    }
}
