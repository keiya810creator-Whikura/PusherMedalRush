using UnityEngine;

public class IceBreadController : MonoBehaviour
{
    [Header("Rotation Settings")]
    [SerializeField] private float rotateSpeed = 360f;
    [SerializeField] private float lifeTime = 2f;
    void Start()
    {
        AudioManager.Instance.PlaySE(AudioManager.Instance.iceBread);

        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        // ✅その場で回転
        transform.Rotate(0f, 0f, rotateSpeed * Time.deltaTime);
    }

    // -----------------------------
    // ✅敵に当たったらフラグを立てるだけ
    // -----------------------------
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            AudioManager.Instance.PlaySE(AudioManager.Instance.skillHit);
            collision.GetComponent<Enemy>().TakeDamage(Mathf.CeilToInt(BattleManager.Instance.Status.Attack * 3.25f), collision.transform.position, false, true);
        }
    }
}
