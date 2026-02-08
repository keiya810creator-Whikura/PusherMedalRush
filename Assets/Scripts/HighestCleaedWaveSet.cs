using UnityEngine;

public class HighestCleaedWaveSet : MonoBehaviour
{
    public void HighWaveSet()
    {
        GameProgressManager.Instance.RecordClearedWave(2000);
    }
}
