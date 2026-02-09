using UnityEngine;
using TMPro;
using System.Collections;

public class DamageText : MonoBehaviour
{
    [SerializeField] private TMP_Text text;

    [Header("Motion")]
    [SerializeField] private float moveUpDistance = 60f;
    [SerializeField] private float duration = 0.6f;

    public void Set(
    long damage,
    Vector3 screenPos,
    bool isCritical,
    bool isSkillHit,
    int medalMultiplier
)
    {
        transform.position = screenPos;

        text.text = $"{damage:N0}";
        text.fontSize = 36; // ★基準サイズを毎回リセット（重要）

        // ✅スキルダメージ（金・最優先）
        if (isSkillHit)
        {
            text.color = new Color(1f, 0.85f, 0.2f);
            text.fontSize *= 1.3f;
        }
        // ✅クリティカル
        else if (isCritical)
        {
            text.text += "!!";
            text.color = Color.red;
            text.fontSize *= 1.2f;
        }
        // ✅メダル倍率による色分け
        else
        {
            text.color = GetMultiplierColor(medalMultiplier);
        }

        StartCoroutine(MoveUpAndFade());
    }

    private Color GetMultiplierColor(int multiplier)
    {
        return multiplier switch
        {
            >= 10 => Color.red,                     // ×10
            >= 5 => new Color(0.3f, 0.6f, 1f),     // ×5（青）
            >= 2 => new Color(0.3f, 1f, 0.3f),     // ×2（緑）
            _ => Color.white
        };
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
