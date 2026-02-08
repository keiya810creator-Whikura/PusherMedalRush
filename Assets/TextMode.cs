using UnityEngine;

public class TextMode : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Tap()
    {
        GameProgressManager.Instance.RecordClearedWave(500);
    }
}
