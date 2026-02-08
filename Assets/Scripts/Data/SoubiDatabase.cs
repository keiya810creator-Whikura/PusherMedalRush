using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Game/Soubi Database")]
public class SoubiDatabase : ScriptableObject
{
    [Header("All Equipments")]
    public List<SoubiData> soubiList = new();

    /// <summary>
    /// 全装備を返す
    /// </summary>
    public IReadOnlyList<SoubiData> All => soubiList;

    /// <summary>
    /// ランダムに1つ取得（等確率）
    /// </summary>
    public SoubiData GetRandom()
    {
        if (soubiList == null || soubiList.Count == 0)
            return null;

        return soubiList[Random.Range(0, soubiList.Count)];
    }
    public SoubiData FindById(string id)
    {
        return soubiList.Find(x => x != null && x.id == id);
    }

}
