using UnityEngine;

public class GameProgressManager : MonoBehaviour
{
    public static GameProgressManager Instance { get; private set; }

    public int Stage1HighestWave = 0;
    public int Stage2HighestWave = 0;

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
        int stage = SaveManager.Instance.Data.currentStage;

        if (stage == 1)
        {
            if (wave > Stage1HighestWave)
            {
                Stage1HighestWave = wave;
            }
        }
        else
        {
            if (wave > Stage2HighestWave)
            {
                Stage2HighestWave = wave;
            }
        }

        Debug.Log($"Å‚“’BWaveXV: {wave}");

        FindAnyObjectByType<ProgressSaveBridge>()?.SaveProgress();
    }
    public void SetHighestClearedWave(int wave)
    {
        int stage = SaveManager.Instance.Data.currentStage;

        if (stage == 1)
            Stage1HighestWave = wave;
        else
            Stage2HighestWave = wave;
    }

}
