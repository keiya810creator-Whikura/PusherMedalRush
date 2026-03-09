using UnityEngine;
using System.Collections;

public class RotatingAreaDamage : MonoBehaviour
{
    [Header("‰ñ“]İ’è")]
    [SerializeField] private float rotateSpeed = 360f; // 1•b‚ ‚½‚è‚Ì‰ñ“]Šp“x

    [Header("ƒ_ƒ[ƒWİ’è")]
    [SerializeField] private int hitCount = 20;        // ƒ_ƒ[ƒW‰ñ”
    [SerializeField] private float hitInterval = 0.05f; // ‰½•b‚²‚Æ‚Éƒ_ƒ[ƒW‚ğ—^‚¦‚é‚©
    [SerializeField] private float damageRate = 1;   // UŒ‚—Í”{—¦

    [Header("I—¹İ’è")]
    [SerializeField] private bool destroyAfterFinish = true;

    private Coroutine damageRoutine;
    private bool isRunning;

    private void OnEnable()
    {
        StartSkill();
        AudioManager.Instance.PlaySE(AudioManager.Instance.endworld);

    }

    private void Update()
    {
        // ‚»‚Ìê‚Å‰ñ“]‚µ‘±‚¯‚é
        transform.Rotate(0f, 0f, rotateSpeed * Time.deltaTime);
    }

    public void StartSkill()
    {
        if (isRunning) return;

        isRunning = true;
        damageRoutine = StartCoroutine(DamageRoutine());
    }

    private IEnumerator DamageRoutine()
    {
        for (int i = 0; i < hitCount; i++)
        {
            DamageAllEnemies();
            yield return new WaitForSeconds(hitInterval);
        }

        isRunning = false;

        if (destroyAfterFinish)
        {
            Destroy(gameObject);
        }
    }

    private void DamageAllEnemies()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        if (enemies == null || enemies.Length == 0)
            return;

        int damage = Mathf.CeilToInt(
            BattleManager.Instance.Status.Attack
            * damageRate
            * BetManager.Instance.CurrentBet
            * BattleManager.Instance.Status.CriticalDamageRate
        );

        foreach (GameObject target in enemies)
        {
            if (target == null) continue;

            Enemy enemy = target.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(
                    damage,
                    enemy.transform.position,
                    false,
                    true,
                    1
                );
            }
        }
    }
}