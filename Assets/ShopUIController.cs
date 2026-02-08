using UnityEngine;

public class ShopUIController : MonoBehaviour
{
    [SerializeField] private ProductRowUI removeAdsRow;
    [SerializeField] private ProductRowUI speed3xRow;
    [SerializeField] private ProductRowUI titleBoostRow;
    [SerializeField] private ProductRowUI bundlePackRow;
    [SerializeField] private ProductRowUI storageRow;

    void OnEnable()
    {
        if (IAPManager.Instance == null) return;

        IAPManager.Instance.OnIAPInitialized += RefreshPrices;
        IAPManager.Instance.InitializeIAPIfNeeded();

        RefreshPurchasedState();
    }

    void OnDisable()
    {
        if (IAPManager.Instance != null)
            IAPManager.Instance.OnIAPInitialized -= RefreshPrices;
    }

    public void RefreshAll()
    {
        RefreshPurchasedState();
        RefreshPrices();
    }

    private void RefreshPurchasedState()
    {
        removeAdsRow.SetPurchased(PurchaseState.HasRemoveAds);
        speed3xRow.SetPurchased(PurchaseState.HasSpeed3x);
        titleBoostRow.SetPurchased(PurchaseState.HasTitleBoost);

        bool anyPurchased =
            PurchaseState.HasRemoveAds ||
            PurchaseState.HasSpeed3x ||
            PurchaseState.HasTitleBoost;

        bundlePackRow.SetPurchased(anyPurchased);
        storageRow.SetPurchased(false);
    }

    private void RefreshPrices()
    {
        var iap = IAPManager.Instance;
        if (iap == null) return;

        removeAdsRow.SetPrice(iap.GetProduct(IAPManager.PRODUCT_REMOVE_ADS));
        speed3xRow.SetPrice(iap.GetProduct(IAPManager.PRODUCT_SPEED3X));
        titleBoostRow.SetPrice(iap.GetProduct(IAPManager.PRODUCT_TITLE_BOOST));
        bundlePackRow.SetPrice(iap.GetProduct(IAPManager.PRODUCT_BUNDLE_PACK));
        storageRow.SetPrice(iap.GetProduct(IAPManager.PRODUCT_STORAGE_100));
    }
}
