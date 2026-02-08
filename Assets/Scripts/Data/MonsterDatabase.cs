using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(menuName = "Game/Monster Database")]
public class MonsterDatabase : ScriptableObject
{
    [Header("All Monsters")]
    public List<MonsterData> monsterList = new();

    /// <summary>
    /// 全モンスター
    /// </summary>
    public IReadOnlyList<MonsterData> All => monsterList;

    /// <summary>
    /// Wave帯に出現可能なモンスター一覧
    /// </summary>
    public List<MonsterData> GetAvailableMonsters(int currentWave)
    {
        return monsterList
            .Where(m =>
                m.minWave <= currentWave &&
                currentWave <= m.maxWave
            )
            .ToList();
    }

    /// <summary>
    /// ボスモンスター取得（Wave条件込み）
    /// </summary>
    public MonsterData GetBoss(int currentWave)
    {
        return monsterList.FirstOrDefault(m =>
            m.isBoss &&
            m.minWave <= currentWave &&
            currentWave <= m.maxWave
        );
    }
}
