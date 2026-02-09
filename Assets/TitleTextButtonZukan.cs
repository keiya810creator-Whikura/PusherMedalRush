using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class TitleTextButtonZukan : MonoBehaviour
{
    [SerializeField] TMP_Text titleText;
    [SerializeField] Button button;

    public void Setup(SyougouData data,
                      RarityColorTable table,
                      EquipmentDetailPanelZukan panel)
    {
        titleText.text = TextManager.Instance.GetTitle(data.nameKey);
        titleText.color = table.GetColor(data.rarity);

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() =>
        {
            panel.ShowTitle(data);
        });
    }
    public void SetSecret()
    {
        titleText.text = TextManager.Instance.GetUI("ui_mainmenu_8_9");
        titleText.color = Color.black;
        button.interactable = false;
    }

}

