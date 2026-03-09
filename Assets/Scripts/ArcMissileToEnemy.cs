using UnityEngine;

public class ArcHomingMissile : MonoBehaviour
{
    enum State
    {
        Rise,
        Scatter,
        Homing
    }

    private State state;

    [Header("Search")]
    [SerializeField] private string targetTag = "Enemy";

    [Header("Lifetime")]
    [SerializeField] private float destroyTime = 6f;

    [Header("Rise")]
    [SerializeField] private float riseSpeed = 8f;
    [SerializeField] private float riseDuration = 0.35f;

    [Header("Scatter")]
    [SerializeField] private float scatterSpeed = 5f;

    [Header("Arc")]
    [SerializeField] private float arcDuration = 0.9f;
    [SerializeField] private float arcHeightMin = 1.5f;
    [SerializeField] private float arcHeightMax = 3f;
    [SerializeField] private float arcSideMin = -1.5f;
    [SerializeField] private float arcSideMax = 1.5f;

    [Header("Rotation")]
    [SerializeField] private float rotateSpeed = 720f;

    private Transform target;

    private float timer;

    private Vector3 arcStart;
    private Vector3 arcControl;
    private Vector3 arcEnd;

    private Vector3 scatterDir;

    private void Start()
    {
        Destroy(gameObject, destroyTime);

        state = State.Rise;

        SetRotation(Vector2.up);
    }

    void Update()
    {
        switch (state)
        {
            case State.Rise:
                UpdateRise();
                break;

            case State.Scatter:
                UpdateScatter();
                break;

            case State.Homing:
                UpdateHoming();
                break;
        }
    }

    void UpdateRise()
    {
        timer += Time.deltaTime;

        transform.position += Vector3.up * riseSpeed * Time.deltaTime;

        RotateTowards(Vector2.up);

        if (timer >= riseDuration)
        {
            timer = 0f;

            target = FindTarget();

            if (target != null)
            {
                CreateNewArc();
                state = State.Homing;
            }
            else
            {
                // Œü‚¢‚Ä‚¢‚é•ûŒü‚ÖŽU‚é
                scatterDir = transform.up;
                state = State.Scatter;
            }
        }
    }

    void UpdateScatter()
    {
        transform.position += scatterDir * scatterSpeed * Time.deltaTime;

        target = FindTarget();

        if (target != null)
        {
            CreateNewArc();
            state = State.Homing;
        }
    }

    void UpdateHoming()
    {
        if (target == null)
        {
            target = FindTarget();

            if (target == null)
            {
                scatterDir = transform.up;
                state = State.Scatter;
                return;
            }

            CreateNewArc();
        }

        timer += Time.deltaTime;

        arcEnd = target.position;

        float t = Mathf.Clamp01(timer / arcDuration);

        Vector3 pos = Bezier(t, arcStart, arcControl, arcEnd);
        Vector3 next = Bezier(Mathf.Clamp01(t + 0.02f), arcStart, arcControl, arcEnd);

        Vector3 dir = (next - pos).normalized;

        transform.position = pos;

        RotateTowards(dir);

        if (t >= 1f)
        {
            CreateNewArc();
        }
    }

    void CreateNewArc()
    {
        if (target == null) return;

        timer = 0f;

        arcStart = transform.position;
        arcEnd = target.position;

        Vector3 mid = (arcStart + arcEnd) * 0.5f;

        float height = Random.Range(arcHeightMin, arcHeightMax);
        float sideOffset = Random.Range(arcSideMin, arcSideMax);

        Vector3 dir = (arcEnd - arcStart).normalized;
        Vector3 side = Vector3.Cross(dir, Vector3.forward).normalized;

        arcControl = mid + Vector3.up * height + side * sideOffset;
    }

    Transform FindTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(targetTag);

        if (enemies.Length == 0)
            return null;

        Transform nearest = null;
        float minDist = float.MaxValue;

        foreach (GameObject e in enemies)
        {
            float dist = (e.transform.position - transform.position).sqrMagnitude;

            if (dist < minDist)
            {
                minDist = dist;
                nearest = e.transform;
            }
        }

        return nearest;
    }

    Vector3 Bezier(float t, Vector3 a, Vector3 b, Vector3 c)
    {
        float u = 1f - t;

        return
            u * u * a +
            2f * u * t * b +
            t * t * c;
    }

    void RotateTowards(Vector2 dir)
    {
        if (dir.sqrMagnitude < 0.001f) return;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        Quaternion targetRot = Quaternion.Euler(0, 0, angle - 90);

        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRot,
            rotateSpeed * Time.deltaTime
        );
    }

    void SetRotation(Vector2 dir)
    {
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0, 0, angle - 90);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.CompareTag("Enemy")) return;

        AudioManager.Instance.PlaySE(AudioManager.Instance.godrain);

        Enemy enemy = col.GetComponent<Enemy>();

        if (enemy != null)
        {
            enemy.TakeDamage(
                Mathf.CeilToInt(
                    BattleManager.Instance.Status.Attack *
                    BetManager.Instance.CurrentBet *
                    BattleManager.Instance.Status.CriticalDamageRate
                ),
                enemy.transform.position,
                false,
                true,
                1
            );
        }

        Destroy(gameObject);
    }
}