using UnityEngine;
using TMPro;
using System.Collections;

public class SkillToastUIController : MonoBehaviour
{
    [SerializeField] private TMP_Text messageText;
    private CanvasGroup canvasGroup;

    private bool canClose = false;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    // ✅表示開始
    public void Show(string msg)
    {
        messageText.text = msg;
        canvasGroup.alpha = 1;
        canClose = false;
    }

    // ✅途中で文字を更新できる
    public void UpdateText(string msg)
    {
        messageText.text = msg;
    }

    public void AllowClose()
    {
        canClose = true;
    }

    public IEnumerator CloseRoutine()
    {
        while (!canClose)
            yield return null;

        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1, 0, t);
            yield return null;
        }

        Destroy(gameObject);
    }
}