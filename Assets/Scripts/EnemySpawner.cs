using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private MonsterDatabase monsterDatabase;
    [SerializeField] private string currentStageId = "Stage1";
    public static EnemySpawner Instance;
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    public void SpawnEnemy(int currentWave)
    {
        // =========================
        // ✅ Wave1500：151番目のモンスターを固定出現（1始まり→index150）
        // =========================
        if (currentWave == 1500)
        {
            const int oneBasedIndex = 151;
            int index = oneBasedIndex - 1;

            if (monsterDatabase == null || monsterDatabase.monsterList == null)
            {
                Debug.LogError("monsterDatabase / monsterList が null です");
                return;
            }

            if (monsterDatabase.monsterList.Count <= index)
            {
                Debug.LogError($"monsterList に {oneBasedIndex}番目が存在しません。Count={monsterDatabase.monsterList.Count}");
                return;
            }

            MonsterData selected = monsterDatabase.monsterList[index];

            // 図鑑記録
            ZukanProgressManager.Instance.RecordEncounter(selected.id);

            // 生成
            GameObject enemy = Instantiate(selected.prefab, transform.position, Quaternion.identity);
            long hp = WaveManager.Instance.GetEnemyHP();
            enemy.GetComponent<Enemy>().Init(selected, hp);

            return; // ★ここで終わり（通常抽選に行かない）
        }

        // =========================
        // Wave1001以降：全敵ランダム（ボス無し）
        // =========================
        List<MonsterData> candidates;

        if (currentWave >= 1001)
        {
            candidates = monsterDatabase.monsterList
                .Where(m => m.stageId == currentStageId && !m.isBoss)
                .ToList();

            if (candidates.Count == 0)
            {
                candidates = monsterDatabase.monsterList
                    .Where(m => !m.isBoss)
                    .ToList();
            }
        }
        else
        {
            candidates = monsterDatabase.monsterList
                .Where(m =>
                    m.stageId == currentStageId &&
                    currentWave >= m.minWave &&
                    currentWave <= m.maxWave
                ).ToList();
        }

        if (candidates.Count == 0)
        {
            Debug.LogError($"No monster for wave {currentWave}");
            return;
        }

        MonsterData selectedNormal;

        if (WaveManager.Instance.IsBossWave())
        {
            MonsterData boss = candidates.FirstOrDefault(m => m.isBoss);
            selectedNormal = boss != null ? boss : SelectByWeight(candidates);
        }
        else
        {
            selectedNormal = SelectByWeight(candidates);
        }

        ZukanProgressManager.Instance.RecordEncounter(selectedNormal.id);

        GameObject enemyObj = Instantiate(selectedNormal.prefab, transform.position, Quaternion.identity);
        long hp2 = WaveManager.Instance.GetEnemyHP();
        enemyObj.GetComponent<Enemy>().Init(selectedNormal, hp2);
    }



    // =========================
    // 重み抽選
    // =========================

    private MonsterData SelectByWeight(List<MonsterData> list)
    {
        int total = list.Sum(m => m.syutugennritu);
        int rand = Random.Range(0, total);

        int current = 0;
        foreach (var m in list)
        {
            current += m.syutugennritu;
            if (rand < current)
                return m;
        }

        return list[0];
    }
}
