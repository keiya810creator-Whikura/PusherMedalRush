using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class LongPressButton : MonoBehaviour,
    IPointerDownHandler,
    IPointerUpHandler
{
    public System.Action OnClickOnce;
    public System.Action OnHoldRepeat;

    [SerializeField] private float startDelay = 0.4f;
    [SerializeField] private float interval = 0.05f;

    private bool holding;
    private Coroutine routine;

    public void OnPointerDown(PointerEventData eventData)
    {
        holding = true;

        // 最初に1回
        OnClickOnce?.Invoke();

        routine = StartCoroutine(HoldRoutine());
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        StopHold();
    }

    private void StopHold()
    {
        holding = false;

        if (routine != null)
        {
            StopCoroutine(routine);
            routine = null;
        }
    }

    private IEnumerator HoldRoutine()
    {
        yield return new WaitForSeconds(startDelay);

        while (holding)
        {
            OnHoldRepeat?.Invoke();
            yield return new WaitForSeconds(interval);
        }
    }
}
