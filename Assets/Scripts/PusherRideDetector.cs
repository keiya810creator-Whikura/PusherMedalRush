using UnityEngine;

public class PusherRideDetector : MonoBehaviour
{
    private Pusher2D pusher;

    void Awake()
    {
        pusher = GetComponentInParent<Pusher2D>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        TryAddMedal(other);
    }

    void OnTriggerStay2D(Collider2D other)
    {
        // š ‚±‚±‚ªd—vFŒã‚©‚çğŒ‚ğ–‚½‚µ‚½ê‡‚àE‚¤
        TryAddMedal(other);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Medal")) return;

        Rigidbody2D rb = other.attachedRigidbody;
        if (rb != null)
        {
            pusher.RemoveRidingMedal(rb);
        }
    }

    void TryAddMedal(Collider2D other)
    {
        if (!other.CompareTag("Medal")) return;

        Medal medal = other.GetComponent<Medal>();
        if (medal == null || !medal.CanRidePusher) return;

        Rigidbody2D rb = other.attachedRigidbody;
        if (rb != null)
        {
            pusher.AddRidingMedal(rb);
        }
    }
}
