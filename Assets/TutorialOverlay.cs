using UnityEngine;
using TMPro;

public class TutorialOverlay : MonoBehaviour
{
    [SerializeField] private TutorialHoleMask holeMask;
    [SerializeField] private TutorialFocusTarget focuser;
    [SerializeField] private TMP_Text messageText;

    private RectTransform currentTarget;

    public void Show(string message, RectTransform target = null)
    {
        gameObject.SetActive(true);

        messageText.text = message;

        if (target != null)
        {
            currentTarget = target;

            holeMask.Highlight(target);
            focuser.Focus(target);
        }
    }

    public void Hide()
    {
        if (currentTarget != null)
        {
            focuser.Unfocus(currentTarget);
            currentTarget = null;
        }

        holeMask.Clear();
        gameObject.SetActive(false);
    }
}