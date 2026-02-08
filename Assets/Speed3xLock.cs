using UnityEngine;

public class Speed3xLock : MonoBehaviour
{
    [Header("3倍速トグル（課金専用）")]
    [SerializeField] private GameObject speed3xToggleObject;

    void OnEnable()
    {
        // ✅課金してなければ3倍速を非表示
        if (!PurchaseState.HasSpeed3x)
        {
            speed3xToggleObject.SetActive(false);
        }
    }
}
