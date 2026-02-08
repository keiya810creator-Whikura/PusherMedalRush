using UnityEngine;

public class TutorialPanelUI : MonoBehaviour
{
    public TutorialID tutorialID;

    void OnEnable()
    {
        // •\Ž¦‚µ‚Ä‚¢‚¢‚©Šm”F
        if (!TutorialManager.Instance.CanShow(tutorialID))
        {
            gameObject.SetActive(false);
        }
    }

    public void OnClickOK()
    {
        TutorialManager.Instance.MarkDone(tutorialID);
        gameObject.SetActive(false);
    }
}
