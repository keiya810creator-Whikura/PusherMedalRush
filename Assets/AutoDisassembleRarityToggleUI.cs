using UnityEngine;
using UnityEngine.UI;

public class AutoDisassembleRarityToggleUI : MonoBehaviour
{
    [SerializeField] private Toggle toggle;
    [SerializeField, Range(1, 6)] private int rarity;

    void Start()
    {
        toggle.isOn = AutoDisassembleSetting.EnableRarity[rarity];
        toggle.onValueChanged.AddListener(OnToggleChanged);
    }

    void OnDestroy()
    {
        toggle.onValueChanged.RemoveListener(OnToggleChanged);
    }

    void OnToggleChanged(bool isOn)
    {
        AutoDisassembleSetting.EnableRarity[rarity] = isOn;

        FindAnyObjectByType<AutoDisassembleSaveBridge>()
            ?.SaveSetting();
    }

}
