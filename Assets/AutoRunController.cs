using UnityEngine;

public class AutoRunController : MonoBehaviour
{
    public static AutoRunController Instance;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        //DontDestroyOnLoad(gameObject);
    }

    public void StartAutoRun()
    {
        AdventureSession.IsAutoRun = true;
    }

    public void StopAutoRun()
    {
        AdventureSession.IsAutoRun = false;
    }
}
