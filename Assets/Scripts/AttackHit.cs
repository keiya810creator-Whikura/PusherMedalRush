using UnityEngine;

public class AttackHit : MonoBehaviour
{
    private bool hasHit = false;
    private int medalMultiplier = 1;

    [SerializeField] GameObject hitSparkPrefab;

    private void OnEnable()
    {
        hasHit = false;

        MedalMultiplier multi = GetComponent<MedalMultiplier>();
        medalMultiplier = (multi != null) ? Mathf.Max(1, multi.multiplier) : 1;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHit) return;

        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy == null) return; // Enemy以外は無視

        hasHit = true;

        var status = BattleManager.Instance.Status;

        float damageFloat = status.Attack * Random.Range(0.8f, 1.2f);

        bool isCritical = Random.value < status.CriticalRate;
        if (isCritical)
            damageFloat *= status.CriticalDamageRate;

        damageFloat *= medalMultiplier;
        damageFloat *= BetManager.Instance.CurrentBet;

        long damage = Mathf.Max(1, Mathf.RoundToInt(damageFloat));

        enemy.TakeDamage(
    damage,
    transform.position,
    isCritical,
    false,
    medalMultiplier   // ★追加
);


        Instantiate(hitSparkPrefab, transform.position, Quaternion.identity);

        if (isCritical)
            AudioManager.Instance.PlayCritikalSE();
        else
            AudioManager.Instance.PlayHitSE();

        // ✅Fireは FirePool に戻す！！
        if (FirePoolManager.Instance != null)
        {
            FirePoolManager.Instance.Return(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}