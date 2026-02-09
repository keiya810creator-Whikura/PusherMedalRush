using UnityEngine;

public class FireBallController : MonoBehaviour
{
    [Header("Move Settings")]
    [SerializeField] private float speed = 15f;

    [Header("Speed Random Range")]
    [SerializeField] private float speedRandomRange = 3f;

    [SerializeField] private float lifeTime = 2f;

    private GameObject targetEnemy;

    // ✅進行方向（最初に確定）
    private Vector3 moveDirection;

    // ✅最終速度（最初に確定）
    private float finalSpeed;

    void Start()
    {
        // ✅寿命で消える
        Destroy(gameObject, lifeTime);

        // ✅速度をランダム化
        finalSpeed = Random.Range(
            speed - speedRandomRange,
            speed + speedRandomRange
        );

        // ✅最初に敵を1回だけ探す
        targetEnemy = FindClosestEnemy();

        if (targetEnemy != null)
        {
            // ✅敵の方向を最初に確定
            moveDirection =
                (targetEnemy.transform.position - transform.position).normalized;

            // ✅発射SE
            AudioManager.Instance.PlaySE(AudioManager.Instance.fireBall);
        }
        else
        {
            // ✅敵がいないならとりあえず上方向に飛ばす
            moveDirection = Vector3.up;
        }
    }

    void Update()
    {
        // ✅止まらずずっと直進する
        transform.position += moveDirection * finalSpeed * Time.deltaTime;
    }

    // -----------------------------
    // 命中判定（Triggerでやる場合推奨）
    // -----------------------------
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Enemy")) return;

        Enemy enemy = collision.GetComponent<Enemy>();
        if (enemy == null) return;

        // ✅ダメージ処理（スキル扱い）
        enemy.TakeDamage(
            Mathf.CeilToInt(BattleManager.Instance.Status.Attack * 5.5f),
            transform.position,
            false,
            true,
                0
        );

        // ✅命中SE
        AudioManager.Instance.PlaySE(AudioManager.Instance.skillHit);

        Destroy(gameObject);
    }

    // -----------------------------
    // 最も近いEnemyタグを探す（初回のみ）
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