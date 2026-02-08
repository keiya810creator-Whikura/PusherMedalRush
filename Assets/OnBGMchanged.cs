using UnityEngine;
using UnityEngine.UI;
public class OnBGMchanged : MonoBehaviour
{
    [SerializeField] private Slider bgmSlider;
    void Start()
    {
        bgmSlider.value = PlayerPrefs.GetFloat("BGMVolumeValue", 1f);
    }
    public void OnBGMChanged(float value)
    {
        AudioManager.Instance.SetBGMVolume(value);
    }

}
