using UnityEngine;

public class AdBuffManager : MonoBehaviour
{
    public static AdBuffManager Instance;

    public bool IsRewardBoostActive { get; private set; }

    public float GoldDropMultiplier { get; private set; } = 1f;
    public float EquipDropMultiplier { get; private set; } = 1f;
    public float TitleDropMultiplier { get; private set; } = 1f;

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

    public void ActivateRewardBuff(float multiplier)
    {
        GoldDropMultiplier = multiplier;
        EquipDropMultiplier = multiplier;
        TitleDropMultiplier = multiplier;
        IsRewardBoostActive = true;
    }

    public void Clear()
    {
        GoldDropMultiplier = 1f;
        EquipDropMultiplier = 1f;
        TitleDropMultiplier = 1f;
        IsRewardBoostActive = false;
    }
}


