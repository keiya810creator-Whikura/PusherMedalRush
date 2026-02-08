using UnityEngine;

public class PowerSave : MonoBehaviour
{
    public void OnClickPowerSave()
    {
        PowerSaveManager.Instance.EnterPowerSaveMode();
    }

}
