using UnityEngine;
using System.Collections;

public class SouenController : MonoBehaviour
{
    [Header("Fall Settings")]
    [SerializeField] private float fallSpeed = 8f;
    [SerializeField] private float lifeTime = 3f;

    private bool isFalling = false;

    void Start()
    {
        // ✅敵が出現するまで待機
        StartCoroutine(WaitForEnemy());
    }

    void Update()
    {
        // ✅敵がいるときだけ落下する
        if (!isFalling) return;

        transform.position += Vector3.down * fallSpeed * Time.deltaTime;
    }

    // -----------------------------
    // ✅敵がいるのを確認してから発射
    // -----------------------------
    IEnumerator WaitForEnemy()
    {
        while (true)
        {
            // ✅敵が存在するかチェック
            if (GameObject.FindGameObjectsWithTag("Enemy").Length > 0)
            {
                // ✅発射開始
                isFalling = true;

                // ✅発射SE
                AudioManager.Instance.PlaySE(AudioManager.Instance.souen);

                // ✅寿命で消える
                Destroy(gameObject, lifeTime);

                yield break;
            }

            // ✅0.1秒ごとにチェック（負荷軽減）
            yield return new WaitForSeconds(0.1f);
        }
    }

    // -----------------------------
    // ✅敵に当たったら消える
    // -----------------------------
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Enemy")) return;

        // ✅命中SE
        AudioManager.Instance.PlaySE(AudioManager.Instance.skillHit);

        // ✅ダメージ（スキル扱い）
        collision.GetComponent<Enemy>().TakeDamage(
            BattleManager.Instance.Status.Attack * 15,
            collision.transform.position,
            false,
            true,
                0
        );

        Destroy(gameObject);
    }
}