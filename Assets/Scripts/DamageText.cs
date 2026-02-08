using UnityEngine;
using TMPro;
using System.Collections;

public class DamageText : MonoBehaviour
{
    [SerializeField] private TMP_Text text;

    [Header("Motion")]
    [SerializeField] private float moveUpDistance = 60f;
    [SerializeField] private float duration = 0.6f;

    public void Set(long damage, Vector3 screenPos, bool isCritical, bool isSkillHit)
    {
        transform.position = screenPos;

        // ✅スキルダメージは金色（最優先）
        if (isSkillHit)
        {
            text.text = $"{damage:N0}";
            text.color = new Color(1f, 0.85f, 0.2f); // 金色
            text.fontSize *= 1.3f;
        }
        // ✅クリティカルは赤
        else if (isCritical)
        {
            text.text = $"{damage:N0}!!";
            text.color = Color.red;
            text.fontSize *= 1.2f;
        }
        // ✅通常
        else
        {
            text.text = $"{damage:N0}";
            text.color = Color.white;
        }

        StartCoroutine(MoveUpAndFade());
    }

    IEnumerator MoveUpAndFade()
    {
        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + Vector3.up * moveUpDistance;

        float elapsed = 0f;
        Color startColor = text.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            // 上に移動（イージング）
            transform.position = Vector3.Lerp(startPos, endPos, t);

            // フェードアウト
            text.color = new Color(
                startColor.r,
                startColor.g,
                startColor.b,
                Mathf.Lerp(1f, 0f, t)
            );

            yield return null;
        }

        Destroy(gameObject);
    }
}
