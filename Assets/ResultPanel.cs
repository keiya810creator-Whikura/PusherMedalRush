using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class ResultPanel : MonoBehaviour
{
    [SerializeField] TMP_Text clearedWaveText;
    [SerializeField] TMP_Text gainedGoldText;

    [SerializeField] EquipmentGridView equipmentGrid;
    [SerializeField] TitleGridView titleGrid;

    [SerializeField] Button retryButton;
    [SerializeField] Button backToMenuButton;

    public static bool IsOpen { get; private set; }

    private void OnEnable()
    {
        IsOpen = true;
        ShowResult();
    }

    private void OnDisable()
    {
        IsOpen = false;
    }
    public void ShowResult()
    {
        var result = ResultManager.Instance.currentResult;

        clearedWaveText.text = string.Format(
            TextManager.Instance.GetUI("ui_resultmenu_2"), result.clearedWave);
        gainedGoldText.text = string.Format(
            TextManager.Instance.GetUI("ui_resultmenu_3"), result.gainedGold);

        equipmentGrid.ShowResult(result.gainedEquipments);
        titleGrid.ShowResult(result.gainedTitles);
    }
    public void OnClickRetry()
    {
        ResultManager.Instance.Clear();
        SceneManager.LoadScene("Stage1");
    }

    public void OnClickBackToMenu()
    {
        ResultManager.Instance.Clear();
        gameObject.SetActive(false);
    }
}
