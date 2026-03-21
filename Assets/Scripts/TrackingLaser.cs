using UnityEngine;
using System.Collections;

[RequireComponent(typeof(LineRenderer))]
public class TrackingLaser : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;
    [SerializeField] private Transform firePoint;

    [Header("Damage Settings")]
    [SerializeField] private int damagePerHit = 10;
    [SerializeField] private int hitCount = 10;
    [SerializeField] private float hitInterval = 0.1f;

    [Header("Laser Settings")]
    [SerializeField] private float maxDistance = 20f;

    private LineRenderer lineRenderer;
    private bool isFiring;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
    }

    private void OnEnable()
    {
        Destroy(gameObject, 5);

        damagePerHit = Mathf.CeilToInt(BattleManager.Instance.Status.Attack * .875f * BetManager.Instance.CurrentBet * BattleManager.Instance.Status.CriticalDamageRate);
        StartLaser();
    }

    public void StartLaser()
    {
        if (isFiring) return;

        AudioManager.Instance.PlaySE(AudioManager.Instance.bureth);
        // ターゲットが無ければ自動取得
        if (target == null)
        {
            target = FindNearestEnemy();
        }

        isFiring = true;
        StartCoroutine(DamageRoutine());
    }

    private void Update()
    {
        UpdateLaserVisual();
    }

    private void UpdateLaserVisual()
    {
        if (firePoint == null)
        {
            lineRenderer.enabled = false;
            return;
        }

        // ターゲットが死んだら再取得
        if (target == null)
        {
            target = FindNearestEnemy();
        }

        Vector3 startPos = firePoint.position;
        Vector3 endPos;

        if (target != null)
        {
            endPos = target.position;
        }
        else
        {
            endPos = startPos + firePoint.right * maxDistance;
        }

        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, startPos);
        lineRenderer.SetPosition(1, endPos);

        Vector3 dir = endPos - startPos;
        if (dir.sqrMagnitude > 0.0001f)
        {
            transform.right = dir.normalized;
        }
    }

    private IEnumerator DamageRoutine()
    {
        int currentHits = 0;

        while (currentHits < hitCount)
        {
            if (target == null)
            {
                target = FindNearestEnemy();
                if (target == null) break;
            }

            Enemy enemy = target.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damagePerHit, enemy.transform.position, false, true, 1);
            }

            currentHits++;
            yield return new WaitForSeconds(hitInterval);
        }

        Destroy(gameObject);
    }

    Transform FindNearestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        float minDistance = float.MaxValue;
        Transform nearest = null;

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(firePoint.position, enemy.transform.position);

            if (distance < minDistance)
            {
                minDistance = distance;
                nearest = enemy.transform;
            }
        }

        return nearest;
    }
}