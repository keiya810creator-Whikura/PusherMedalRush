using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Purchasing;

public class ProductRowUI : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text titleText;
    public TMP_Text priceText;
    public Button buyButton;
    public GameObject purchasedOverlay;

    // ✅購入済み表示
    public void SetPurchased(bool purchased)
    {
        purchasedOverlay.SetActive(purchased);
        buyButton.interactable = !purchased;
    }

    // ✅価格表示（自動通貨対応）
    public void SetPrice(Product product)
    {
        if (product == null)
        {
            priceText.text = "...";
            return;
        }

        priceText.text = product.metadata.localizedPriceString;
    }
}
