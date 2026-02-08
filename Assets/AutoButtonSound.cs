using UnityEngine;
using UnityEngine.UI;

public class AutoButtonSound : MonoBehaviour
{
    void Start()
    {
        Button[] buttons = FindObjectsOfType<Button>(true);

        foreach (Button btn in buttons)
        {
            // ✅すでに登録されてたら追加しない
            btn.onClick.AddListener(() =>
            {
                AudioManager.Instance.PlaySE(AudioManager.Instance.clickSE);
            });
        }

        Debug.Log($"✅ {buttons.Length} buttons にクリック音を自動追加しました");
    }
}
