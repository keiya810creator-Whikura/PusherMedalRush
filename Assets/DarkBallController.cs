using UnityEngine;
using System.Collections;

public class DarkBallController : MonoBehaviour
{
    [Header("Move Settings")]
    [SerializeField] private float speed = 6f;

    [Header("Speed Random Range")]
    [SerializeField] private float speedRandomRange = 2f;
    // ✅speed=6なら → 4〜8になる

    [Header("Wobble Settings")]
    [SerializeField] private float wobbleStrength = 0.4f;
    [SerializeField] private float wobbleSpeed = 6f;

    private GameObject targetEnemy;

    // ✅実際に使う速度（毎回ランダム確定）
    private float finalSpeed;

    // ✅揺れ用ランダム軸
    private Vector2 wobbleSeed;

    void Start()
    {
        AudioManager.Instance.PlaySE(AudioManager.Instance.darkBall);
        // ✅速度をランダム確定
        finalSpeed = Random.Range(
            speed - speedRandomRange,
            speed + speedRandomRange
        );

        // ✅揺れのランダムパターン決定
        wobbleSeed = Random.insideUnitCircle * 10f;

        // ✅敵探索開始
        StartCoroutine(TargetLoop());
    }

    void Update()
    {
        if (targetEnemy == null) return;

        // ✅敵方向へ進む（ランダム速度）
        Vector3 dir =
            (targetEnemy.transform.position - transform.position).normalized;

        transform.position += dir * finalSpeed * Time.deltaTime;

        // ✅闇っぽい揺れ
        float wobbleX =
            Mathf.PerlinNoise(Time.time * wobbleSpeed, wobbleSeed.x) - 0.5f;
        float wobbleY =
            Mathf.PerlinNoise(wobbleSeed.y, Time.time * wobbleSpeed) - 0.5f;

        Vector3 wobbleOffset =
            new Vector3(wobbleX, wobbleY, 0f) * wobbleStrength;

        transform.position += wobbleOffset * Time.deltaTime;

        // ✅命中判定
        if (Vector3.Distance(transform.position, targetEnemy.transform.position) < 0.3f)
        {
            OnHit();
        }
    }

    // -----------------------------
    // ✅敵監視ループ（倒れたら停止→次の敵へ）
    // -----------------------------
    IEnumerator TargetLoop()
    {
        while (true)
        {
            if (targetEnemy == null)
            {
                targetEnemy = FindClosestEnemy();
            }

            yield return new WaitForSeconds(0.2f);
        }
    }

    // -----------------------------
    // ✅命中処理
    // -----------------------------
    void OnHit()
    {
        if (targetEnemy != null)
        {
            targetEnemy.GetComponent<Enemy>().TakeDamage(
                Mathf.CeilToInt(BattleManager.Instance.Status.Attack * 5.3f * BetManager.Instance.CurrentBet * BattleManager.Instance.Status.CriticalDamageRate),
                transform.position,
                false,
                true,
                0
            );

            AudioManager.Instance.PlaySE(AudioManager.Instance.skillHit);
        }

        Destroy(gameObject);
    }

    // -----------------------------
    // ✅一番近い敵を探す
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
            float dist = Vector3.Distance(transform.position, e.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = e;
            }
        }

        return closest;
    }
}