using UnityEngine;
using System.Collections;

public class MedalKakutoku : MonoBehaviour
{
    [Header("特殊獲得口")]
    [SerializeField] private bool isSlotHole;

    bool slotTriggerLock;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Medal"))
            return;

        Medal medal = collision.GetComponent<Medal>();

        // =========================
        // 🎰 特殊獲得口
        // =========================
        if (isSlotHole)
        {
            if (SlotManager.Instance != null)
            {
                // ★二重防止
                if (slotTriggerLock)
                {
                    MedalPoolManager.Instance.ReturnMedal(collision.gameObject);
                    return;
                }

                AudioManager.Instance.PlaySE(AudioManager.Instance.tokusyukakutoku);

                slotTriggerLock = true;

                // =========================
                // スロット処理
                // =========================
                if (SlotManager.Instance.IsSpinning)
                {
                    SlotManager.Instance.AddMultiplier();
                }
                else if (!SlotManager.Instance.IsCooldown)
                {
                    SlotManager.Instance.StartSlot();
                }

                // ★ロック解除
                StartCoroutine(ResetTriggerLock());
            }

            MedalPoolManager.Instance.ReturnMedal(collision.gameObject);
            //return;
        }

        // =========================
        // 通常獲得口
        // =========================

        if (SkillManager.Instance != null)
        {
            SkillManager.Instance.TriggerMedalCollected(medal);
        }

        int multiplier = 1;
        MedalMultiplier multi = collision.GetComponent<MedalMultiplier>();

        if (multi != null)
            multiplier = multi.multiplier;

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

        MedalPoolManager.Instance.ReturnMedal(collision.gameObject);
    }

    IEnumerator ResetTriggerLock()
    {
        // ★少し待つと安定する
        yield return new WaitForSeconds(0.1f);
        slotTriggerLock = false;
    }
}