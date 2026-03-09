using UnityEngine;
using System.Collections;

public class AcidRainDamage : MonoBehaviour
{
    private float duration = 10f;
    private int hitCount = 20;

    private float interval;

    private void Start()
    {
        interval = duration / hitCount;
        AudioManager.Instance.PlaySE(AudioManager.Instance.rain);

        StartCoroutine(DamageRoutine());
    }

    IEnumerator DamageRoutine()
    {
        for (int i = 0; i < hitCount; i++)
        {
            DealDamage();
            yield return new WaitForSeconds(interval);
        }

        Destroy(gameObject);
    }

    void DealDamage()
    {
        GameObject[] targets = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject target in targets)
        {
            Enemy enemy = target.GetComponent<Enemy>();

            if (enemy != null)
            {
                enemy.TakeDamage(
                    Mathf.CeilToInt(
                        BattleManager.Instance.Status.Attack *
                        0.9f *
                        BetManager.Instance.CurrentBet *
                        BattleManager.Instance.Status.CriticalDamageRate
                    ),
                    enemy.transform.position,
                    false,
                    true,
                    1
                );
            }
        }
    }
}