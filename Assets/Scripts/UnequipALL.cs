using UnityEngine;

public class UnequipALL : MonoBehaviour
{
    public void OnClickUnequipAll()
    {
        AudioManager.Instance.PlaySE(AudioManager.Instance.clickSE);
        InventoryManager.Instance.UnequipAll();
    }

}
