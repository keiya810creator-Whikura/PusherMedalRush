using UnityEngine;
using UnityEngine.UI;

public static class AutoDisassembleSetting
{
    // index: 1〜6 を使用
    public static bool[] EnableRarity = new bool[7];

    /// <summary>
    /// 自動分解が有効か（どれか1つでもONならtrue）
    /// </summary>
    public static bool IsEnabled
    {
        get
        {
            for (int i = 1; i <= 6; i++)
            {
                if (EnableRarity[i])
                    return true;
            }
            return false;
        }
    }
}

