using UnityEngine;
using System.Collections;

public class SyuuyoukoudannController : MonoBehaviour
{
    [Header("Move Settings")]
    public float speed = 10f;
    public float lifeTime = 3f;

    private bool isMoving = false;

    void Start()
    {
        Destroy(gameObject, 5f);
        AudioManager.Instance.PlaySE(AudioManager.Instance.renngeki);

    }

    void Update()
    {

        transform.position += Vector3.down * speed * Time.deltaTime;
    }
    // -----------------------------
    // ✅命中処理
    // -----------------------------
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Enemy")) return;

        AudioManager.Instance.PlaySE(AudioManager.Instance.skillHit);
        other.GetComponent<Enemy>().TakeDamage(
            Mathf.CeilToInt(BattleManager.Instance.Status.Attack * 1.85f * BetManager.Instance.CurrentBet * BattleManager.Instance.Status.CriticalDamageRate),
            other.transform.position,
            false,
            true,
                0
        );
    }
}
