using UnityEngine;

public class MedalKakutoku : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Medal"))
            return;

        // ✅Medalコンポーネント取得
        Medal medal = collision.GetComponent<Medal>();

        // =========================
        // ✅特殊メダル回収スキル発動
        // =========================
        if (SkillManager.Instance != null)
        {
            SkillManager.Instance.TriggerMedalCollected(medal);
        }

        // =========================
        // 倍率取得（Fire演出用）
        // =========================
        int multiplier = 1;
        MedalMultiplier multi =
            collision.GetComponent<MedalMultiplier>();

        if (multi != null)
            multiplier = multi.multiplier;

        // =========================
        // Fireエフェクト再生
        // =========================
        FireType type = multiplier switch
        {
            2 => FireType.X2,
            5 => FireType.X5,
            10 => FireType.X10,
            _ => FireType.Normal
        };

        if (FirePoolManager.Instance != null)
        {
            FirePoolManager.Instance.Play(
                type,
                collision.transform.position,
                Quaternion.identity
            );
        }

        // =========================
        // ✅Poolに返却（Destroy禁止）
        // =========================
        MedalPoolManager.Instance.ReturnMedal(collision.gameObject);
    }
}