using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance;

    [Header("Battle Status (Read Only)")]
    [SerializeField] private BattleStatus status;

    public BattleStatus Status => status;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // ✅まず通常ステで確定
        status = BattleStatusBuilder.Build();

        // ✅スキル読み込み
        SkillManager.Instance.RefreshEquippedSkills();

        // ✅Wave開始スキル発動でStatusを変化させる
        SkillManager.Instance.TriggerWaveStart(1);

        Debug.Log($"🔥 BattleManager Awake : {GetInstanceID()}");
    }
    public void AddAttackRate(float rate)
    {
        int add = Mathf.RoundToInt(status.Attack * rate);
        status.Attack += add;

        Debug.Log($"✅Attackスキル上昇 +{rate * 100}% → {status.Attack}");
    }
}