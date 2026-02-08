using UnityEngine;

public class ForceSunderContlroller : MonoBehaviour
{
    [Header("Move Settings")]
    [SerializeField] private float speed = 15f;
    [SerializeField] private float lifeTime = 2f;

    private Vector3 targetPos;
    private GameObject targetEnemy;
    private bool hasTarget = false;

    void Start()
    {
        // ✅寿命で消える
        Destroy(gameObject, lifeTime);

        // ✅敵が出現するまで待機開始
        StartCoroutine(WaitForEnemy());
    }

    void Update()
    {
        if (!hasTarget) return;

        // ✅敵が途中で消えたら再探索する
        if (targetEnemy == null)
        {
            hasTarget = false;
            StartCoroutine(WaitForEnemy());
            return;
        }

        // ✅常に敵の位置を追従（落雷のように一直線）
        targetPos = targetEnemy.transform.position;

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPos,
            speed * Time.deltaTime
        );

        // ✅到達したら命中処理
        if (Vector3.Distance(transform.position, targetPos) < 0.05f)
        {
            OnHit();
        }
    }

    // -----------------------------
    // 敵に命中したとき
    // -----------------------------
    void OnHit()
    {
        // ✅ここに攻撃処理追加可能
        targetEnemy.GetComponent<Enemy>().TakeDamage(BattleManager.Instance.Status.Attack*10, transform.position, false, true);
        AudioManager.Instance.PlaySE(AudioManager.Instance.skillHit);

        Destroy(gameObject);
    }

    // -----------------------------
    // 敵が出るまで待つ
    // -----------------------------
    System.Collections.IEnumerator WaitForEnemy()
    {
        while (true)
        {
            GameObject enemy = FindClosestEnemy();

            if (enemy != null)
            {
                AudioManager.Instance.PlaySE(AudioManager.Instance.forsesunder);
                targetEnemy = enemy;
                hasTarget = true;
                yield break; // ✅敵が見つかったので発進開始
            }

            // ✅0.1秒ごとに探す（負荷軽減）
            yield return new WaitForSeconds(0.1f);
        }
    }

    // -----------------------------
    // 最も近いEnemyタグを探す
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