using UnityEngine;
using System.Collections.Generic;
using System.Linq;
public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    public List<SoubiInstance> soubiList = new();
    [System.Serializable]
    public class SyougouStack
    {
        public SyougouData data;
        public int count;
    }
    public List<SyougouStack> syougouStacks = new();


    private int nextInstanceId = 0;

    public event System.Action OnEquipmentChanged;
    public bool isDismantleMode;
    public bool isLoading;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        //DontDestroyOnLoad(gameObject);
    }
    private InventorySaveBridge saveBridge;
    public int maxInventory = 300;

    private void Start()
    {
        saveBridge = FindAnyObjectByType<InventorySaveBridge>();
    }
    public SoubiInstance AddSoubi(SoubiData master)
    {
        var inst = new SoubiInstance
        {
            instanceId = nextInstanceId++,
            master = master
        };
        soubiList.Add(inst);
        ResultManager.Instance.AddGainedEquipment(inst);
        NotifyEquipmentChanged();
        return inst;
    }

    public void AddSyougou(SyougouData data, int amount = 1)
    {
        var stack = syougouStacks.Find(s => s.data == data);
        if (stack != null)
        {
            stack.count += amount;
        }
        else
        {
            syougouStacks.Add(new SyougouStack
            {
                data = data,
                count = amount
            });
        }
        ResultManager.Instance.AddGainedTitle(data);
        NotifyEquipmentChanged();
    }

    public void EquipSoubi(SoubiInstance target)
    {
        target.isEquipped = true;
        NotifyEquipmentChanged();
    }

    public void UnequipSoubi(SoubiInstance target)
    {
        target.isEquipped = false;
        NotifyEquipmentChanged();
    }
    public const int MAX_EQUIP = 5;

    public bool CanEquip()
    {
        return soubiList.Count(s => s.isEquipped) < MAX_EQUIP;
    }

    public bool Equip(SoubiInstance soubi)
    {
        if (soubi.isEquipped)
            return true;

        // 空きスロットを探す
        foreach (EquipSlotId id in System.Enum.GetValues(typeof(EquipSlotId)))
        {
            bool used = soubiList.Any(s =>
                s.isEquipped && s.equippedSlotId == id);

            if (!used)
            {
                soubi.isEquipped = true;
                soubi.equippedSlotId = id;

                // ★ 追加
                EquipmentGridView.Instance.RefreshVisibleSlotUI();


                PlayerStatusDatabase.Instance.RecalculateFromEquipments();
                EquippedSlotsPanel.Instance.Refresh();
                OnEquipmentChanged?.Invoke(); // ★追加
                saveBridge.SaveInventory();

                return true;
            }
        }
        Debug.Log("装備スロットが満杯です");
        saveBridge.SaveInventory();
        return false;
    }


    public void Unequip(SoubiInstance soubi)
    {
        if (!soubi.isEquipped)
            return;

        soubi.isEquipped = false;
        soubi.equippedSlotId = null;

        // ★ 追加
        EquipmentGridView.Instance.RefreshVisibleSlotUI();

        PlayerStatusDatabase.Instance.RecalculateFromEquipments();
        OnEquipmentChanged?.Invoke(); // ★追加
        EquippedSlotsPanel.Instance.Refresh();
        saveBridge.SaveInventory();

    }

    private void RecalculatePlayerStatus()
    {
        PlayerStatusDatabase.Instance.RecalculateFromEquipments();
    }

    public void NotifyEquipmentChanged()
    {
        OnEquipmentChanged?.Invoke();

        // ✅ロード中は絶対セーブ禁止
        if (isLoading) return;

        saveBridge?.SaveInventory();
    }




    public bool CanDisassemble(SoubiInstance soubi)
    {
        if (soubi == null) return false;
        if (soubi.isEquipped) return false;
        if (soubi.isFavorite) return false;
        return true;
    }

    public void Disassemble(SoubiInstance soubi)
    {
        // ① 称号返却
        if (soubi.attachedTitles != null)
        {
            foreach (var title in soubi.attachedTitles)
            {
                AddSyougou(title);
            }
            soubi.attachedTitles.Clear();
        }

        // ② 装備削除
        soubiList.Remove(soubi);
        //saveBridge?.SaveInventory();

    }


    public void DisassembleSelected()
    {
        var targets = soubiList
        .Where(s => s.isSelectedForDismantle)
        .ToList();

        foreach (var soubi in targets)
        {
            // ① 称号を返却
            if (soubi.attachedTitles != null)
            {
                foreach (var title in soubi.attachedTitles)
                {
                    AddSyougou(title);
                }

                soubi.attachedTitles.Clear();
            }

            // ② 装備を削除
            soubiList.Remove(soubi);
        }
        NotifyEquipmentChanged();
    }

    private int GetDisassembleGold(SoubiInstance soubi)
    {
        // 仮ルール：レアリティ × 100
        return soubi.master.rarity * 100;
    }
    public void ToggleSelectAllForDismantle()
    {
        var candidates = soubiList
            .FindAll(s => !s.isFavorite && s.isEquipped == false);

        bool allSelected =
            candidates.Count > 0 &&
            candidates.TrueForAll(s => s.isSelectedForDismantle);

        foreach (var s in candidates)
        {
            s.isSelectedForDismantle = !allSelected;
        }

        NotifyEquipmentChanged();
    }
    public void SetDismantleMode(bool on)
    {
        if (isDismantleMode == on)
            return;

        isDismantleMode = on;

        // OFF に戻すときは選択を全解除
        if (!on)
        {
            foreach (var s in soubiList)
                s.isSelectedForDismantle = false;
        }

        NotifyEquipmentChanged();
    }
    public void ClearAllDismantleSelection()
    {
        foreach (var s in soubiList)
        {
            s.isSelectedForDismantle = false;
        }
    }
    public void Disassemble(List<SoubiInstance> list)
    {
        foreach (var soubi in list)
            Disassemble(soubi);

        NotifyEquipmentChanged(); // ✅ここで1回だけSaveされる
    }

    public void SetNextInstanceId(int value)
    {
        nextInstanceId = value;
    }
    public int MaxStorage
    {
        get
        {
            return PurchaseState.StorageSize;
        }
    }
    public int TotalCount
    {
        get
        {
            return soubiList.Count;
        }
    }
    public void UnequipAll()
    {
        bool changed = false;

        foreach (var soubi in soubiList)
        {
            if (!soubi.isEquipped)
                continue;

            soubi.isEquipped = false;
            soubi.equippedSlotId = null;
            changed = true;
        }

        if (!changed)
            return;

        // UI & ステータス更新
        PlayerStatusDatabase.Instance.RecalculateFromEquipments();

        EquipmentGridView.Instance.RefreshVisibleSlotUI();
        EquippedSlotsPanel.Instance.Refresh();

        // イベント & セーブ（1回だけ）
        OnEquipmentChanged?.Invoke();

        if (!isLoading)
            saveBridge?.SaveInventory();
    }

}

