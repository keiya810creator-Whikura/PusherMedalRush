using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class Enemy : MonoBehaviour
{
    private MonsterData monsterData;

    [Header("HP")]
    public long maxHP;
    private long currentHP;

    [Header("UI")]
    [SerializeField] private Slider hpSlider;
    [SerializeField] private TextMeshProUGUI hpText;

    [SerializeField] private GameObject goldDropPrefab;
    [SerializeField] private RectTransform goldUITarget;

    [SerializeField] private GameObject soulPrefab;

    [SerializeField] private RarityColorTable rarityColorTable;
    [SerializeField] private GameObject soubiDropPrefab;
    [SerializeField] private GameObject syougouDropPrefab;

    [Header("Drop Spawn Offset")]
    [SerializeField] private Vector2 soubiSpawnOffset = new Vector2(-0.4f, 0.1f);
    [SerializeField] private Vector2 syougouSpawnOffset = new Vector2(0.4f, 0.1f);

    void Start()
    {
        // ゴールドUI（Image）の取得
        GameObject goldUIObj = GameObject.Find("ゴールドImage");
        if (goldUIObj != null)
        {
            goldUITarget = goldUIObj.GetComponent<RectTransform>();
        }
        else
        {
            Debug.LogError("ゴールドImage が見つかりません");
        }
        UpdateHPBar();
    }


    public void TakeDamage(
    long damage,
    Vector3 hitPoint,
    bool isCritical,
    bool isSkillHit,
    int medalMultiplier
)
    {
        if (isDead) return;

        currentHP -= damage;
        if (currentHP < 0)
            currentHP = 0;

        // ✅ダメージテキスト生成（倍率付き）
        if (DamageTextManager.Instance != null)
        {
            DamageTextManager.Instance.Spawn(
                damage,
                hitPoint,
                isCritical,
                isSkillHit,
                medalMultiplier
            );
        }

        UpdateHPBar();

        if (currentHP == 0)
            Die();
    }



    void UpdateHPBar()
    {
        if (hpSlider != null)
        {
            hpSlider.value = (float)currentHP / maxHP;
            hpText.text = currentHP.ToString("N0");
        }
    }
    private bool isDead = false;
    void Die()
    {
        if (isDead) return; // ✅二重死亡防止
        isDead = true;

        SkillManager.Instance.TriggerEnemyDefeated();
        DropLoot();

        GameObject soulObj = Instantiate(
            soulPrefab,
            transform.position,
            Quaternion.identity
        );

        // =========================
        // ゴールド倍率を反映
        // =========================
        long baseGold = WaveManager.Instance.GetEnemyGoldReward();
        float goldRate = BattleManager.Instance.Status.GoldDropRate;

        long totalGold =
            Mathf.Max(1, Mathf.RoundToInt(baseGold * goldRate));

        int visualCount =
            WaveManager.Instance.GetEnemyGoldDropVisualCount();

        SpawnGoldDrops(totalGold, visualCount);

        soulObj.GetComponent<Soul>().Init();
        Destroy(gameObject);
    }

    void DropLoot()
    {
        Vector3 target = DropAbsorbTarget.Instance.WorldPosition;
        Vector3 basePos = transform.position;

        var status = BattleManager.Instance.Status;
        // =========================
        // 装備（左）※即取得
        // =========================
        if (monsterData.dropSoubiList.Count > 0)
        {
            float finalSoubiRate =
                monsterData.soubiDropChance * status.EquipDropRate;

            int rollCount =
                SkillManager.Instance.GetEquipDropRollCount();

            for (int i = 0; i < rollCount; i++)
            {
                if (Random.value < finalSoubiRate)
                {
                    var soubi = monsterData.dropSoubiList[
                        Random.Range(0, monsterData.dropSoubiList.Count)
                    ];

                    if (AutoDisassembleSetting.EnableRarity[soubi.rarity])
                    {
                        //Debug.Log("分解済み");
                        // AddSoubiしない（対価なし）
                        return;
                    }

                    // ✅即取得
                    InventoryManager.Instance.AddSoubi(soubi);
                    ZukanProgressManager.Instance.RecordEquip(soubi.id);

                    // ✅演出のみ
                    Color color =
                        rarityColorTable.GetColor(soubi.rarity);

                    Vector3 spawnPos =
                        basePos + (Vector3)soubiSpawnOffset;

                    Instantiate(
                        soubiDropPrefab,
                        spawnPos,
                        Quaternion.identity
                    )
                    .GetComponent<SoubiDropEffect>()
                    .Init(soubi, color, target);
                }
            }
        }


        // =========================
        // 称号（右）
        // =========================
        if (monsterData.dropShogoList.Count > 0)
        {
            float finalShogoRate =
                monsterData.syougouDropChance * status.TitleDropRate;

            // ✅スキルで判定回数増加
            int rollCount =
                SkillManager.Instance.GetTitleDropRollCount();

            for (int i = 0; i < rollCount; i++)
            {
                if (Random.value < finalShogoRate)
                {
                    var syougou = monsterData.dropShogoList[
                        Random.Range(0, monsterData.dropShogoList.Count)
                    ];

                    // ✅即取得
                    InventoryManager.Instance.AddSyougou(syougou);
                    ZukanProgressManager.Instance.RecordTitle(syougou.id);

                    // ✅演出だけ
                    Color color =
                        rarityColorTable.GetColor(syougou.rarity);

                    Vector3 spawnPos =
                        basePos + (Vector3)syougouSpawnOffset;

                    Instantiate(
                        syougouDropPrefab,
                        spawnPos,
                        Quaternion.identity
                    )
                    .GetComponent<SyougouDropEffect>()
                    .Init(syougou, color, target);
                }
            }
        }
        // =========================
        // 深層称号ドロップ（Wave501+）
        // =========================
        TryDropAbyssTitle(basePos, target);
    }
    // =========================
    // 深層称号ドロップ（Wave501以降限定）
    // =========================
    // =========================
    // 深層称号ドロップ（Wave1001以降・段階解禁）
    // =========================
    void TryDropAbyssTitle(Vector3 basePos, Vector3 target)
    {
        int wave = WaveManager.Instance.CurrentWave;
        if (wave < 1001) return;

        // ✅深層称号Database取得
        var abyssDB = DatabaseManager.Instance.sinnsouSyougouDatabase;
        if (abyssDB == null) return;

        var abyssList = abyssDB.All;
        if (abyssList == null || abyssList.Count == 0) return;

        // =========================
        // 抽選対象レンジ決定
        // =========================
        int startIndex = 0;
        int count = 0;

        if (wave <= 1100)
        {
            startIndex = 0; count = 5;
        }
        else if (wave <= 1200)
        {
            startIndex = 5; count = 5;
        }
        else if (wave <= 1300)
        {
            startIndex = 10; count = 5;
        }
        else if (wave <= 1400)
        {
            startIndex = 15; count = 5;
        }
        else if (wave <= 1500)
        {
            startIndex = 20; count = 5;
        }
        else
        {
            startIndex = 0;
            count = abyssList.Count;
        }

        // 念のため範囲ガード
        if (startIndex >= abyssList.Count) return;
        count = Mathf.Min(count, abyssList.Count - startIndex);

        // =========================
        // ドロップ率（トレハン依存のみ）
        // =========================
        float dropRate = 0.0003f*BattleManager.Instance.Status.TitleDropRate;
        if (Random.value >= dropRate) return;

        // =========================
        // 称号選択
        // =========================
        var title =
            abyssList[Random.Range(startIndex, startIndex + count)];

        // ✅即取得
        InventoryManager.Instance.AddSyougou(title);
        ZukanProgressManager.Instance.RecordTitle(title.id);

        // ✅演出
        Color color = rarityColorTable.GetColor(title.rarity);
        Vector3 spawnPos = basePos + (Vector3)syougouSpawnOffset;

        Instantiate(
            syougouDropPrefab,
            spawnPos,
            Quaternion.identity
        )
        .GetComponent<SyougouDropEffect>()
        .Init(title, color, target);

        ToastManager.Instance.ShowToast(
            TextManager.Instance.GetUI("ui_toast_6")
        );
    }

    void SpawnGoldDrops(long totalGold, int maxVisualCount)
    {
        if (goldUITarget == null) return;

        int dropCount = Mathf.Min(maxVisualCount, (int)totalGold);

        long baseValue = totalGold / dropCount;
        long remainder = totalGold % dropCount;

        for (int i = 0; i < dropCount; i++)
        {
            long value = baseValue;

            // 端数を最初の数枚に足す
            if (i < remainder)
            {
                value += 1;
            }

            Vector3 offset = new Vector3(
                UnityEngine.Random.Range(-0.6f, 0.6f),
                UnityEngine.Random.Range(0.2f, 0.8f),
                0f
            );

            GameObject drop = Instantiate(
                goldDropPrefab,
                transform.position + offset,
                Quaternion.identity
            );

            drop.GetComponent<GoldDrop>()
                .Init(value, goldUITarget);
        }
    }
    public void Init(MonsterData data)
    {
        monsterData = data;

        // 今は WaveManager 依存のままでOK
        maxHP = WaveManager.Instance.GetEnemyHP();
        currentHP = maxHP;

        UpdateHPBar();
    }
    public void Init(MonsterData data, long hp)
    {
        monsterData = data;
        maxHP = hp;
        currentHP = hp;
    }

}
