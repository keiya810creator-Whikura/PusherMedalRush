using UnityEngine;

public class StageManager : MonoBehaviour
{
    public static StageManager Instance;

    void Awake()
    {
        Instance = this;
    }

    public int CurrentStage
    {
        get { return SaveManager.Instance.Data.currentStage; }
    }

    public void SetStage(int stage)
    {
        SaveManager.Instance.Data.currentStage = stage;
        SaveManager.Instance.SaveToDisk();
    }

    public bool IsStage2Unlocked()
    {
        return SaveManager.Instance.Data.stage2Unlocked;
    }

    public void UnlockStage2()
    {
        var save = SaveManager.Instance.Data;

        if (!save.stage2Unlocked)
        {
            save.stage2Unlocked = true;
            SaveManager.Instance.SaveToDisk();
        }
    }

}