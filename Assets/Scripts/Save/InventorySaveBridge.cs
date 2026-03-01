using System.Collections;
using System.Linq;
using UnityEngine;

public class InventorySaveBridge : MonoBehaviour
{
    [Header("Databases")]
    [SerializeField] private SoubiDatabase soubiDatabase;
    [SerializeField] private SyougouDatabase syougouDatabase;
    [SerializeField] private SyougouDatabase sinnsouSyougouDatabase;

    // =========================
    // ✅起動時ロード（Startで1フレーム待つ）
    // =========================
    private IEnumerator Start()
    {
        // ✅SingletonとSaveManager初期化待ち
        yield return null;

        LoadInventoryIntoRuntime();

        //Debug.Log("✅ロード後 soubi数=" +
           // InventoryManager.Instance.soubiList.Count);
    }

    // =========================
    // ✅セーブ
    // =========================
    public void SaveInventory()
    {
        var inv = InventoryManager.Instance;
        var data = SaveManager.Instance.Data;

        // nextInstanceId
        data.nextInstanceId = GetNextInstanceId(inv);

        // -------------------------
        // ✅装備インスタンス保存
        // -------------------------
        data.soubiInstances.Clear();

        foreach (var inst in inv.soubiList)
        {
            var s = new SoubiInstanceSaveData();

            s.instanceId = inst.instanceId;
            s.masterId = inst.master != null ? inst.master.id : "";

            // ✅装備状態も保存
            s.isEquipped = inst.isEquipped;
            s.equippedSlotId =
                inst.equippedSlotId.HasValue
                ? (int)inst.equippedSlotId.Value
                : -1;

            // ✅お気に入り
            s.isFavorite = inst.isFavorite;

            // ✅付与称号もIDで保存
            s.attachedTitleIds.Clear();
            if (inst.attachedTitles != null)
            {
                foreach (var t in inst.attachedTitles)
                {
                    if (t != null)
                        s.attachedTitleIds.Add(t.id);
                }
            }

            data.soubiInstances.Add(s);
        }

        // -------------------------
        // ✅称号スタック保存
        // -------------------------
        data.syougouStacks.Clear();

        foreach (var st in inv.syougouStacks)
        {
            if (st.data == null) continue;

            data.syougouStacks.Add(new SyougouStackSaveData
            {
                titleId = st.data.id,
                count = st.count
            });
        }

        SaveManager.Instance.SaveToDisk();
        //Debug.Log("✅Inventoryセーブ完了");
    }

    // =========================
    // ✅ロード（完全復元）
    // =========================
    public void LoadInventoryIntoRuntime()
    {
        var inv = InventoryManager.Instance;
        var data = SaveManager.Instance.Data;

        inv.isLoading = true;

        inv.soubiList.Clear();
        inv.syougouStacks.Clear();

        // -------------------------
        // ✅装備インスタンス復元
        // -------------------------
        foreach (var s in data.soubiInstances)
        {
            //Debug.Log("Load masterId=" + s.masterId);

            var master = soubiDatabase.FindById(s.masterId);
            if (master == null)
            {
                Debug.LogError("❌SoubiDatabaseに存在しないID: " + s.masterId);
                continue;
            }

            var inst = new SoubiInstance();

            inst.instanceId = s.instanceId;
            inst.master = master;

            // ✅装備状態復元
            inst.isEquipped = s.isEquipped;

            inst.equippedSlotId =
                (s.equippedSlotId >= 0)
                ? (EquipSlotId?)((EquipSlotId)s.equippedSlotId)
                : null;

            // ✅お気に入り復元
            inst.isFavorite = s.isFavorite;

            // ✅付与称号復元
            inst.attachedTitles.Clear();

            // ✅付与称号復元（通常＋深層対応）
            inst.attachedTitles.Clear();

            if (s.attachedTitleIds != null)
            {
                foreach (var tid in s.attachedTitleIds)
                {
                    // ① 通常称号DB
                    var title = syougouDatabase.FindById(tid);

                    // ② 無ければ深層称号DB
                    if (title == null)
                    {
                        title = sinnsouSyougouDatabase.FindById(tid);
                    }

                    // ③ 両方無ければエラー
                    if (title == null)
                    {
                        Debug.LogError("❌装備付与称号IDがどちらのDBにも存在しません: " + tid);
                        continue;
                    }

                    inst.attachedTitles.Add(title);
                }
            }

            inv.soubiList.Add(inst);
        }

        // -------------------------
        // ✅称号スタック復元
        // -------------------------
        // -------------------------
        // ✅称号スタック復元（通常＋深層対応）
        // -------------------------
        foreach (var st in data.syougouStacks)
        {
            // ✅まず通常称号DBで探す
            var title = syougouDatabase.FindById(st.titleId);

            // ✅無ければ深層称号DBでも探す
            if (title == null)
            {
                title = sinnsouSyougouDatabase.FindById(st.titleId);
            }

            // ✅両方無ければエラー
            if (title == null)
            {
                Debug.LogError("❌称号IDがどちらのDBにも存在しません: " + st.titleId);
                continue;
            }

            inv.syougouStacks.Add(new InventoryManager.SyougouStack
            {
                data = title,
                count = st.count
            });
        }

        // ✅nextInstanceId 更新
        SetNextInstanceId(inv, GetNextInstanceId(inv));

        inv.isLoading = false;

        // ✅UI更新
        //EquipmentGridView.Instance.RefreshVisibleSlotUI();
        //EquippedSlotsPanel.Instance.Refresh();

        // ✅ステータス再計算
        PlayerStatusDatabase.Instance.RecalculateFromEquipments();

        //Debug.Log("✅Inventoryロード完全復元完了");
    }

    // =========================
    // nextInstanceId 管理
    // =========================
    int GetNextInstanceId(InventoryManager inv)
    {
        if (inv.soubiList.Count == 0) return 0;
        return inv.soubiList.Max(s => s.instanceId) + 1;
    }

    void SetNextInstanceId(InventoryManager inv, int next)
    {
        inv.SetNextInstanceId(next);
    }
}
