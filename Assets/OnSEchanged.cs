using UnityEngine;
using UnityEngine.UI;
public class OnSEchanged : MonoBehaviour
{
    [SerializeField] private Slider seSlider;
    void Start()
    {
        seSlider.value = PlayerPrefs.GetFloat("SEVolumeValue", 1f);
    }
    public void OnSEChanged(float value)
    {
        AudioManager.Instance.SetSEVolume(value);
    }

}
