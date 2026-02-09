using UnityEngine;
using System.Collections;

public class MeteoController : MonoBehaviour
{
    [Header("Move Settings")]
    [SerializeField] private float moveSpeed = 10f;

    [Header("Stay Settings")]
    [SerializeField] private float stayTime = 2f;

    [Header("Damage Settings")]
    [SerializeField] private float damageInterval = 0.5f;
    [SerializeField] private float damageRadius = 1.5f;
    [SerializeField] private long damagePerTick = 10;

    [Header("Lifetime Safety")]
    [SerializeField] private float maxLifeTime = 6f;

    private Vector3 targetPos;
    private bool reached = false;

    void Start()
    {
        // ✅寿命保険
        Destroy(gameObject, maxLifeTime);

        // ✅ターゲット位置（BattleManagerの位置）
        targetPos = BattleManager.Instance.gameObject.transform.position;

        // ✅落下SE
        AudioManager.Instance.PlaySE(AudioManager.Instance.meteo);

        // ✅移動開始
        StartCoroutine(MoveToTarget());
    }

    // -----------------------------
    // 指定位置へ移動
    // -----------------------------
    IEnumerator MoveToTarget()
    {
        while (Vector3.Distance(transform.position, targetPos) > 0.05f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPos,
                moveSpeed * Time.deltaTime
            );

            yield return null;
        }

        // ✅到達！
        reached = true;

        // ✅着弾SE
        AudioManager.Instance.PlaySE(AudioManager.Instance.skillHit);

        // ✅設置開始
        StartCoroutine(StayAndDamage());
    }

    // -----------------------------
    // 設置 → ダメージ継続
    // -----------------------------
    IEnumerator StayAndDamage()
    {
        float elapsed = 0f;

        while (elapsed < stayTime)
        {
            DealDamage();

            elapsed += damageInterval;
            yield return new WaitForSeconds(damageInterval);
        }

        Destroy(gameObject);
    }

    // -----------------------------
    // 範囲内に継続ダメージ
    // -----------------------------
    void DealDamage()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position,
            damageRadius
        );

        foreach (var hit in hits)
        {
            if (!hit.CompareTag("Enemy")) continue;

            Enemy enemy = hit.GetComponent<Enemy>();
            if (enemy == null) continue;

            // ✅ダメージ処理（スキル扱い）
            enemy.TakeDamage(
                BattleManager.Instance.Status.Attack*3,
                enemy.transform.position,
                false,
                true,
                0
            );
        }

        // ✅燃焼音など入れたいならここで
        // AudioManager.Instance.PlaySE(AudioManager.Instance.meteoTick);
    }

    // -----------------------------
    // デバッグ用：範囲可視化
    // -----------------------------
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, damageRadius);
    }
}