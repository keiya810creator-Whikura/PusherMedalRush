using UnityEngine;

public class BuyReset : MonoBehaviour
{
#if UNITY_IOS
    public void Restore()
    {
        //IAPManager.Instance.RestorePurchases();
    }
#endif
}
