using UnityEngine;
using System.Collections;
using System;
using UnityEngine.SceneManagement;
public enum WaveState
{
    Battle,
    Recovery
}

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance;

    // =========================
    // Wave 基本情報
    // =========================

    public int CurrentWave { get; private set; } = 1;
    public WaveState CurrentState { get; private set; } = WaveState.Battle;

    [Header("Recovery Settings")]
    [SerializeField] private float recoveryTime = 30f;
    //[SerializeField] private EnemySpawner enemySpawner;

    public event Action<int> OnWaveChanged;
    public event Action<float> OnRecoveryTimeChanged;
    public event Action<bool> OnRecoveryStateChanged;

    public int startSliderValue;
    public int endSliderValue;

    private bool isEndlessRun;
    private long currentEnemyHP = 5;
    private const int BossInterval = 40;
    private const float BossMultiplier = 1.475f;

    // =========================
    // Wave501+ Endless Settings
    // =========================
    private const int EndlessStartWave = 1001;
    private const float EndlessHpMultiplier = 1.001f; // HP × 1.001 (毎Wave)
    private const long EndlessGoldReward = 500;       // Gold 固定

    [SerializeField] private BackgroundController backgroundController;
    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        Time.timeScale = 1f;
        // ✅戦闘開始時にSpeed適用
        SetGameSpeed(AdventureSession.GameSpeed);
    }

    // =========================
    // Enemy Status（Wave依存）
    // =========================

    public long GetEnemyHP()
    {
        return currentEnemyHP;
    }



    public long GetEnemyGoldReward()
    {
        // ✅Wave1001以降：Goldは固定
        if (CurrentWave >= EndlessStartWave)
        {
            return EndlessGoldReward;
        }

        long baseGold = Mathf.CeilToInt(CurrentWave/2);

        if (IsBossWave())
        {
            baseGold *= 2;
        }

        return baseGold;
    }

    // =========================
    // Wave 判定
    // =========================

    public bool IsBossWave()
    {
        // ✅Wave1001以降：ボス無し
        if (CurrentWave >= EndlessStartWave) return false;
        return CurrentWave % 40 == 0;
    }

    public bool IsRecoveryWave()
    {
        return CurrentWave % 10 == 0;
    }

    // =========================
    // Wave 進行管理
    // =========================

    /// <summary>
    /// 敵が倒されたときに Enemy から呼ばれる
    /// </summary>
    public void OnEnemyDefeated()
    {
        // ★ 何もしない or ログだけ
        Debug.Log("Enemy defeated (waiting for soul)");
    }
    void NextWave()
    {
        int clearedWave = CurrentWave;

        GameProgressManager.Instance.RecordClearedWave(clearedWave);
        ResultManager.Instance.currentResult.clearedWave = clearedWave;

        if (ShouldEndAtWave(clearedWave))
        {
            if (isEndlessRun)
            {
                AdventureSession.IsEndless = true;
            }
            ResultFlowController.Instance.GoToResult();
            return;
        }

        CurrentWave++;

        // =========================
        // Enemy HP Growth
        // =========================
        if (CurrentWave >= EndlessStartWave)
        {
            // ✅Wave1001以降：HP×1.001（ボス倍率なし）
            double next = Math.Ceiling(currentEnemyHP * EndlessHpMultiplier);
            long nextHp = (long)next;
            if (nextHp <= currentEnemyHP) nextHp = currentEnemyHP + 1; // 念のため停滞防止
            currentEnemyHP = nextHp;
        }
        else
        {
            // ✅Wave1〜1000：従来ロジック（毎Wave +10 / ボス倍率）
            currentEnemyHP += 10;

            // ✅ボスWaveなら倍率
            if (CurrentWave % BossInterval == 0)
            {
                currentEnemyHP = (long)(currentEnemyHP * BossMultiplier);
            }
        }
        if (CurrentWave % 40 == 1 && CurrentWave != 1&&CurrentWave<=1000)
        {
            ToastManager.Instance.ShowToast(string.Format(TextManager.Instance.GetUI("ui_toast_3")));
        }

        StartBattleWave();
        AdventureSession.CurrentWave = CurrentWave; // ✅進行保存
    }


    bool ShouldEndAtWave(int clearedWave)
    {
        if (AdventureSession.IsEndless)
            return false;

        return clearedWave >= AdventureSession.EndWave;
    }

    private bool abyssin = false;
    void StartBattleWave()
    {
        if (EnemySpawner.Instance == null) return;

        if (backgroundController != null)
            backgroundController.OnWaveChanged(CurrentWave);

        SkillManager.Instance.TriggerWaveStart(CurrentWave);
        waveTransitionLocked = false;

        CurrentState = WaveState.Battle;
        OnWaveChanged?.Invoke(CurrentWave);
        OnRecoveryStateChanged?.Invoke(false);

        // ✅BGM切り替え（ボスならボス曲）
        if (AudioManager.Instance != null)
        {
            if (IsBossWave())
            {
                AudioManager.Instance.PlayBGM(AudioManager.Instance.bossBGM);
            }
            else
            {
                AudioManager.Instance.PlayBGM(AudioManager.Instance.battleBGM);
            }
        }

        EnemySpawner.Instance.SpawnEnemy(CurrentWave);

        if (CurrentWave >= EndlessStartWave && abyssin == false)
        {
            ToastManager.Instance.ShowToast(TextManager.Instance.GetUI("ui_toast_7"));
            abyssin = true;
        }
    }




    // =========================
    // Recovery Wave
    // =========================

    IEnumerator RecoveryRoutine()
    {
        ToastManager.Instance.ShowToast(string.Format(TextManager.Instance.GetUI("ui_toast_2")));

        CurrentState = WaveState.Recovery;
        OnRecoveryStateChanged?.Invoke(true);

        float recoveryTime = RecoveryWaveSetting.RecoverySeconds;
        while (recoveryTime > 0f)
        {
            recoveryTime -= Time.deltaTime * WaveManager.Instance.CurrentGameSpeed;
            OnRecoveryTimeChanged?.Invoke(recoveryTime);
            yield return null;
        }

        CurrentState = WaveState.Battle;
        OnRecoveryStateChanged?.Invoke(false);

        NextWave();
    }

    public long GetConsumeMedalPerShot()
    {
        return 1 + (CurrentWave - 1) / BossInterval;
    }


    // =========================
    // Hooks（他クラスと接続）
    // =========================

    void SpawnEnemy()
    {
        // EnemySpawner.Instance.Spawn();
        // もしくは Inspector 参照でもOK
    }
    /// <summary>
    /// ゴールドの演出用ドロップ枚数（見た目用）
    /// </summary>
    public int GetEnemyGoldDropVisualCount()
    {
        // ボスは派手に
        if (IsBossWave())
        {
            return 10;
        }

        // 通常敵は最大5枚
        return 5;
    }
    private bool waveTransitionLocked = false;

    public void OnSoulFinished()
    {
        // ★ まだ敵が残っているなら Wave を進めない
        if (GameObject.FindGameObjectWithTag("Enemy") != null)
        {
            Debug.Log("Enemy still exists. Ignore soul finish.");
            return;
        }

        if (CurrentState != WaveState.Battle)
            return;

        if (waveTransitionLocked)
            return;

        waveTransitionLocked = true;

        if (IsRecoveryWave())
        {
            int clearedWave = CurrentWave;

            // 記録（進行用）
            GameProgressManager.Instance.RecordClearedWave(clearedWave);
            ResultManager.Instance.currentResult.clearedWave = clearedWave;
            if (ShouldEndAtWave(clearedWave))
            {
                // ★ Endless維持
                if (isEndlessRun)
                {
                    AdventureSession.IsEndless = true;
                }

                ResultFlowController.Instance.GoToResult();
                return;
            }


            StartCoroutine(RecoveryRoutine());
        }
        else
        {
            NextWave();
        }
    }
    public void StartFromWave(int startWave)
    {
        isEndlessRun = AdventureSession.IsEndless;

        // ✅スライダー指定Waveで開始
        CurrentWave = startWave;

        // ✅進行保存も更新
        AdventureSession.CurrentWave = startWave;

        // ✅倍速復元
        SetGameSpeed(AdventureSession.GameSpeed);

        // ✅HP再構築
        RecalculateEnemyHP();

        StartBattleWave();
    }



    public float CurrentGameSpeed { get; private set; } = 1f;

    public void SetGameSpeed(float speed)
    {
        // ここでは保存するだけ
        CurrentGameSpeed = speed;
        // Debug.Log(CurrentGameSpeed);

    }
    private void RecalculateEnemyHP()
    {
        currentEnemyHP = 5;

        for (int w = 2; w <= CurrentWave; w++)
        {
            if (w >= EndlessStartWave)
            {
                // ✅501以降：×1.001成長
                currentEnemyHP =
                    (long)Math.Ceiling(currentEnemyHP * EndlessHpMultiplier);

                if (currentEnemyHP <= 0)
                    currentEnemyHP = 1;
            }
            else
            {
                // ✅Wave1〜500：+10成長
                currentEnemyHP += 10;

                // ✅ボス倍率
                if (w % BossInterval == 0)
                {
                    currentEnemyHP =
                        (long)(currentEnemyHP * BossMultiplier);
                }
            }
        }
    }
    private void OnDestroy()
    {
        StopAllCoroutines();
    }
    // =========================
    // ✅Waveスキップスキル用
    // =========================
    public void SkipWave(int count)
    {
        if (count <= 0) return;

        Debug.Log($"⏭ Waveスキップ発動：{count}回");

        for (int i = 0; i < count; i++)
        {
            // ✅終了Waveに到達してたら止める
            if (ShouldEndAtWave(CurrentWave))
                return;

            // ✅RecoveryWaveはスキップ禁止（事故防止）
            if (IsRecoveryWave())
            {
                Debug.Log("⚠ RecoveryWave中はスキップ禁止");
                return;
            }

            // ✅Waveを進める（内部HP成長も進む）
            NextWave();
        }
    }
    public void ForceSkip(int count)
    {
        for (int i = 0; i < count; i++)
            OnSoulFinished(); // 次Waveへ
    }
}
