using UnityEngine;

public class Ougonnsyouzyuunohousyuu : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        MedalGenerator.Instance.SpawnMedals(25);
        Destroy(gameObject);
    }
}
