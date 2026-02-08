using UnityEngine;
using System.Collections;

public class GoldDrop : MonoBehaviour
{
    private RectTransform targetUI;
    private long goldValue;

    public void Init(long value, RectTransform uiTarget)
    {
        goldValue = value;
        targetUI = uiTarget;

        StartCoroutine(MoveRoutine());
    }

    IEnumerator MoveRoutine()
    {
        Vector3 startPos = transform.position;

        // =========================
        // ‡@ ƒhƒƒbƒvŒã‚ÌÃ~ŠÔ
        // =========================
        float waitTime = Random.Range(0.2f, 1.3f)/WaveManager.Instance.CurrentGameSpeed;
        yield return new WaitForSeconds(waitTime);

        // =========================
        // ‡A UI‚Ö‹z‚¢‚İ
        // =========================
        float t = 0f;
        float speed = Random.Range(2.5f, 3.5f)*WaveManager.Instance.CurrentGameSpeed; // ŒÂ‘Ì·

        while (t < 1f)
        {
            t += Time.deltaTime * speed;

            Vector3 uiWorldPos = Camera.main.ScreenToWorldPoint(
                targetUI.position
            );
            uiWorldPos.z = 0f;

            // ­‚µŒÊ‚ğ•`‚­
            Vector3 curve = Vector3.up * Mathf.Sin(t * Mathf.PI) * 0.5f;

            transform.position =
                Vector3.Lerp(startPos, uiWorldPos, t) + curve;

            yield return null;
        }

        // =========================
        // ‡B “’B ¨ ƒS[ƒ‹ƒh‰ÁZ
        // =========================
        MoneyManager.instance.AddGold(goldValue);

        Destroy(gameObject);
    }
}
