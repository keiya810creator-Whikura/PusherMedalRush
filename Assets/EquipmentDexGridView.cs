using UnityEngine;

public class EquipmentDexGridView : MonoBehaviour
{
    [Header("Database")]
    [SerializeField] private SoubiDatabase soubiDatabase;

    [Header("UI")]
    [SerializeField] private Transform contentParent;
    [SerializeField] private EquipmentSlotUIZukan slotPrefab;

    [Header("Detail Panel")]
    [SerializeField] private EquipmentDetailPanelZukan detailPanel;

    private void Start()
    {
        Build();
    }

    // ============================
    // ✅装備図鑑を並べる
    // ============================
    public void Build()
    {
        // ✅既存削除
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        // ✅全装備生成
        foreach (var soubi in soubiDatabase.All)
        {
            var slot = Instantiate(slotPrefab, contentParent);

            // ✅取得済み判定
            bool unlocked =
                ZukanProgressManager.Instance.IsObtainedEquip(soubi.id);

            // ✅スロットセットアップ
            slot.Setup(soubi, unlocked);

            // ✅タップで詳細を開く
            slot.OnClickSlot += OnClickSoubi;
        }
    }

    // ============================
    // ✅詳細パネル表示
    // ============================
    private void OnClickSoubi(SoubiData data)
    {
        detailPanel.ShowSoubi(data);
    }
}