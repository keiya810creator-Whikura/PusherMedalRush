using UnityEngine;

public class Yokoana : MonoBehaviour
{
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Medal"))
            return;

        MedalPoolManager.Instance.ReturnMedal(collision.gameObject);
    }
}
