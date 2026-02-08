using UnityEngine;
using TMPro;
using System.Linq;   // ★これを追加
using System.Collections.Generic;

public class DisassembleConfirmDialog : MonoBehaviour
{
    public static DisassembleConfirmDialog Instance;

    [SerializeField] TMP_Text equipCountText;
    [SerializeField] TMP_Text titleListText;

    private List<SoubiInstance> targets;

    private void Awake()
    {
        Instance = this;
        gameObject.SetActive(false);
    }

    public void Show(IEnumerable<SoubiInstance> soubiList)
    {
        targets = soubiList.ToList();

        equipCountText.text = string.Format(
    TextManager.Instance.GetUI("ui_mainmenu_6_41"),
    targets.Count
);
        titleListText.text = BuildTitleListText(targets);

        gameObject.SetActive(true);
    }

    private string BuildTitleListText(List<SoubiInstance> list)
    {
        var titles = list
            .Where(s => s.attachedTitles != null)
            .SelectMany(s => s.attachedTitles)
            .ToList();

        // ✅称号なしの場合
        if (titles.Count == 0)
            return string.Format(
                TextManager.Instance.GetUI("ui_mainmenu_6_43")
            );

        // ✅称号あり → 翻訳して表示
        return string.Join("\n",
            titles.Select(t =>
            {
                string titleName = TextManager.Instance.GetTitle(t.nameKey);
                return $"・《{titleName}》";
            })
        );
    }

    public void OnClickConfirm()
    {
        InventoryManager.Instance.Disassemble(targets);

        EquipmentGridView.Instance.Refresh();
        EquipmentGridView.Instance.RefreshVisibleSlotUI();
        DisassembleToolbar.Instance.Refresh();

        EquipmentDetailPanel.Instance?.Close();

        gameObject.SetActive(false);
    }

    public void OnClickCancel()
    {
        gameObject.SetActive(false);
    }
}


