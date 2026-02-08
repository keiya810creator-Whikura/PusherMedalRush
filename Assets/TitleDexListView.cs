using UnityEngine;

public class TitleDexListView : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Transform contentParent;
    [SerializeField] private TitleTextButtonZukan buttonPrefab;

    [Header("Tables")]
    [SerializeField] private RarityColorTable rarityColorTable;

    [Header("Detail Panel")]
    [SerializeField] private EquipmentDetailPanelZukan detailPanel;

    private void Start()
    {
        Build();
    }

    // ============================
    // ✅称号図鑑を並べる
    // ============================
    public void Build()
    {
        // ✅既存削除
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        // ============================
        // ✅通常称号を先に生成
        // ============================
        foreach (var title in DatabaseManager.Instance.ShogoDB.All)
        {
            CreateSlot(title);
        }

        // ============================
        // ✅深層称号を末尾に追加生成
        // ============================
        foreach (var abyssTitle in DatabaseManager.Instance.SinsouSyougouShogoDB.All)
        {
            CreateSlot(abyssTitle);
        }
    }

    // ============================
    // ✅1スロット生成処理
    // ============================
    private void CreateSlot(SyougouData data)
    {
        var slot = Instantiate(buttonPrefab, contentParent);

        // ✅取得済み判定
        bool unlocked =ZukanProgressManager.Instance.IsObtainedTitle(data.id);


        if (unlocked)
        {
            slot.Setup(data, rarityColorTable, detailPanel);
        }
        else
        {
            slot.SetSecret();
        }
    }
}