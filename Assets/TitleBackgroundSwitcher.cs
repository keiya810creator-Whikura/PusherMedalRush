using UnityEngine;
using UnityEngine.UI;
using System;

public class TitleBackgroundSwitcher : MonoBehaviour
{
    [Serializable]
    public class LanguageBackground
    {
        public SystemLanguage language;
        public Sprite backgroundSprite;
    }

    [Header("Background Target")]
    [SerializeField] private Image backgroundImage;            // UIの場合
    [SerializeField] private SpriteRenderer backgroundRenderer; // Spriteの場合

    [Header("Language Backgrounds")]
    [SerializeField] private LanguageBackground[] backgrounds;

    private void Start()
    {
        ApplyBackground();
    }

    // -------------------------
    // 言語に応じて背景切り替え
    // -------------------------
    public void ApplyBackground()
    {
        if (TextManager.Instance == null)
        {
            Debug.LogWarning("❌ TextManager not found");
            return;
        }

        var lang = TextManager.Instance.currentLanguage;

        foreach (var bg in backgrounds)
        {
            if (bg.language == lang && bg.backgroundSprite != null)
            {
                SetSprite(bg.backgroundSprite);
                return;
            }
        }

        Debug.LogWarning($"⚠ Background not found for language: {lang}");
    }

    private void SetSprite(Sprite sprite)
    {
        if (backgroundImage != null)
            backgroundImage.sprite = sprite;

        if (backgroundRenderer != null)
            backgroundRenderer.sprite = sprite;
    }
}
