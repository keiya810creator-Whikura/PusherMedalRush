using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class WaveUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI waveText;
    [SerializeField] private TextMeshProUGUI recoveryText;

    void Start()
    {
        recoveryText.gameObject.SetActive(false);

        WaveManager.Instance.OnWaveChanged += UpdateWaveText;
        WaveManager.Instance.OnRecoveryTimeChanged += UpdateRecoveryTime;
        WaveManager.Instance.OnRecoveryStateChanged += ToggleRecoveryUI;
    }

    void OnDestroy()
    {
        if (WaveManager.Instance == null) return;

        WaveManager.Instance.OnWaveChanged -= UpdateWaveText;
        WaveManager.Instance.OnRecoveryTimeChanged -= UpdateRecoveryTime;
        WaveManager.Instance.OnRecoveryStateChanged -= ToggleRecoveryUI;
    }

    void UpdateWaveText(int wave)
    {
        waveText.text =
            LocalizationManager.Instance.GetText("wave", wave);
    }

    void UpdateRecoveryTime(float time)
    {
        recoveryText.text =
            string.Format(TextManager.Instance.GetUI("ui_sentou_11"),
        Mathf.CeilToInt(time));
    }


    void ToggleRecoveryUI(bool isRecovery)
    {
        recoveryText.gameObject.SetActive(isRecovery);
    }
}
