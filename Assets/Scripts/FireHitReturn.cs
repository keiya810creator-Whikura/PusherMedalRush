using UnityEngine;

public class FireHitReturn : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        // ✅Enemy以外なら無視
        if (!other.CompareTag("Enemy"))
            return;

        // ✅Enemyに当たった時だけ消す
        if (FirePoolManager.Instance != null)
        {
            FirePoolManager.Instance.Return(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}