using UnityEngine;
using TMPro;

public class DexProgressUI : MonoBehaviour
{
    [Header("Databases")]
    [SerializeField] private MonsterDatabase monsterDatabase;
    [SerializeField] private SoubiDatabase soubiDatabase;
    [SerializeField] private SyougouDatabase syougouDatabase;

    [Header("UI")]
    [SerializeField] private TMP_Text progressText;

    void Start()
    {
        Refresh();
    }

    public void Refresh()
    {
        int monsterTotal = monsterDatabase.All.Count;
        int equipTotal = soubiDatabase.All.Count;
        int titleTotal = syougouDatabase.All.Count;

        // ✅遭遇数（Monster）
        int monsterEncountered =
            ZukanProgressManager.Instance.GetEncounteredList().Count;

        // ✅取得数（Equip）
        int equipObtained =
            ZukanProgressManager.Instance.GetEquipList().Count;

        // ✅取得数（Title）
        int titleObtained =
            ZukanProgressManager.Instance.GetTitleList().Count;

        // ✅割合計算
        float monsterRate = monsterTotal > 0
            ? (float)monsterEncountered / monsterTotal
            : 0f;

        float equipRate = equipTotal > 0
            ? (float)equipObtained / equipTotal
            : 0f;

        float titleRate = titleTotal > 0
            ? (float)titleObtained / titleTotal
            : 0f;

        // ✅全体達成率
        int totalCount = monsterTotal + equipTotal + titleTotal;
        int totalUnlocked = monsterEncountered + equipObtained + titleObtained;

        float totalRate = totalCount > 0
            ? (float)totalUnlocked / totalCount
            : 0f;

        // ✅表示
        progressText.text = string.Format(
            TextManager.Instance.GetUI("ui_mainmenu_8_8"), totalRate * 100, monsterRate * 100, equipRate * 100,titleRate*100);
    }
}
