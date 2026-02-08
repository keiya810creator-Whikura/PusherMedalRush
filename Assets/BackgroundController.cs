using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundController : MonoBehaviour
{
    [Header("Backgrounds")]
    [SerializeField] private List<GameObject> backgrounds;

    [Header("Settings")]
    [SerializeField] private int wavePerBackground = 20;
    [SerializeField] private int randomStartWave = 501;
    [SerializeField] private float fadeDuration = 0.5f;

    private int currentIndex = -1;
    private Coroutine fadeCoroutine;

    // ===============================
    // Wave変更時に呼ぶ
    // ===============================
    public void OnWaveChanged(int wave)
    {
        int nextIndex;

        if (wave < randomStartWave)
        {
            // ---- 500以下：順番固定 ----
            nextIndex = (wave - 1) / wavePerBackground;
            nextIndex = Mathf.Clamp(nextIndex, 0, backgrounds.Count - 1);
        }
        else
        {
            // ---- 501以降：ランダム ----
            nextIndex = GetRandomIndexExceptCurrent();
        }

        if (nextIndex == currentIndex)
            return;

        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeSwitch(nextIndex));
    }

    // ===============================
    // フェード切替
    // ===============================
    private IEnumerator FadeSwitch(int nextIndex)
    {
        GameObject nextBG = backgrounds[nextIndex];
        SpriteRenderer nextSR = nextBG.GetComponent<SpriteRenderer>();

        // 次BG準備
        nextBG.SetActive(true);
        nextSR.color = new Color(1, 1, 1, 0);

        float t = 0f;

        SpriteRenderer currentSR = null;
        if (currentIndex >= 0)
            currentSR = backgrounds[currentIndex].GetComponent<SpriteRenderer>();

        // ---- クロスフェード ----
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float rate = t / fadeDuration;

            if (currentSR != null)
                currentSR.color = new Color(1, 1, 1, 1f - rate);

            nextSR.color = new Color(1, 1, 1, rate);
            yield return null;
        }

        // ---- 後処理 ----
        if (currentIndex >= 0)
        {
            backgrounds[currentIndex].SetActive(false);
            currentSR.color = Color.white;
        }

        nextSR.color = Color.white;
        currentIndex = nextIndex;
        fadeCoroutine = null;
    }

    // ===============================
    // ランダム（同一連続防止）
    // ===============================
    private int GetRandomIndexExceptCurrent()
    {
        if (backgrounds.Count <= 1)
            return 0;

        int index;
        do
        {
            index = Random.Range(0, backgrounds.Count);
        }
        while (index == currentIndex);

        return index;
    }
}
