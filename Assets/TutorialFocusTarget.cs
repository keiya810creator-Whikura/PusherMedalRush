using UnityEngine;
using UnityEngine.UI;

public class TutorialFocusTarget : MonoBehaviour
{
    private Canvas addedCanvas;
    private GraphicRaycaster raycaster;

    /// ✅対象をOverlayより上に出して押せるようにする
    public void Focus(RectTransform target)
    {
        addedCanvas = target.GetComponent<Canvas>();
        if (addedCanvas == null)
            addedCanvas = target.gameObject.AddComponent<Canvas>();

        addedCanvas.overrideSorting = true;
        addedCanvas.sortingOrder = 200;

        raycaster = target.GetComponent<GraphicRaycaster>();
        if (raycaster == null)
            target.gameObject.AddComponent<GraphicRaycaster>();
    }

    /// ✅元に戻す
    public void Unfocus(RectTransform target)
    {
        if (addedCanvas != null)
            Destroy(addedCanvas);

        if (raycaster != null)
            Destroy(raycaster);
    }
}