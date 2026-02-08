using UnityEngine;

public class EnemyHPScaler
{
    private long currentHP;

    private const int BOSS_INTERVAL = 20;
    private const float BOSS_MULTIPLIER = 1.625f;

    public EnemyHPScaler(long startHP)
    {
        currentHP = startHP;
    }

    public long GetHPForWave(int wave)
    {
        // Wave1は初期値
        if (wave == 1)
            return currentHP;

        // 通常Wave：+1
        currentHP += 1;

        // ボスWave判定
        if (wave % BOSS_INTERVAL == 0)
        {
            currentHP = Mathf.RoundToInt(currentHP * BOSS_MULTIPLIER);
        }

        return currentHP;
    }
}
