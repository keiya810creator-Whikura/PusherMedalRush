using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MonsterDetailPanel : MonoBehaviour
{
    [Header("UI Parts")]
    [SerializeField] private Image monsterIcon;
    [SerializeField] private TMP_Text monsterName;

    [SerializeField] private TMP_Text stageText;
    [SerializeField] private TMP_Text waveText;
    [SerializeField] private TMP_Text rateText;

    [SerializeField] private GameObject bossBadge;

    [Header("Drop Equipment UI")]
    [SerializeField] private Transform dropSoubiParent;
    [SerializeField] private EquipmentSlotUIZukan equipmentSlotPrefab;
    [SerializeField] private EquipmentDetailPanelZukan equipmentDetailPanelZukan;
    [SerializeField] private Transform dropTitleParent;
    [SerializeField] private TitleTextButtonZukan titleButtonPrefab;
    [SerializeField] private EquipmentDetailPanelZukan detailPanel;
    [SerializeField] private RarityColorTable rarityTable;

    /// <summary>
    /// 詳細表示更新
    /// </summary>
    public void Show(MonsterData data)
    {
        monsterIcon.sprite = data.icon;
        monsterName.text = TextManager.Instance.GetMonster(data.nameKey);

        stageText.text = string.Format(
            TextManager.Instance.GetUI("ui_mainmenu_8_4"), data.syutugennStage);
        waveText.text = $"Wave : {data.minWave} ～ {data.maxWave}";
        rateText.text = string.Format(
            TextManager.Instance.GetUI("ui_mainmenu_8_5"), data.syutugennritu)+"%";

        bossBadge.SetActive(data.isBoss);

        // ✅既存削除
        // ✅ドロップ装備欄を初期化
        foreach (Transform child in dropSoubiParent)
            Destroy(child.gameObject);

        // ✅ドロップ装備を生成
        foreach (var soubi in data.dropSoubiList)
        {
            var slot = Instantiate(equipmentSlotPrefab, dropSoubiParent);

            // ✅取得済みなら解放
            bool unlocked =
    ZukanProgressManager.Instance.IsObtainedEquip(soubi.id);


            slot.Setup(soubi, unlocked);

            // ✅タップで詳細
            slot.OnClickSlot += (clickedSoubi) =>
            {
                equipmentDetailPanelZukan.ShowSoubi(clickedSoubi);
            };
        }

        foreach (Transform child in dropTitleParent)
            Destroy(child.gameObject);

        foreach (var title in data.dropShogoList)
        {
            var btn = Instantiate(titleButtonPrefab, dropTitleParent);

            // ✅取得済みか？
            bool unlocked =
    ZukanProgressManager.Instance.IsObtainedTitle(title.id);


            // ❌未取得なら？？？
            if (!unlocked)
            {
                btn.SetSecret();
                continue;
            }

            // ✅取得済みならボタンとして表示
            btn.Setup(title, rarityTable, equipmentDetailPanelZukan);
        }



        void OnClickEquipment(SoubiData soubi)
        {
            equipmentDetailPanelZukan.ShowSoubi(soubi);
        }

    }
}
