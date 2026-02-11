using UnityEngine;
using System.Collections;

public class LightningController : MonoBehaviour
{
    [Header("Move Settings")]
    [SerializeField] private float speed = 18f;

    [Header("Lifetime Settings")]
    [SerializeField] private float lifeTime = 7f; // ✅7秒で消える

    [Header("Retreat Settings")]
    [SerializeField] private float retreatDistance = 1.2f;
    [SerializeField] private float retreatTime = 0.12f;

    [Header("Target Refresh")]
    [SerializeField] private float retargetInterval = 0.2f;

    private Transform target;
    private Vector3 moveDirection = Vector3.up;

    private bool isRetreating = false;

    void Start()
    {
        // ✅7秒で自動消滅
        Destroy(gameObject, lifeTime);

        // ✅敵探索開始
        StartCoroutine(TargetLoop());

        // ✅発射SE
        AudioManager.Instance.PlaySE(AudioManager.Instance.lightning);
    }

    void Update()
    {
        // ✅離脱中は方向更新しない
        if (!isRetreating && target != null)
        {
            Vector3 desiredDir =
                (target.position - transform.position).normalized;

            moveDirection = Vector3.Lerp(
                moveDirection,
                desiredDir,
                Time.deltaTime * 10f
            );
        }

        // ✅常に前進
        transform.position += moveDirection * speed * Time.deltaTime;
    }

    // -----------------------------
    // ✅敵探索ループ
    // -----------------------------
    IEnumerator TargetLoop()
    {
        while (true)
        {
            if (target == null)
            {
                GameObject enemy = FindClosestEnemy();
                if (enemy != null)
                {
                    target = enemy.transform;
                }
            }

            yield return new WaitForSeconds(retargetInterval);
        }
    }

    // -----------------------------
    // ✅敵ヒット処理（無限チェイン）
    // -----------------------------
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Enemy")) return;
        if (isRetreating) return;

        // ✅ダメージ（Attack100%）
        DealDamage(collision.gameObject);

        // ✅命中SE
        AudioManager.Instance.PlaySE(AudioManager.Instance.skillHit);

        // ✅次の敵へターゲット更新
        GameObject nextEnemy = FindClosestEnemy();
        target = (nextEnemy != null) ? nextEnemy.transform : null;

        // ✅滑らか離脱開始
        StartCoroutine(RetreatSmooth(collision.transform.position));
    }

    // -----------------------------
    // ✅滑らか離脱
    // -----------------------------
    IEnumerator RetreatSmooth(Vector3 enemyPos)
    {
        isRetreating = true;

        Vector3 startPos = transform.position;
        Vector3 awayDir = (transform.position - enemyPos).normalized;
        Vector3 endPos = startPos + awayDir * retreatDistance;

        float elapsed = 0f;

        while (elapsed < retreatTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / retreatTime;

            transform.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }

        isRetreating = false;
    }

    // -----------------------------
    // ✅ダメージ処理
    // -----------------------------
    void DealDamage(GameObject enemyObj)
    {
        Enemy enemy = enemyObj.GetComponent<Enemy>();
        if (enemy == null) return;

        long damage = BattleManager.Instance.Status.Attack * BetManager.Instance.CurrentBet;

        enemy.TakeDamage(
            damage,
            enemy.transform.position,
            false,
            true,
                0
        );
    }

    // -----------------------------
    // ✅最も近い敵を探す
    // -----------------------------
    GameObject FindClosestEnemy()
    {
        GameObject[] enemies =
            GameObject.FindGameObjectsWithTag("Enemy");

        if (enemies.Length == 0) return null;

        GameObject closest = null;
        float minDist = float.MaxValue;

        foreach (var e in enemies)
        {
            float dist =
                Vector3.Distance(transform.position, e.transform.position);

            if (dist < minDist)
            {
                minDist = dist;
                closest = e;
            }
        }

        return closest;
    }
}