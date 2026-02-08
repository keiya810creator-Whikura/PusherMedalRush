using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class Pusher2D : MonoBehaviour
{
    public float forwardDistance = 0.5f;
    public float moveSpeed = 1.0f;

    private Rigidbody2D rb;
    private BoxCollider2D col;

    private Vector2 basePos;
    private Vector2 lastPos;
    private bool pushing;

    private readonly List<Rigidbody2D> ridingMedals = new();

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();

        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.linearVelocity = Vector2.zero;

        basePos = rb.position;
        lastPos = rb.position;

        //moveSpeed = WaveManager.Instance.CurrentGameSpeed*moveSpeed;
    }
    private void Start()
    {
        moveSpeed = WaveManager.Instance.CurrentGameSpeed * moveSpeed;

    }
    public float baseMoveSpeed = 1f; // 元の速度（固定）
    void FixedUpdate()
    {
        float speedRate = WaveManager.Instance.CurrentGameSpeed;
        float move =
        baseMoveSpeed * speedRate * Time.fixedDeltaTime;
        Vector2 targetPos;

        if (!pushing)
        {
            // 上昇（すり抜け）
            col.enabled = false;

            targetPos = Vector2.MoveTowards(
                rb.position,
                basePos + Vector2.up * forwardDistance,
                move
            );

            if (Vector2.Distance(rb.position, basePos + Vector2.up * forwardDistance) < 0.001f)
            {
                pushing = true;
            }
        }
        else
        {
            // 下降（押す）
            col.enabled = true;

            targetPos = Vector2.MoveTowards(
                rb.position,
                basePos,
                move
            );

            if (Vector2.Distance(rb.position, basePos) < 0.001f)
            {
                pushing = false;
            }
        }

        // ★ ここが重要：deltaは targetPos から計算
        Vector2 delta = targetPos - lastPos;

        rb.MovePosition(targetPos);

        foreach (var medalRb in ridingMedals)
        {
            if (medalRb == null) continue;
            medalRb.MovePosition(medalRb.position + delta);
        }

        lastPos = targetPos;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Medal")) return;

        if (collision.rigidbody != null && !ridingMedals.Contains(collision.rigidbody))
        {
            ridingMedals.Add(collision.rigidbody);
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Medal")) return;

        if (collision.rigidbody != null)
        {
            ridingMedals.Remove(collision.rigidbody);
        }
    }
    public void AddRidingMedal(Rigidbody2D rb)
    {
        if (!ridingMedals.Contains(rb))
            ridingMedals.Add(rb);
    }

    public void RemoveRidingMedal(Rigidbody2D rb)
    {
        ridingMedals.Remove(rb);
    }

}
