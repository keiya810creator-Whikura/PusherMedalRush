using UnityEngine;
using System.Collections;

public abstract class DropItemEffect : MonoBehaviour
{
    [Header("Move Settings")]
    [SerializeField] private float waitTime = 0.3f;
    [SerializeField] private float moveTime = 0.6f;
    [SerializeField] private AnimationCurve moveCurve;

    protected SpriteRenderer spriteRenderer;
    protected Vector3 targetPos;

    protected virtual void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Init(Color rarityColor, Vector3 target)
    {
        spriteRenderer.color = rarityColor;
        targetPos = target;
        StartCoroutine(MoveRoutine());
    }

    IEnumerator MoveRoutine()
    {
        // è≠Çµê√é~
        yield return new WaitForSeconds(waitTime / WaveManager.Instance.CurrentGameSpeed);

        Vector3 start = transform.position;
        float t = 0f;

        while (t < moveTime)
        {
            t += Time.deltaTime * WaveManager.Instance.CurrentGameSpeed;
            float rate = moveCurve.Evaluate(t / moveTime);
            transform.position = Vector3.Lerp(start, targetPos, rate);
            yield return null;
        }

        Destroy(gameObject);
    }
}
