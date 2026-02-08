using UnityEngine;

public class SentouTyuutorialPanelOpen : MonoBehaviour
{
    public GameObject battleTutorial1;
    public GameObject battleTutorial2;
    void Start()
    {
        // 戦闘開始
        if (TutorialManager.Instance.CanShow(TutorialID.FirstBattle_1))
            battleTutorial1.SetActive(true);

    }
    // パネル2のOK
    public void OnClickBattle1OK()
    {
        TutorialManager.Instance.MarkDone(TutorialID.FirstBattle_1);
       // battleTutorial1.SetActive(false);

        if (TutorialManager.Instance.CanShow(TutorialID.FirstBattle_2))
            battleTutorial2.SetActive(true);
    }

}
