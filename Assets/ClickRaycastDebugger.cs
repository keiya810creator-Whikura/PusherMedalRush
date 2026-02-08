using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class ClickRaycastDebugger : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            PointerEventData pointer = new PointerEventData(EventSystem.current);
            pointer.position = Input.mousePosition;

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointer, results);

            //Debug.Log("==== Raycast Hit ====");
            foreach (var r in results)
            {
                //Debug.Log(r.gameObject.name);
            }
        }
    }
}
