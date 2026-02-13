using UnityEngine;
using System.Collections.Generic;

public class UzusioController : MonoBehaviour
{
    [Header("Rotation Settings")]
    [SerializeField] private float rotateSpeed = 180f;

    [Header("Damage Settings")]
    [SerializeField] private float damageInterval = 0.5f; // ✅攻撃頻度
    [SerializeField] private int maxHitCount = 5;          // ✅最大ヒット回数
    [SerializeField] private long damagePerHit = 5;        // ✅ダメージ量（任意）

    // ✅敵ごとの次回ダメージ受付時間
    private Dictionary<GameObject, float> nextDamageTime = new();

    // ✅ヒット総数
    private int totalHitCount = 0;
    private void Start()
    {
        AudioManager.Instance.PlaySE(AudioManager.Instance.uzusio);

    }
    void Update()
    {
        // ✅回り続ける
        transform.Rotate(0f, 0f, rotateSpeed * Time.deltaTime);
    }

    // -----------------------------
    // ✅敵に触れている間、一定間隔でダメージ
    // -----------------------------
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!collision.CompareTag("Enemy")) return;

        GameObject enemyObj = collision.gameObject;

        // ✅初回登録
        if (!nextDamageTime.ContainsKey(enemyObj))
        {
            nextDamageTime[enemyObj] = 0f;
        }

        // ✅攻撃頻度チェック
        if (Time.time >= nextDamageTime[enemyObj])
        {
            nextDamageTime[enemyObj] = Time.time + damageInterval;

            // ✅ヒット処理
            DealDamage(enemyObj);
            AudioManager.Instance.PlaySE(AudioManager.Instance.skillHit);

            // ✅ヒット回数加算
            totalHitCount++;

            // ✅最大ヒット回数で消える
            if (totalHitCount >= maxHitCount)
            {
                Destroy(gameObject);
            }
        }
    }

    // -----------------------------
    // ✅敵が外れたら管理から削除
    // -----------------------------
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Enemy")) return;

        GameObject enemyObj = collision.gameObject;

        if (nextDamageTime.ContainsKey(enemyObj))
        {
            nextDamageTime.Remove(enemyObj);
        }
    }

    // -----------------------------
    // ✅ダメージ処理
    // -----------------------------
    void DealDamage(GameObject enemyObj)
    {
        Enemy enemy = enemyObj.GetComponent<Enemy>();
        if (enemy == null) return;

        enemy.TakeDamage(
            Mathf.CeilToInt(BattleManager.Instance.Status.Attack*2.8f * BetManager.Instance.CurrentBet * BattleManager.Instance.Status.CriticalDamageRate),
            enemy.transform.position,
            false,
            true,
                0  // ✅スキル扱い（金文字）
        );

        // ✅必要ならSEもここで鳴らせる
        // AudioManager.Instance.PlaySE(AudioManager.Instance.skillHit);
    }
}