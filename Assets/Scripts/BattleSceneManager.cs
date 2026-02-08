using UnityEngine;

public class BattleSceneManager : MonoBehaviour
{
    void Start()
    {
        WaveManager.Instance.StartFromWave(AdventureSession.StartWave);
    }

}

