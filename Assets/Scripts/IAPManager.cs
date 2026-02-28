using UnityEngine;
using UnityEngine.Purchasing;
using System;
using System.Collections;
using System.Text;
using UnityEngine.Purchasing;
public class IAPManager : MonoBehaviour, IStoreListener
{
    public static IAPManager Instance;

    private IStoreController controller;
    private IExtensionProvider extensions;

    public const string PRODUCT_REMOVE_ADS = "remove_ads_medalrush";
    public const string PRODUCT_SPEED3X = "speed_3x";
    public const string PRODUCT_TITLE_BOOST = "title_boost";
#if UNITY_IOS
    public　const string PRODUCT_BUNDLE_PACK = "bundle_pack"; // iOS（審査済み）
#elif UNITY_ANDROID
    public const string PRODUCT_BUNDLE_PACK = "starter_pack_android"; // 新ID
#endif

    public const string PRODUCT_STORAGE_100 = "storage_100plus";

    public event Action OnIAPInitialized;

    private bool isInitializing;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    IEnumerator Start()
    {
        yield return null;
        yield return new WaitForSeconds(0.5f);
        InitializeIAPIfNeeded();
    }

    // =========================
    // 初期化
    // =========================
    public void InitializeIAPIfNeeded()
    {
        if (controller != null || isInitializing)
            return;

        isInitializing = true;

        var builder = ConfigurationBuilder.Instance(
            StandardPurchasingModule.Instance()
        );

        builder.AddProduct(PRODUCT_REMOVE_ADS, ProductType.NonConsumable);
        builder.AddProduct(PRODUCT_SPEED3X, ProductType.NonConsumable);
        builder.AddProduct(PRODUCT_TITLE_BOOST, ProductType.NonConsumable);
        builder.AddProduct(PRODUCT_BUNDLE_PACK, ProductType.NonConsumable);
        builder.AddProduct(PRODUCT_STORAGE_100, ProductType.Consumable);

        UnityPurchasing.Initialize(this, builder);
    }

    public void Buy(string productId)
    {
        if (controller == null)
        {
            Debug.LogError("❌ IAP未初期化");
            return;
        }

        controller.InitiatePurchase(productId);
    }

    // =========================
    // 初期化成功
    // =========================
    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        this.controller = controller;
        this.extensions = extensions;
        isInitializing = false;

        Debug.Log("✅ IAP 初期化完了");

        DebugDumpProductsOnce();

        OnIAPInitialized?.Invoke();
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        isInitializing = false;
        Debug.LogError("❌ IAP 初期化失敗: " + error);
    }

    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        isInitializing = false;
        Debug.LogError($"❌ IAP 初期化失敗: {error} / {message}");
    }

    // =========================
    // 購入処理
    // =========================
    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        string id = args.purchasedProduct.definition.id;

        switch (id)
        {
            case PRODUCT_REMOVE_ADS:
                PurchaseState.HasRemoveAds = true;
                break;
            case PRODUCT_SPEED3X:
                PurchaseState.HasSpeed3x = true;
                break;
            case PRODUCT_TITLE_BOOST:
                PurchaseState.HasTitleBoost = true;
                break;
            case PRODUCT_BUNDLE_PACK:
                PurchaseState.HasRemoveAds = true;
                PurchaseState.HasSpeed3x = true;
                PurchaseState.HasTitleBoost = true;
                break;
            case PRODUCT_STORAGE_100:
                PurchaseState.ExpandStorage();
                break;
        }

        FindObjectOfType<ShopUIController>()?.RefreshAll();
        return PurchaseProcessingResult.Complete;
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason reason)
    {
        Debug.LogError($"❌ 購入失敗: {product.definition.id} / {reason}");
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureDescription description)
    {
        Debug.LogError($"❌ 購入失敗: {product.definition.id} / {description}");
    }

    public Product GetProduct(string productId)
    {
        return controller?.products.WithID(productId);
    }

    public void DebugDumpProductsOnce()
    {
        if (controller == null)
        {
            Debug.LogWarning("❌ IAP not initialized yet");
            return;
        }

        var sb = new StringBuilder();
        sb.AppendLine("===== IAP DEBUG DUMP =====");

        foreach (var p in controller.products.all)
        {
            if (p == null)
            {
                sb.AppendLine("Product: null");
                continue;
            }

            sb.AppendLine($"ID: {p.definition.id}");
            sb.AppendLine($"  availableToPurchase: {p.availableToPurchase}");
            sb.AppendLine($"  localizedPriceString: {p.metadata?.localizedPriceString}");
            sb.AppendLine($"  isoCurrencyCode: {p.metadata?.isoCurrencyCode}");
            sb.AppendLine($"  localizedPrice(decimal): {p.metadata?.localizedPrice}");
            sb.AppendLine("--------------------------");
        }

        sb.AppendLine("===== END IAP DEBUG =====");
        Debug.Log(sb.ToString());
    }
#if UNITY_IOS
    public void RestorePurchases()
    {
        if (extensions == null) return;

        var apple = extensions.GetExtension<IAppleExtensions>();
        apple.RestoreTransactions((result, error) =>
        {
            if (result)
                ToastManager.Instance.ShowToast(
                    TextManager.Instance.GetUI("ui_toast_5"));
        });
    }
#endif
}
