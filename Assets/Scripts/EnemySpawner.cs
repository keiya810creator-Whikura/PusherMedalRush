using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private MonsterDatabase monsterDatabase;

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
        // ★現在ステージ取得
        int stage = SaveManager.Instance.Data.currentStage;
        string stageId = "Stage" + stage;

        // =========================
        // Wave1500 固定敵
        // =========================
        if (currentWave == 1500)
        {
            const int oneBasedIndex = 151;
            int index = oneBasedIndex - 1;

            if (monsterDatabase == null || monsterDatabase.monsterList == null)
            {
                Debug.LogError("monsterDatabase / monsterList null");
                return;
            }

            if (monsterDatabase.monsterList.Count <= index)
            {
                Debug.LogError($"monsterList不足 Count={monsterDatabase.monsterList.Count}");
                return;
            }

            MonsterData selectedSpecial = monsterDatabase.monsterList[index];

            ZukanProgressManager.Instance.RecordEncounter(selectedSpecial.id);

            GameObject enemy =
                Instantiate(selectedSpecial.prefab, transform.position, Quaternion.identity);

            long hp = WaveManager.Instance.GetEnemyHP();
            enemy.GetComponent<Enemy>().Init(selectedSpecial, hp);

            return;
        }

        List<MonsterData> candidates;

        // =========================
        // Wave1001以降
        // =========================

        if (currentWave >= 1001)
        {
            candidates = monsterDatabase.monsterList
                .Where(m => m.stageId == stageId && !m.isBoss)
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
                    m.stageId == stageId &&
                    currentWave >= m.minWave &&
                    currentWave <= m.maxWave
                ).ToList();
        }

        if (candidates.Count == 0)
        {
            Debug.LogError($"No monster for wave {currentWave} stage {stageId}");
            return;
        }

        int spawnCount = (stage == 2) ? 2 : 1;

        for (int i = 0; i < spawnCount; i++)
        {
            MonsterData selected;

            if (WaveManager.Instance.IsBossWave())
            {
                MonsterData boss = candidates.FirstOrDefault(m => m.isBoss);
                selected = boss != null ? boss : SelectByWeight(candidates);
            }
            else
            {
                selected = SelectByWeight(candidates);
            }

            ZukanProgressManager.Instance.RecordEncounter(selected.id);

            Vector3 spawnPos = transform.position;

            if (spawnCount > 1)
            {
                spawnPos.x += (i == 0) ? -2f : 2f;
            }

            GameObject enemyObj =
                Instantiate(selected.prefab, spawnPos, Quaternion.identity);

            long hp = WaveManager.Instance.GetEnemyHP();
            enemyObj.GetComponent<Enemy>().Init(selected, hp);

            WaveManager.Instance.RegisterEnemy();
        }
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