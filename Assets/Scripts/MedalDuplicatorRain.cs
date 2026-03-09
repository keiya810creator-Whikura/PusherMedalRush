using UnityEngine;

public class MedalDuplicatorRain : MonoBehaviour
{
    [SerializeField] private float fallSpeed = 8f;
    [SerializeField] private float destroyY = -15f;

    void Update()
    {
        transform.Translate(Vector3.down * fallSpeed * Time.deltaTime);

        if (transform.position.y <= destroyY)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        TryDuplicate(other.gameObject);
    }

    void TryDuplicate(GameObject obj)
    {
        if (!obj.CompareTag("Medal")) return;

        Medal flag = obj.GetComponent<Medal>();
        if (flag == null) return;

        if (flag.isDuplicated) return;

        DuplicateMedal(obj);

        flag.MarkDuplicated();
    }
    void DuplicateMedal(GameObject original)
    {
        MedalPoolObject info = original.GetComponent<MedalPoolObject>();
        if (info == null) return;

        GameObject prefab = info.prefabKey;

        GameObject clone = MedalPoolManager.Instance.GetMedal(prefab);

        Medal flag = clone.GetComponent<Medal>();
        flag.SetNormalState();
        flag.MarkDuplicated();

        clone.transform.position = original.transform.position;
        clone.transform.rotation = original.transform.rotation;
    }
}