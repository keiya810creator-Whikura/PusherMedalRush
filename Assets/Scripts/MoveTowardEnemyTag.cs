using UnityEngine;

public class MoveTowardEnemyTag : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private string enemyTag = "Enemy";
    [SerializeField] private bool reacquireTargetContinuously = true;

    [Header("Move")]
    [SerializeField] private float moveSpeed = 6f;

    [Header("Homing Curve")]
    [SerializeField] private float turnSpeed = 8f;

    [Header("No Enemy Goal")]
    [SerializeField] private Vector3 goalRight = new Vector3(5.7f, 9.2f, 0f);
    [SerializeField] private Vector3 goalLeft = new Vector3(-5.7f, 9.2f, 0f);

    [Header("Medal Prefabs（倍率排出用）")]
    [SerializeField] private GameObject normalMedalPrefab;
    [SerializeField] private GameObject medal2xPrefab;
    [SerializeField] private GameObject medal5xPrefab;
    [SerializeField] private GameObject medal10xPrefab;

    private Transform _target;
    private Vector3 currentDir;

    private bool goingToGoal = false;
    private Vector3 goalPos;

    private int multiplier = 1;

    // =========================
    // ✅Pool再利用初期化
    // =========================
    private void OnEnable()
    {
        goingToGoal = false;
        _target = null;

        currentDir = transform.up;

        MedalMultiplier multi = GetComponent<MedalMultiplier>();
        multiplier = (multi != null) ? Mathf.Max(1, multi.multiplier) : 1;

        AcquireNearestTarget();

        if (_target == null)
            StartGoalMode();
    }

    private void Update()
    {
        // ✅回収Wave中は即消す
        if (WaveManager.Instance.CurrentState == WaveState.Recovery)
        {
            ReturnSelf();
            return;
        }

        // ✅ゴールへ向かう
        if (goingToGoal)
        {
            MoveToGoal();
            return;
        }

        // ✅ターゲット再探索
        if (reacquireTargetContinuously || _target == null)
            AcquireNearestTarget();

        if (_target == null)
        {
            StartGoalMode();
            return;
        }

        // ✅ホーミング移動
        Vector3 targetDir =
            (_target.position - transform.position).normalized;

        currentDir = Vector3.Lerp(
            currentDir,
            targetDir,
            turnSpeed * Time.deltaTime
        ).normalized;

        transform.position +=
            currentDir * moveSpeed * Time.deltaTime *
            WaveManager.Instance.CurrentGameSpeed;

        RotateToward(currentDir);
    }

    // =========================
    // ✅ゴールモード開始
    // =========================
    private void StartGoalMode()
    {
        goingToGoal = true;
        goalPos = (Random.value < 0.5f) ? goalLeft : goalRight;
    }

    // =========================
    // ✅ゴールへ直線移動
    // =========================
    private void MoveToGoal()
    {
        float step =
            moveSpeed * Time.deltaTime *
            WaveManager.Instance.CurrentGameSpeed;

        transform.position =
            Vector3.MoveTowards(transform.position, goalPos, step);

        Vector3 dir = (goalPos - transform.position).normalized;
        RotateToward(dir);

        if (Vector3.Distance(transform.position, goalPos) < 0.05f)
        {
            CollectMedalsAndDisappear();
        }
    }

    // =========================
    // ✅倍率メダル排出して消える
    // =========================
    private void CollectMedalsAndDisappear()
    {
        GameObject prefab = GetMedalPrefabByMultiplier();

        if (prefab != null && MedalPoolManager.Instance != null)
        {
            GameObject medalObj =
                MedalPoolManager.Instance.GetMedal(prefab);

            if (medalObj != null)
                medalObj.transform.position = transform.position;
        }

        ReturnSelf();
    }

    // =========================
    // ✅倍率 → Prefab取得
    // =========================
    private GameObject GetMedalPrefabByMultiplier()
    {
        return multiplier switch
        {
            2 => medal2xPrefab,
            5 => medal5xPrefab,
            10 => medal10xPrefab,
            _ => normalMedalPrefab
        };
    }

    // =========================
    // ✅自分を安全に消す
    // =========================
    private void ReturnSelf()
    {
        if (FirePoolManager.Instance != null)
        {
            FirePoolManager.Instance.Return(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // =========================
    // ✅回転
    // =========================
    private void RotateToward(Vector3 dir)
    {
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
    }

    // =========================
    // ✅敵を探す
    // =========================
    private void AcquireNearestTarget()
    {
        GameObject[] enemies =
            GameObject.FindGameObjectsWithTag(enemyTag);

        if (enemies.Length == 0)
        {
            _target = null;
            return;
        }

        Transform best = null;
        float bestSqr = float.PositiveInfinity;
        Vector3 myPos = transform.position;

        foreach (var e in enemies)
        {
            if (e == null) continue;

            float d =
                (e.transform.position - myPos).sqrMagnitude;

            if (d < bestSqr)
            {
                bestSqr = d;
                best = e.transform;
            }
        }

        _target = best;
    }
}