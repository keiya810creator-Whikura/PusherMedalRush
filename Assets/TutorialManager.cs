using UnityEngine;
using System.Collections.Generic;

public enum TutorialID
{
    MainMenu = 1,        // 初回起動 メインメニュー
    FirstBattle_1 = 2,   // 初戦闘①
    FirstBattle_2 = 3,   // 初戦闘②
    StatusMenu = 4,      // 初ステータスメニュー
    EquipmentMenu = 5,   // 初装備メニュー
    TitleAssign = 6      // 初称号付与
}

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;

    private HashSet<TutorialID> opened = new();

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        //DontDestroyOnLoad(gameObject);
        //ResetAllTutorials();
    }

    public bool CanShow(TutorialID id)
    {
        return PlayerPrefs.GetInt($"Tutorial_{id}", 0) == 0;
    }

    public void MarkDone(TutorialID id)
    {
        PlayerPrefs.SetInt($"Tutorial_{id}", 1);
        PlayerPrefs.Save();
    }
    public void ResetAllTutorials()
    {
        foreach (TutorialID id in System.Enum.GetValues(typeof(TutorialID)))
        {
            PlayerPrefs.DeleteKey($"Tutorial_{id}");
        }
        PlayerPrefs.Save();
    }

}

