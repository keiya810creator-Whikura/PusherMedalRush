using UnityEngine;
using System.Collections;

public class MedalDrop : MonoBehaviour
{
    private RectTransform targetUI;
    private long medalValue;
    public void Init(long value, RectTransform uiTarget)
    {
        medalValue = value;
        targetUI = uiTarget;

        StartCoroutine(MoveRoutine());
    }

    IEnumerator MoveRoutine()
    {
        Vector3 startPos = transform.position;

        // á@ è≠Çµê√é~
        yield return new WaitForSeconds(Random.Range(0.2f, 0.4f) / WaveManager.Instance.CurrentGameSpeed);

        float t = 0f;
        float speed = Random.Range(2.5f, 3.5f) * WaveManager.Instance.CurrentGameSpeed; ;

        while (t < 1f)
        {
            t += Time.deltaTime * speed;

            Vector3 uiWorldPos = Camera.main.ScreenToWorldPoint(
                targetUI.position
            );
            uiWorldPos.z = 0f;

            Vector3 curve = Vector3.up * Mathf.Sin(t * Mathf.PI) * 0.5f;

            transform.position =
                Vector3.Lerp(startPos, uiWorldPos, t) + curve;

            yield return null;
        }

        // áA ìûíB Å® ÉÅÉ_Éãâ¡éZ
        MoneyManager.instance.AddMedal(medalValue);

        Destroy(gameObject);
    }
}
