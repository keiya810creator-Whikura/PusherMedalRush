using System.Collections.Generic;
using UnityEngine;

public class ZukanProgressManager : MonoBehaviour
{
    public static ZukanProgressManager Instance;

    private HashSet<string> encountered = new();
    private HashSet<string> obtainedEquip = new();
    private HashSet<string> obtainedTitle = new();

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        //DontDestroyOnLoad(gameObject);
    }

    // ============================
    // ✅遭遇
    // ============================
    public bool IsEncountered(string id)
        => encountered.Contains(id);

    public void RecordEncounter(string id)
    {
        if (encountered.Add(id))
        {
            Debug.Log("✅遭遇記録: " + id);
            FindAnyObjectByType<ZukanSaveBridge>()?.SaveZukan();
        }
    }

    // ============================
    // ✅取得（装備）
    // ============================
    public bool IsObtainedEquip(string id)
        => obtainedEquip.Contains(id);

    public void RecordEquip(string id)
    {
        if (obtainedEquip.Add(id))
        {
            Debug.Log("✅装備取得記録: " + id);
            FindAnyObjectByType<ZukanSaveBridge>()?.SaveZukan();
        }
    }

    // ============================
    // ✅取得（称号）
    // ============================
    public bool IsObtainedTitle(string id)
        => obtainedTitle.Contains(id);

    public void RecordTitle(string id)
    {
        if (obtainedTitle.Add(id))
        {
            Debug.Log("✅称号取得記録: " + id);
            FindAnyObjectByType<ZukanSaveBridge>()?.SaveZukan();
        }
    }

    // ✅ロード用
    public void SetAll(List<string> e1, List<string> e2, List<string> e3)
    {
        encountered = new HashSet<string>(e1);
        obtainedEquip = new HashSet<string>(e2);
        obtainedTitle = new HashSet<string>(e3);
    }

    public List<string> GetEncounteredList() => new(encountered);
    public List<string> GetEquipList() => new(obtainedEquip);
    public List<string> GetTitleList() => new(obtainedTitle);
}
