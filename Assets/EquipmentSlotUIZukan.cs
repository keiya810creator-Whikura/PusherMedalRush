using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class EquipmentSlotUIZukan : MonoBehaviour
{
    [Header("UI Parts")]
    [SerializeField] private Image background;
    [SerializeField] private Image soubiIcon;
    [SerializeField] private GameObject secretOverlay;
    [SerializeField] private Button button;

    [Header("Rarity Color Table")]
    [SerializeField] private RarityColorTable rarityColorTable;

    private SoubiData soubiData;
    private bool isUnlocked;

    public Action<SoubiData> OnClickSlot;

    public void Setup(SoubiData data, bool unlocked)
    {
        soubiData = data;
        isUnlocked = unlocked;

        if (data == null) return;

        // ✅アイコン
        soubiIcon.sprite = data.icon;

        // ✅背景色
        background.color = rarityColorTable.GetColor(data.rarity);

        // ✅未取得なら隠す
        secretOverlay.SetActive(!unlocked);

        // ✅ボタン制御
        button.interactable = unlocked;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() =>
        {
            Debug.Log("✅ Button.onClick 発火: " + soubiData.nameKey);

            OnClickSlot?.Invoke(soubiData);
        });
    }

    // ============================
    // ✅クリックが奪われてるか調べる
    // ============================
    public void DebugRaycast(PointerEventData eventData)
    {
        Debug.Log("Pointerヒット対象: " +
            eventData.pointerCurrentRaycast.gameObject.name);
    }
}