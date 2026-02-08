using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UIButtonSE : MonoBehaviour
{
    [Header("鳴らすSE（未設定ならAudioManager.clickSE）")]
    [SerializeField] private AudioClip customSE;

    private Button button;
    private bool initialized = false;

    void Awake()
    {
        button = GetComponent<Button>();
    }

    void OnEnable()
    {
        // ✅ 二重登録防止
        if (initialized) return;
        initialized = true;

        button.onClick.AddListener(PlayClickSE);
    }

    void OnDisable()
    {
        // ✅ 無効化時に解除（安全）
        button.onClick.RemoveListener(PlayClickSE);
        initialized = false;
    }

    private void PlayClickSE()
    {
        if (AudioManager.Instance == null) return;

        // ✅ 個別指定があればそれを鳴らす
        if (customSE != null)
        {
            AudioManager.Instance.PlaySE(customSE);
        }
        else
        {
            AudioManager.Instance.PlaySE(AudioManager.Instance.clickSE);
        }
    }
}
