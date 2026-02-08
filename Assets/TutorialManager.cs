using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    [Header("Step Prefabs（順番通りに手動登録）")]
    [SerializeField] private GameObject[] tutorialStepPrefabs;

    [Header("生成先（Canvasの中にする）")]
    [SerializeField] private Transform tutorialParent;

    private int currentIndex = 0;

    private const string PREF_KEY = "TutorialDone";

    void Start()
    {
        // ✅2回目以降は表示しない
        if (PlayerPrefs.GetInt(PREF_KEY, 0) == 1)
            return;

        ShowStep(0);
    }

    public void ShowStep(int index)
    {
        if (index >= tutorialStepPrefabs.Length)
        {
            FinishTutorial();
            return;
        }

        currentIndex = index;

        // ✅Prefab生成
        Instantiate(tutorialStepPrefabs[currentIndex], tutorialParent);
    }

    public void NextStep()
    {
        currentIndex++;
        ShowStep(currentIndex);
    }

    private void FinishTutorial()
    {
        PlayerPrefs.SetInt(PREF_KEY, 1);
        Debug.Log("✅ Tutorial Finished!");
    }
}