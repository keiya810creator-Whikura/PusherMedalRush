using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class TitleGridView : MonoBehaviour
{
    [SerializeField] Transform content;
    [SerializeField] TitleRowUI slotPrefab;
    [SerializeField] RarityColorTable rarityTable;

    public void ShowResult(List<SyougouData> list)
    {
        // 既存クリア
        foreach (Transform child in content)
            Destroy(child.gameObject);

        if (list == null || list.Count == 0)
            return;

        // 同一称号をまとめる（SO参照でグループ化）
        var grouped = list
            .Where(t => t != null)
            .GroupBy(t => t)
            .Select(g => new InventoryManager.SyougouStack
            {
                data = g.Key,
                count = g.Count()
            })
            .ToList();

        foreach (var stack in grouped)
        {
            var slot = Instantiate(slotPrefab, content);

            // ★ ResultGained モードで表示（付与ボタン非表示）
            slot.Set(
                stack,
                null, // Resultではクリック処理不要
                rarityTable,
                TitleSelectPanel.TitleSelectMode.ResultGained
            );
        }
    }
}
