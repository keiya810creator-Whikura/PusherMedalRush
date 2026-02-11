using UnityEngine;
using System.Collections;

public class WindCutterProjectile : MonoBehaviour
{
    [Header("Move Settings")]
    public float speed = 10f;
    public float lifeTime = 3f;

    private bool isMoving = false;

    void Start()
    {

        // ✅敵が出るまで待機
        StartCoroutine(WaitForEnemy());
    }

    void Update()
    {
        // ✅敵が出現するまで停止
        if (!isMoving) return;

        // ✅発射後は常に下へ進む
        transform.position += Vector3.down * speed * Time.deltaTime;
    }

    // -----------------------------
    // ✅敵がいるまで待つ
    // -----------------------------
    IEnumerator WaitForEnemy()
    {
        while (true)
        {
            if (GameObject.FindGameObjectsWithTag("Enemy").Length > 0)
            {
                // ✅発射開始
                isMoving = true;
                AudioManager.Instance.PlaySE(AudioManager.Instance.windCutter);
                // ✅寿命カウント開始
                Destroy(gameObject, lifeTime);

                yield break;
            }

            // ✅0.1秒ごとにチェック
            yield return new WaitForSeconds(0.1f);
        }
    }

    // -----------------------------
    // ✅命中処理
    // -----------------------------
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Enemy")) return;

        Debug.Log("🌪 WindCutter Hit!");
        AudioManager.Instance.PlaySE(AudioManager.Instance.skillHit);
        other.GetComponent<Enemy>().TakeDamage(
            Mathf.CeilToInt(BattleManager.Instance.Status.Attack * 15.5f * BetManager.Instance.CurrentBet),
            other.transform.position,
            false,
            true,
                0
        );
    }
}