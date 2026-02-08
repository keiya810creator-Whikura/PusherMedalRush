using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Game/Syougou Database")]
public class SyougouDatabase: ScriptableObject
{
    [Header("All Titles")]
    public List<SyougouData> shogoList = new();

    public IReadOnlyList<SyougouData> All => shogoList;

    public List<SyougouData> GetByRarity(int rarity)
    {
        return shogoList.FindAll(s => s.rarity == rarity);
    }

    public SyougouData GetRandomByRarity(int rarity)
    {
        var list = GetByRarity(rarity);
        if (list.Count == 0)
            return null;

        return list[Random.Range(0, list.Count)];
    }
    public SyougouData FindById(string id)
    {
        return shogoList.Find(x => x != null && x.id == id);
    }

}
