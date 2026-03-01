using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer mixer;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource seSource;

    [Header("BGM Clips")]
    public AudioClip titleBGM;
    public AudioClip battleBGM;
    public AudioClip bossBGM;

    [Header("SE Clips")]
    public AudioClip clickSE;
    public AudioClip noSE;
    public AudioClip hitSE;
    public AudioClip critikalHitSE;
    public AudioClip shotSE;
    public AudioClip goldKakutokuSE;
    public AudioClip syougouHuyoSE;

    public AudioClip skillHit;
    public AudioClip forsesunder;
    public AudioClip fireBall;
    public AudioClip meteo;
    public AudioClip iceBread;
    public AudioClip uzusio;
    public AudioClip souen;
    public AudioClip windCutter;
    public AudioClip darkBall;
    public AudioClip arsukueiku;
    public AudioClip lightning;

    private float lastHitTime;

    public AudioClip medalSpawn;
    public AudioClip skillHatudou;
    public AudioClip medalKakutoku;
    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
    void Start()
    {
        // ✅保存値読み込み（無ければ1）
        float bgm = PlayerPrefs.GetFloat("BGMVolumeValue", 1f);
        float se = PlayerPrefs.GetFloat("SEVolumeValue", 1f);

        // ✅Mixerへ反映
        SetBGMVolume(bgm);
        SetSEVolume(se);

        // ✅BGM再生
        PlayBGM(titleBGM);
    }

    // =========================
    // ✅ BGM再生
    // =========================
    public void PlayBGM(AudioClip clip)
    {
        if (bgmSource.clip == clip) return;

        bgmSource.clip = clip;
        bgmSource.loop = true;
        bgmSource.Play();
    }

    // =========================
    // ✅ SE再生
    // =========================
    public void PlaySE(AudioClip clip)
    {
        seSource.PlayOneShot(clip);
    }

    // =========================
    // ✅ ヒット音制限付き
    // =========================
    public void PlayHitSE()
    {
        if (Time.time - lastHitTime < 0.05f) return;

        lastHitTime = Time.time;
        PlaySE(hitSE);
    }
    public void MedalKakutokuSE()
    {
        if (Time.time - lastHitTime < 0.05f) return;

        lastHitTime = Time.time;
        PlaySE(medalKakutoku);
    }
    public void PlayCritikalSE()
    {
        if (Time.time - lastHitTime < 0.05f) return;

        lastHitTime = Time.time;
        PlaySE(critikalHitSE);
    }
    // =========================
    // ✅ 音量設定
    // =========================
    public void SetSEVolume(float value)
    {
        if (value <= 0f)
        {
            mixer.SetFloat("SEVolume", -80f); // 完全ミュート
            PlayerPrefs.SetFloat("SEVolumeValue", 0f);
            return;
        }

        value = Mathf.Clamp(value, 0.001f, 1f);
        mixer.SetFloat("SEVolume", Mathf.Log10(value) * 20);
        PlayerPrefs.SetFloat("SEVolumeValue", value);
    }

    public void SetBGMVolume(float value)
    {
        if (value <= 0f)
        {
            mixer.SetFloat("BGMVolume", -80f);
            PlayerPrefs.SetFloat("BGMVolumeValue", 0f);
            return;
        }

        value = Mathf.Clamp(value, 0.001f, 1f);
        mixer.SetFloat("BGMVolume", Mathf.Log10(value) * 20);
        PlayerPrefs.SetFloat("BGMVolumeValue", value);
    }


}
