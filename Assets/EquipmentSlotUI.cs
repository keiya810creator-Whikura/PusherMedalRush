using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;   // ★これを追加
using TMPro;

public class EquipmentSlotUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image background;   // レアリティ色（背面）
    [SerializeField] private Image frame;        // 枠（装備中演出など用）
    [SerializeField] private Image icon;
    [SerializeField] private Button button;

    [Header("Rarity")]
    [SerializeField] private RarityColorTable rarityColorTable;

    private SoubiInstance instance;
    private Action<SoubiInstance> onClick;


    [Header("Title Badge")]
    [SerializeField] private Image titleBadge;

    private Vector3 baseScale;

    [Header("Favorite Badge")]
    [SerializeField] private Image favoriteBadge;

    [SerializeField] private GameObject disassembleMark; // ✔ 赤枠・ゴミ箱など

    [SerializeField] private TMP_Text sortValueText;

    public SoubiInstance Instance => instance;

    public void SetPulse(bool on)
    {

    }

    // ===============================
    // 通常スロット表示
    // ===============================
    public void Set(SoubiInstance data, Action<SoubiInstance> onClick)
    {
        instance = data;
        this.onClick = onClick;

        icon.enabled = true;
        icon.sprite = data.master.icon;

        // ★ 背景は常にレアリティ色のみ
        background.color =
            rarityColorTable.GetColor(data.master.rarity);

        // 枠（装備中スロット色など）
        if (data.isEquipped && data.equippedSlotId.HasValue)
        {
            frame.color =
                EquippedSlotsPanel.Instance
                    .GetSlotColor(data.equippedSlotId.Value);
        }
        else
        {
            frame.color =
            rarityColorTable.GetColor(data.master.rarity);
        }

        UpdateTitleBadge(data);
        UpdateFavoriteBadge(data);

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnClick);

        RefreshMarkUI(); // ★ マークだけ更新
    }





    // ===============================
    // 空スロット表示
    // ===============================
    public void SetEmpty()
    {
        //Debug.Log($"SetEmpty called on {name}");

        //Debug.Log("icon = " + icon);
        //Debug.Log("background = " + background);
        //Debug.Log("frame = " + frame);
        instance = null;

        if (icon != null)
            icon.enabled = false;

        if (background != null)
            background.color = Color.clear;

        if (frame != null)
            frame.color = Color.gray;

        if (titleBadge != null)
            titleBadge.gameObject.SetActive(false);

        if (favoriteBadge != null)
            favoriteBadge.gameObject.SetActive(false);

        if (button != null)
            button.onClick.RemoveAllListeners();
    }


    public void SetSlotFrameColor(Color color)
    {
        frame.color = color;
    }
    private void UpdateTitleBadge(SoubiInstance data)
    {
        if (data.attachedTitles == null || data.attachedTitles.Count == 0)
        {
            titleBadge.gameObject.SetActive(false);
            return;
        }

        // 一番レアリティが高い称号を取得
        var highestTitle = data.attachedTitles
            .OrderByDescending(t => t.rarity)
            .First();

        titleBadge.gameObject.SetActive(true);

        // レアリティ色を適用
        titleBadge.color =
            rarityColorTable.GetColor(highestTitle.rarity);
    }
    private void UpdateFavoriteBadge(SoubiInstance data)
    {
        if (favoriteBadge == null) return;

        favoriteBadge.gameObject.SetActive(data.isFavorite);
    }



    private void RefreshMarkUI()
    {
        if (instance == null) return;

        if (disassembleMark != null)
            disassembleMark.SetActive(instance.isSelectedForDismantle);
    }



    private void OnClick()
    {
        if (instance == null) return;

        // 分解モード最優先
        if (InventoryManager.Instance.isDismantleMode)
        {
            ToggleDismantleSelect();
            return;
        }

        // ★リザルト画面なら必ず ShowResult
        if (ResultPanel.IsOpen)
        {
            EquipmentDetailPanel.Instance.ShowResult(instance);
            AudioManager.Instance.PlaySE(AudioManager.Instance.clickSE);

            return;
        }

        // 通常（インベントリ）
        EquipmentDetailPanel.Instance.Show(instance);
        AudioManager.Instance.PlaySE(AudioManager.Instance.clickSE);
    }


    private void ToggleDismantleSelect()
    {
        // ★ 解除は常に許可
        if (!instance.isSelectedForDismantle)
        {
            // 選択しようとしているときだけチェック
            if (!InventoryManager.Instance.CanDisassemble(instance))
                return;
        }

        instance.isSelectedForDismantle = !instance.isSelectedForDismantle;

        RefreshDisassembleMark();
        DisassembleToolbar.Instance.Refresh();
    }
    public void SetSortValue(EquipmentSortType type, float value)
    {
        if (sortValueText == null)
            return;

        // レアリティなど「見せたくない」場合
        if (type == EquipmentSortType.Rarity)
        {
            sortValueText.text = "";
            return;
        }

        sortValueText.text = FormatSortValue(type, value);
    }
    private string FormatSortValue(EquipmentSortType type, float value)
    {
        switch (type)
        {
            // パーセンテージ
            case EquipmentSortType.AttackRate:
            case EquipmentSortType.CriticalRate:
            case EquipmentSortType.CriticalDamageRate:
            case EquipmentSortType.MedalConsumeZeroRate:
            case EquipmentSortType.Attack2xMedalRate:
            case EquipmentSortType.Attack5xMedalRate:
            case EquipmentSortType.Attack10xMedalRate:
            case EquipmentSortType.GoldDropRate:
            case EquipmentSortType.EquipDropRate:
            case EquipmentSortType.TitleDropRate:
                return $"+{value * 100f:0.#}%";

            // 通常数値
            default:
                return $"+{Mathf.RoundToInt(value)}";
        }
    }
    public void RefreshDisassembleMark()
    {
        if (instance == null) return;

        if (disassembleMark != null)
            disassembleMark.SetActive(instance.isSelectedForDismantle);
    }

    public void RefreshAllUI()
    {
        if (instance == null) return;

        // 背景・枠
        background.color = rarityColorTable.GetColor(instance.master.rarity);

        if (instance.isEquipped && instance.equippedSlotId.HasValue)
        {
            frame.color =
                EquippedSlotsPanel.Instance
                    .GetSlotColor(instance.equippedSlotId.Value);
        }
        else
        {
            frame.color =
                rarityColorTable.GetColor(instance.master.rarity);
        }

        // バッジ系
        UpdateFavoriteBadge(instance);
        UpdateTitleBadge(instance);
        RefreshEquipFrame();
        // 分解マーク
        RefreshDisassembleMark();
    }
    public void RefreshEquipFrame()
    {
        if (instance == null) return;

        if (instance.isEquipped && instance.equippedSlotId.HasValue)
        {
            frame.color =
                EquippedSlotsPanel.Instance
                    .GetSlotColor(instance.equippedSlotId.Value);
        }
        else
        {
            frame.color =
                rarityColorTable.GetColor(instance.master.rarity);
        }
    }

}
