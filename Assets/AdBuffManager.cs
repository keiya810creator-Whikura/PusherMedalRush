using UnityEngine;

public class AdBuffManager : MonoBehaviour
{
    public static AdBuffManager Instance;

    // =========================
    // 恒常バフ（広告削除）
    // =========================
    private float permanentGold = 1f;
    private float permanentEquip = 1f;
    private float permanentTitle = 1f;

    // =========================
    // 一時バフ（リワード広告）
    // =========================
    private float rewardGold = 1f;
    private float rewardEquip = 1f;
    private float rewardTitle = 1f;

    // =========================
    // 外部参照用（最終倍率）
    // =========================
    public float GoldDropMultiplier => permanentGold * rewardGold;
    public float EquipDropMultiplier => permanentEquip * rewardEquip;
    public float TitleDropMultiplier => permanentTitle * rewardTitle;

    public bool IsRewardBoostActive => rewardGold > 1f;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        // DontDestroyOnLoad(gameObject); ← 同一シーンなら不要
    }

    // =========================
    // 恒常バフ（広告削除）
    // =========================
    public void SetPermanentBoost(float multiplier)
    {
        permanentGold = multiplier;
        permanentEquip = multiplier;
        permanentTitle = multiplier;
    }

    // =========================
    // 一時バフ（リワード広告）
    // =========================
    public void ActivateRewardBuff(float multiplier)
    {
        rewardGold = multiplier;
        rewardEquip = multiplier;
        rewardTitle = multiplier;
    }

    // =========================
    // リワードのみ解除
    // =========================
    public void ClearRewardBuff()
    {
        if (AdventureSession.IsAutoRun == false)
        {
            rewardGold = 1f;
            rewardEquip = 1f;
            rewardTitle = 1f;
        }
    }

    // =========================
    // 全解除（基本使わない）
    // =========================
    public void ClearAll()
    {
        permanentGold = 1f;
        permanentEquip = 1f;
        permanentTitle = 1f;

        ClearRewardBuff();
    }
}
