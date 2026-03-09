using UnityEngine;

public class TestManager : MonoBehaviour
{
    public static TestManager Instance;
    public bool textMode;
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
}
