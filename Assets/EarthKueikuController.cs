using UnityEngine;
using System.Collections.Generic;

public class EarthKueikuController : MonoBehaviour
{
    [Header("Shake Settings")]
    [SerializeField] private float shakeStrength = 0.15f;
    [SerializeField] private float shakeSpeed = 25f;

    [Header("Damage Settings")]
    [SerializeField] private float damageInterval = 0.6f;

    [Header("Lifetime Settings")]
    [SerializeField] private float lifeTime = 6f; // ✅6秒で消える

    private Vector3 basePos;

    // ✅敵ごとの次回ダメージ時間
    private Dictionary<GameObject, float> nextDamageTime = new();

    void Start()
    {
        // ✅初期位置保存
        basePos = transform.position;

        // ✅発動SE
        AudioManager.Instance.PlaySE(AudioManager.Instance.arsukueiku);

        // ✅6秒で消える
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        // ✅地震のように揺れる
        float x = Mathf.Sin(Time.time * shakeSpeed) * shakeStrength;
        float y = Mathf.Cos(Time.time * shakeSpeed * 0.8f) * (shakeStrength * 0.5f);

        transform.position = basePos + new Vector3(x, y, 0f);
    }

    // -----------------------------
    // ✅敵に当たり続けたら一定間隔でダメージ
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

        // ✅一定時間ごとにダメージ
        if (Time.time >= nextDamageTime[enemyObj])
        {
            nextDamageTime[enemyObj] = Time.time + damageInterval;

            DealDamage(enemyObj);
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
    // ✅継続ダメージ処理（Attack100%）
    // -----------------------------
    void DealDamage(GameObject enemyObj)
    {
        Enemy enemy = enemyObj.GetComponent<Enemy>();
        if (enemy == null) return;

        // ✅ダメージ量＝攻撃力100%
        long damage = Mathf.CeilToInt(BattleManager.Instance.Status.Attack*1.7f);

        enemy.TakeDamage(
            damage,
            enemy.transform.position,
            false,
            true,
                0  // ✅スキル扱い（金文字）
        );

        // ✅ヒットSE（毎回鳴ると騒がしいなら調整可）
        AudioManager.Instance.PlaySE(AudioManager.Instance.skillHit);
    }
}