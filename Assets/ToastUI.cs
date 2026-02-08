using UnityEngine;
using TMPro;
using System.Collections;

public class ToastUI : MonoBehaviour
{
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private float floatUpDistance = 60f;
    [SerializeField] private float duration = 1.5f;

    private CanvasGroup canvasGroup;
    private RectTransform rect;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>() ?? gameObject.AddComponent<CanvasGroup>();

        rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = Vector2.zero;
        rect.localScale = Vector3.one;
    }


    public void Show(string msg)
    {
        messageText.text = msg;
        StartCoroutine(AnimateRoutine());
    }

    IEnumerator AnimateRoutine()
    {
        Vector2 startPos = rect.anchoredPosition;
        Vector2 endPos = startPos + Vector2.up * floatUpDistance;

        float t = 0f;
        canvasGroup.alpha = 1f;

        while (t < duration)
        {
            t += Time.deltaTime;

            float rate = t / duration;

            // 上へ移動
            rect.anchoredPosition =
                Vector2.Lerp(startPos, endPos, rate);

            // 後半でフェードアウト
            if (rate > 0.5f)
                canvasGroup.alpha =
                    Mathf.Lerp(1f, 0f, (rate - 0.5f) * 2f);

            yield return null;
        }

        Destroy(gameObject);
    }
}
