using UnityEngine;

public class MedalCollector : MonoBehaviour
{
    [SerializeField] private GameObject medalDropPrefab;
    [SerializeField] private long medalPerHit = 1;

    private RectTransform medalUITarget;

    void Start()
    {
        GameObject ui = GameObject.Find("メダルImage");
        if (ui != null)
        {
            medalUITarget = ui.GetComponent<RectTransform>();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (WaveManager.Instance.CurrentState != WaveState.Recovery)
            return;

        MedalMultiplier medal = other.GetComponent<MedalMultiplier>();
        if (medal == null) return;

        // ✅ 二重回収防止
        if (medal.isCollected)
            return;

        medal.isCollected = true;

        SpawnMedalDrop(new Vector3(other.transform.position.x, -5.5f));

        MedalPoolManager.Instance.ReturnMedal(other.gameObject);
    }


    void SpawnMedalDrop(Vector3 spawnPos)
    {
        Vector3 offset = new Vector3(
            Random.Range(-0.4f, 0.4f),
            Random.Range(0.2f, 0.6f),
            0f
        );

        GameObject drop = Instantiate(
            medalDropPrefab,
            spawnPos + offset,
            Quaternion.identity
        );
        

        drop.GetComponent<MedalDrop>()
            .Init(BattleManager.Instance.Status.RecoveryWaveMedalValue, medalUITarget);
    }
}
