using UnityEngine;

public class HoleHighlighter : MonoBehaviour
{
    [SerializeField] RectTransform top;
    [SerializeField] RectTransform bottom;
    [SerializeField] RectTransform left;
    [SerializeField] RectTransform right;

    public void Highlight(RectTransform target)
    {
        Vector3[] corners = new Vector3[4];
        target.GetWorldCorners(corners);

        float xMin = corners[0].x;
        float xMax = corners[2].x;
        float yMin = corners[0].y;
        float yMax = corners[2].y;

        // Top
        top.position = new Vector2(Screen.width / 2, (yMax + Screen.height) / 2);
        top.sizeDelta = new Vector2(Screen.width, Screen.height - yMax);

        // Bottom
        bottom.position = new Vector2(Screen.width / 2, yMin / 2);
        bottom.sizeDelta = new Vector2(Screen.width, yMin);

        // Left
        left.position = new Vector2(xMin / 2, Screen.height / 2);
        left.sizeDelta = new Vector2(xMin, Screen.height);

        // Right
        right.position = new Vector2((xMax + Screen.width) / 2, Screen.height / 2);
        right.sizeDelta = new Vector2(Screen.width - xMax, Screen.height);
    }
}