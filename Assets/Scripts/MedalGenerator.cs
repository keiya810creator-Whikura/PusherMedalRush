using UnityEngine;
using System.Collections;

public class MedalGenerator : MonoBehaviour
{
    [Header("Auto Run Random X Range")]
    [SerializeField] private float autoFireMinX = -5f;
    [SerializeField] private float autoFireMaxX = 5f;

    [Header("Spawn Y Position")]
    public float spawnY = 4.5f;

    [Header("消費メダル")]
    public long consumeMedal = 1;

    [Header("連射間隔（秒）")]
    public float fireInterval = 0.1f;

    private float nextFireTime = 0f;
    private float autoTimer;

    [Header("Medal Prefabs (Normal倍率)")]
    [SerializeField] private GameObject normalMedalPrefab;
    [SerializeField] private GameObject medal2xPrefab;
    [SerializeField] private GameObject medal5xPrefab;
    [SerializeField] private GameObject medal10xPrefab;

    public static MedalGenerator Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        MoneyManager.instance.medal = BattleManager.Instance.Status.MaxMedal;
        MedalTextUI.Instance.UpdateText(MoneyManager.instance.medal);
    }

    private void Update()
    {
        // =========================
        // 自動周回：一定間隔で発射
        // =========================
        if (AdventureSession.IsAutoRun)
        {
            autoTimer += Time.deltaTime;
            if (autoTimer >= fireInterval / WaveManager.Instance.CurrentGameSpeed)
            {
                autoTimer = 0f;
                Fire(ignoreInterval: true);
            }
        }

        // =========================
        // タップ（即発射）
        // =========================
        if (Input.GetMouseButtonDown(0))
        {
            Fire(ignoreInterval: true);
        }

        // =========================
        // 長押し（連射間隔あり）
        // =========================
        if (Input.GetMouseButton(0))
        {
            if (Time.time < nextFireTime)
                return;

            Fire(ignoreInterval: false);
        }
    }

    private void Fire(bool ignoreInterval)
    {
        // ✅回収Waveならメダル消費なし＆残弾0でも撃てる
        bool isRecovery =
            WaveManager.Instance.CurrentState == WaveState.Recovery;

        var status = BattleManager.Instance.Status;

        // ✅Wave消費
        long baseConsume = WaveManager.Instance.GetConsumeMedalPerShot();

        // ✅ベット倍率
        int bet = BetManager.Instance.CurrentBet;

        // ✅最終消費メダル
        consumeMedal = baseConsume * bet;

        int shotCount = Mathf.Max(1, status.ShotCount);

        // ✅戦闘Waveだけ残弾不足で撃てない
        if (!isRecovery && MoneyManager.instance.Medal < consumeMedal)
            return;

        if (!ignoreInterval && Time.time < nextFireTime)
            return;

        // =========================
        // 消費ゼロ判定（Fire単位）
        // =========================
        bool noConsume = Random.value < status.MedalConsumeZeroRate;

        // ✅回収Waveでは必ず消費なし
        if (isRecovery) noConsume = true;

        if (!noConsume)
        {
            long medal = MoneyManager.instance.Medal;

            int maxPossibleShots = (int)(medal / consumeMedal);
            if (maxPossibleShots <= 0) return;

            shotCount = Mathf.Min(shotCount, maxPossibleShots);

            long totalConsume = consumeMedal * shotCount;
            MoneyManager.instance.ConsumeMedal(totalConsume);
        }

        // =========================
        // ✅shotCount分だけ発射
        // =========================
        for (int i = 0; i < shotCount; i++)
        {
            // ✅特殊弾抽選（確率込みで返る）
            SkillData specialSkill = null;

            if (SkillManager.Instance != null)
                specialSkill = SkillManager.Instance.TryGetFireSpecialSkill();

            GameObject prefabToShoot;
            ActiveSkillData linked = null;

            if (specialSkill != null)
            {
                prefabToShoot = specialSkill.specialMedalPrefab;
                linked = specialSkill.activeSkill;
            }
            else
            {
                prefabToShoot = DecideMedalPrefab(status);
            }

            if (prefabToShoot == null)
            {
                Debug.LogError("❌ prefabToShoot が null です（Prefab未設定）");
                continue;
            }

            // ✅PrefabからPool取得（自動Pool方式）
            if (MedalPoolManager.Instance == null)
            {
                Debug.LogError("❌ MedalPoolManager.Instance が存在しません！");
                return;
            }

            GameObject medalObj = MedalPoolManager.Instance.GetMedal(prefabToShoot);
            if (medalObj == null) return;

            // ✅linkedSkill仕込み（特殊弾だけ）
            Medal medal = medalObj.GetComponent<Medal>();
            if (medal != null)
                medal.linkedSkill = linked;

            // ✅位置セット
            Vector3 pos = GetFirePosition();

            float spread = 0.4f;
            float offsetX = (i - (shotCount - 1) / 2f) * spread;

            medalObj.transform.position = new Vector3(
                pos.x + offsetX,
                pos.y,
                pos.z
            );
        }
        SkillManager.Instance.TriggerMedalShot();
        AudioManager.Instance.PlaySE(AudioManager.Instance.shotSE);
        nextFireTime = Time.time + fireInterval / WaveManager.Instance.CurrentGameSpeed;
    }

    // ✅倍率メダルのPrefab選択
    private GameObject DecideMedalPrefab(BattleStatus status)
    {
        float r = Random.value;

        if (r < status.Attack10xMedalRate)
            return medal10xPrefab;

        r -= status.Attack10xMedalRate;

        if (r < status.Attack5xMedalRate)
            return medal5xPrefab;

        r -= status.Attack5xMedalRate;

        if (r < status.Attack2xMedalRate)
            return medal2xPrefab;

        return normalMedalPrefab;
    }

    private Vector3 GetFirePosition()
    {
        // AutoRun：ランダムX
        if (AdventureSession.IsAutoRun)
        {
            float randomX = Random.Range(autoFireMinX, autoFireMaxX);
            return new Vector3(randomX, spawnY, 0f);
        }

        // 手動：マウス位置
        Vector3 screenPos = Input.mousePosition;
        screenPos.z = Mathf.Abs(Camera.main.transform.position.z);

        Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
        return new Vector3(worldPos.x, spawnY, 0f);
    }

    // =========================
    // ✅スキル用：ノーマルメダル排出
    // =========================
    private int pendingMedals = 0;
    private Coroutine spawnRoutine;
    private SkillToastUIController toastUI;
    public void SpawnMedals(int amount)
    {
        if (amount <= 0) return;

        // ✅残り払い出しに加算
        pendingMedals += amount;

        // ✅トースト生成（なければ）
        if (toastUI == null)
        {
            toastUI =
                SkillToastManager.Instance.ShowSkillToastManual(
                    $"Win: {pendingMedals}"
                );
        }

        // ✅Coroutineが動いていなければ開始
        if (spawnRoutine == null)
            spawnRoutine = StartCoroutine(SpawnRoutine());
    }
    private IEnumerator SpawnRoutine()
    {
        while (pendingMedals > 0)
        {
            // ✅表示更新
            toastUI.UpdateText(
                $"Win: {pendingMedals}"
            );

            // ✅メダル排出
            GameObject medalObj =
                MedalPoolManager.Instance.GetMedal(normalMedalPrefab);

            AudioManager.Instance.PlaySE(AudioManager.Instance.medalSpawn);
            if (medalObj != null)
            {
                Vector3 pos = new Vector3(
                    6,
                    spawnY,
                    0f
                );

                medalObj.transform.position = pos;

                Medal medal = medalObj.GetComponent<Medal>();
                if (medal != null)
                    medal.linkedSkill = null;
            }

            // ✅残り枚数を減らす
            pendingMedals--;

            // ✅排出間隔
            yield return new WaitForSeconds(0.1f);
        }

        // ✅0になったら消す
        toastUI.UpdateText($"Win: 0");

        toastUI.AllowClose();
        yield return toastUI.CloseRoutine();

        // ✅リセット
        toastUI = null;
        spawnRoutine = null;
    }
    private IEnumerator SpawnMedalsRoutine(int amount)
    {
        if (MedalPoolManager.Instance == null)
        {
            Debug.LogError("❌ MedalPoolManager.Instance が存在しません！");
            yield break;
        }

        // ✅スキルトースト生成
        SkillToastUIController toast =
            SkillToastManager.Instance.ShowSkillToastManual(
                $"WIN : {amount}"
            );

        for (int i = amount; i > 0; i--)
        {
            // =======================
            // ✅トースト更新
            // =======================
            toast.UpdateText($"WIN : {i}");

            // =======================
            // ✅メダル排出
            // =======================
            GameObject medalObj =
                MedalPoolManager.Instance.GetMedal(normalMedalPrefab);

            if (medalObj == null) yield break;

            Vector3 pos = new Vector3(
                6,
                spawnY,
                0f
            );

            medalObj.transform.position = pos;

            Medal medal = medalObj.GetComponent<Medal>();
            if (medal != null)
                medal.linkedSkill = null;

            // ✅0.3秒間隔で排出
            yield return new WaitForSeconds(0.15f);
        }

        // ✅最後はWin:0表示
        toast.UpdateText($"WIN : 0");

        // ✅消える
        toast.AllowClose();
        yield return toast.CloseRoutine();
    }
}