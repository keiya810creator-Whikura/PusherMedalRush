using UnityEngine;

public class EnemyRandomMove : MonoBehaviour
{
    [Header("移動設定")]
    [SerializeField] float moveSpeed = 1.2f;

    [Header("挙動時間")]
    [SerializeField] Vector2 moveTimeRange = new Vector2(0.5f, 2.0f);
    [SerializeField] Vector2 stopTimeRange = new Vector2(0.3f, 1.5f);

    //[Header("移動可能範囲")]
    [SerializeField] Vector2 minPos = new Vector2(-4.37f, -10.67f);
    [SerializeField] Vector2 maxPos = new Vector2(4.36f, -6.27f);

    [Header("生きてる感")]
    [SerializeField] float breatheScale = 0.05f;
    [SerializeField] float breatheSpeed = 2.0f;
    [SerializeField] float bobHeight = 0.03f;
    [SerializeField] float bobSpeed = 1.5f;

    Vector2 moveDir;
    float timer;
    bool isMoving;

    Vector3 baseScale;
    Vector3 basePos;
    float lifeOffset;

    void Start()
    {
        baseScale = transform.localScale;   // ★ 2.5 を保持
        basePos = transform.position;
        lifeOffset = Random.value * 10f;    // 個体差
        DecideNextState();
    }

    void Update()
    {
        timer -= Time.deltaTime;

        if (isMoving)
        {
            Vector3 nextPos = transform.position +
                              (Vector3)(moveDir * moveSpeed * Time.deltaTime);

            // 範囲外に出そうなら方向変更
            if (nextPos.x < minPos.x || nextPos.x > maxPos.x ||
                nextPos.y < minPos.y || nextPos.y > maxPos.y)
            {
                DecideNextState();
                return;
            }

            transform.position = nextPos;
        }

        if (timer <= 0f)
        {
            DecideNextState();
        }

        // ★ 移動後の位置を基準にする（統合版の要）
        basePos = transform.position;
    }

    void LateUpdate()
    {
        float t = Time.time + lifeOffset;

        // 呼吸（スケール）
        float scale = 1f + Mathf.Sin(t * breatheSpeed) * breatheScale;
        transform.localScale = baseScale * scale;

        // 体重移動（上下）
        float y = Mathf.Sin(t * bobSpeed) * bobHeight;
        transform.position = basePos + Vector3.up * y;
    }

    void DecideNextState()
    {
        isMoving = Random.value > 0.4f; // 60%で移動

        if (isMoving)
        {
            moveDir = Random.insideUnitCircle.normalized;
            timer = Random.Range(moveTimeRange.x, moveTimeRange.y);

            // 向き反転（スケール維持）
            if (moveDir.x != 0)
            {
                transform.localScale = new Vector3(
                    Mathf.Sign(moveDir.x) * Mathf.Abs(baseScale.x),
                    baseScale.y,
                    baseScale.z
                );
            }
        }
        else
        {
            timer = Random.Range(stopTimeRange.x, stopTimeRange.y);
        }
    }

    void OnDisable()
    {
        transform.localScale = baseScale;
        transform.position = basePos;
    }
}
