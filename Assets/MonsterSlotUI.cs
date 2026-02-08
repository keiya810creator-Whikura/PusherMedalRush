using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class MonsterSlotUI : MonoBehaviour
{
    [Header("UI Parts")]
    [SerializeField] private Image iconImage;
    [SerializeField] private GameObject secretOverlay;
    [SerializeField] private GameObject bossBadge;

    private MonsterData monsterData;
    private bool isUnlocked;

    // 外部通知（GridViewが受け取る）
    public Action<MonsterData> OnClickSlot;

    /// <summary>
    /// スロット初期化
    /// </summary>
    public void Setup(MonsterData data, bool unlocked)
    {
        monsterData = data;
        isUnlocked = unlocked;

        // アイコン設定
        iconImage.sprite = data.icon;

        // 未解放なら？？表示
        secretOverlay.SetActive(!unlocked);

        // ボスバッジ表示
        bossBadge.SetActive(unlocked && data.isBoss);
    }

    /// <summary>
    /// ボタン押下
    /// </summary>
    public void OnClick()
    {
        if (!isUnlocked)
        {
            Debug.Log("未遭遇モンスターです");
            return;
        }

        OnClickSlot?.Invoke(monsterData);
    }
}
