using UnityEngine;

public class Soul : MonoBehaviour
{
    [SerializeField] private float floatSpeed = 1.5f;
    [SerializeField] private float lifeTime = 1.2f;

    private float timer;

    public void Init()
    {
        timer = 0f;
    }

    private bool hasNotified = false; // ★追加

    void Update()
    {
        timer += Time.deltaTime*WaveManager.Instance.CurrentGameSpeed;
        transform.position += Vector3.up * floatSpeed * Time.deltaTime;

        if (!hasNotified && timer >= lifeTime)
        {
            if(GameObject.FindWithTag("Drop")==null)
            hasNotified = true; // ★先に立てる（超重要）
            WaveManager.Instance.OnSoulFinished();
            Destroy(gameObject);
        }
    }
}
