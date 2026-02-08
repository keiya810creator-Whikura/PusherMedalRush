using UnityEngine;
using TMPro;
using System.Linq;

public class TitleSelectPanel : MonoBehaviour
{
    // ✅外部UIControllerが操作する正式なソート状態
    public TitleSortType currentSortType = TitleSortType.None;
    public SortOrder currentSortOrder = SortOrder.Descending;

    public static TitleSelectPanel Instance;

    [SerializeField] Transform content;
    [SerializeField] TitleRowUI rowPrefab;
    [SerializeField] RarityColorTable rarityColorTable;

    private SoubiInstance targetSoubi;
    private TitleSelectMode currentMode;

    public enum TitleSelectMode
    {
        Inventory,      // 通常インベントリ・装備詳細からの付与
        ResultGained    // リザルト獲得称号一覧（付与不可）
    }

    private void Awake()
    {
        Instance = this;
        gameObject.SetActive(false);

        // ✅Dropdownイベントはここでは管理しない！
        // TitleSortUIController側だけで管理する
    }
    public GameObject syougoututorialPanel;
    void Start()
    {
        if (TutorialManager.Instance.CanShow(TutorialID.TitleAssign))
            syougoututorialPanel.SetActive(true);
    }

    // ===============================
    // 表示エントリポイント
    // ===============================
    public void Show(SoubiInstance soubi, TitleSelectMode mode)
    {
        targetSoubi = soubi;
        currentMode = mode;

        gameObject.SetActive(true);

        RefreshList();
    }

    // ===============================
    // 一覧更新（ソート反映）
    // ===============================
    public void RefreshList()
    {
        foreach (Transform c in content)
            Destroy(c.gameObject);

        var query = InventoryManager.Instance.syougouStacks
            .Where(s => s.count > 0);

        // ✅昇順・降順をちゃんと切り替え
        query =
            currentSortOrder == SortOrder.Descending
                ? query.OrderByDescending(s => GetSortValue(s.data))
                : query.OrderBy(s => GetSortValue(s.data));

        var sorted = query.ToList();

        foreach (var stack in sorted)
        {
            var row = Instantiate(rowPrefab, content);

            row.Set(
                stack,
                OnSelectTitle,
                rarityColorTable,
                currentMode
            );
        }
    }

    // ===============================
    // ソート値取得
    // ===============================
    private float GetSortValue(SyougouData d)
    {
        // ✅currentSortではなく currentSortType を参照する！
        return currentSortType switch
        {
            // 火力系
            TitleSortType.AttackAdd => d.attackAdd,
            TitleSortType.AttackRate => d.attackRate,
            TitleSortType.SimultaneousShot => d.simultaneousShotRate,
            TitleSortType.CriticalRate => d.criticalAdd,
            TitleSortType.CriticalDamage => d.criticalDamageAdd,

            // 攻撃メダル倍率
            TitleSortType.AttackMedal2x => d.attackMedal2xAdd,
            TitleSortType.AttackMedal5x => d.attackMedal5xAdd,
            TitleSortType.AttackMedal10x => d.attackMedal10xAdd,

            // 耐久系
            TitleSortType.MaxMedalAdd => d.maxMedalAdd,
            TitleSortType.MaxMedalRate => d.maxMedalRate,
            TitleSortType.NoMedalConsume => d.noMedalConsumeAdd,

            // 回収・経済
            TitleSortType.RecoveryValue => d.recoveryWaveMedalValueAdd,
            TitleSortType.RecoveryDuration => d.recoveryWaveDurationAdd,
            TitleSortType.GoldDrop => d.goldDropAdd,

            // 収集系
            TitleSortType.EquipmentDrop => d.equipmentDropAdd,
            TitleSortType.TitleDrop => d.titleDropAdd,

            // その他
            TitleSortType.Rarity => d.rarity,

            _ => 0
        };
    }

    // ===============================
    // 称号選択時
    // ===============================
    private void OnSelectTitle(SyougouData data)
    {
        if (currentMode == TitleSelectMode.ResultGained)
            return;

        if (targetSoubi == null)
            return;

        // ============================
        // ✅すでに称号が付いていたら返却する
        // ============================
        if (targetSoubi.attachedTitles.Count > 0)
        {
            var oldTitle = targetSoubi.attachedTitles[0];

            // ✅装備から外す
            targetSoubi.attachedTitles.Clear();

            // ✅インベントリに返す
            InventoryManager.Instance.AddSyougou(oldTitle, 1);
            ToastManager.Instance.ShowToast(string.Format(TextManager.Instance.GetUI("ui_toast_1"), "《"+TextManager.Instance.GetTitle(oldTitle.nameKey)+"》"));
        }

        // ============================
        // ✅新しい称号を付与（1個だけ）
        // ============================
        targetSoubi.attachedTitles.Add(data);
        AudioManager.Instance.PlaySE(AudioManager.Instance.syougouHuyoSE);
        // ============================
        // ✅称号を消費
        // ============================
        var stack = InventoryManager.Instance.syougouStacks
            .Find(s => s.data == data);

        if (stack != null)
        {
            stack.count--;

            if (stack.count <= 0)
                InventoryManager.Instance.syougouStacks.Remove(stack);
        }

        // ============================
        // ✅UI更新
        // ============================
        InventoryManager.Instance.NotifyEquipmentChanged();
        EquipmentGridView.Instance.RefreshVisibleSlotUI();

        if (ResultPanel.IsOpen)
            EquipmentDetailPanel.Instance.ShowResult(targetSoubi);
        else
            EquipmentDetailPanel.Instance.Show(targetSoubi);

        if (EquippedSlotsPanel.Instance != null)
            EquippedSlotsPanel.Instance.Refresh();
        gameObject.SetActive(false);
    }


    public void Close()
    {
        targetSoubi = null;
        gameObject.SetActive(false);
    }
}
