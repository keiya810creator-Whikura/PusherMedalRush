using UnityEngine;

public class ShopButtonBinder : MonoBehaviour
{
    // ✅広告削除
    public void BuyRemoveAds()
    {
        IAPManager.Instance.Buy(IAPManager.PRODUCT_REMOVE_ADS);
    }

    // ✅3倍速
    public void BuySpeed3x()
    {
        IAPManager.Instance.Buy(IAPManager.PRODUCT_SPEED3X);
    }

    // ✅称号ドロップ1.5倍
    public void BuyTitleBoost()
    {
        IAPManager.Instance.Buy(IAPManager.PRODUCT_TITLE_BOOST);
    }

    // ✅まとめパック
    public void BuyBundlePack()
    {
        IAPManager.Instance.Buy(IAPManager.PRODUCT_BUNDLE_PACK);
    }

    // ✅倉庫拡張100枠
    public void BuyStorage100()
    {
        IAPManager.Instance.Buy(IAPManager.PRODUCT_STORAGE_100);
    }
#if UNITY_IOS
    // ✅復元ボタン（iOS向け）
    public void Restore()
    {
        IAPManager.Instance.RestorePurchases();
    }
#endif
}
