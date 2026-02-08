using UnityEngine;

public class GameProgressManager : MonoBehaviour
{
    public static GameProgressManager Instance { get; private set; }

    public int HighestClearedWave = 0;

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

    public void RecordClearedWave(int wave)
    {
        if (wave > HighestClearedWave)
        {
            HighestClearedWave = wave;
            Debug.Log($"Å‚“’BWaveXV: {HighestClearedWave}");

            FindAnyObjectByType<ProgressSaveBridge>()?.SaveProgress();
        }
    }
    public void SetHighestClearedWave(int wave)
    {
        HighestClearedWave = wave;
    }

}
