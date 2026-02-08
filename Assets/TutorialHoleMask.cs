using UnityEngine;

public class TutorialHoleMask : MonoBehaviour
{
    [Header("Panels")]
    public RectTransform top, bottom, left, right;

    [Header("Canvas Root")]
    public RectTransform canvasRoot;

    [Header("Padding")]
    public Vector2 padding = new Vector2(20, 20);

    private RectTransform currentTarget;

    void Update()
    {
        if (currentTarget != null)
            UpdateHole();
    }

    public void Highlight(RectTransform target)
    {
        currentTarget = target;
        UpdateHole();
    }

    private void UpdateHole()
    {
        // ✅ターゲット座標をCanvas内に変換
        Vector3[] corners = new Vector3[4];
        currentTarget.GetWorldCorners(corners);

        Vector2 min, max;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRoot,
            RectTransformUtility.WorldToScreenPoint(null, corners[0]),
            null,
            out min
        );

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRoot,
            RectTransformUtility.WorldToScreenPoint(null, corners[2]),
            null,
            out max
        );

        min -= padding;
        max += padding;

        float canvasW = canvasRoot.rect.width;
        float canvasH = canvasRoot.rect.height;

        // ✅Top
        SetRect(top,
            0, max.y,
            canvasW, canvasH - max.y);

        // ✅Bottom
        SetRect(bottom,
            0, -canvasH / 2,
            canvasW, min.y + canvasH / 2);

        // ✅Left
        SetRect(left,
            -canvasW / 2, min.y,
            min.x + canvasW / 2, max.y - min.y);

        // ✅Right
        SetRect(right,
            max.x, min.y,
            canvasW / 2 - max.x, max.y - min.y);
    }

    private void SetRect(RectTransform panel, float x, float y, float w, float h)
    {
        panel.anchoredPosition = new Vector2(x + w / 2, y + h / 2);
        panel.sizeDelta = new Vector2(w, h);
    }
    public void Clear()
    {
        currentTarget = null;

        top.gameObject.SetActive(false);
        bottom.gameObject.SetActive(false);
        left.gameObject.SetActive(false);
        right.gameObject.SetActive(false);
    }
}