using UnityEngine;

public class MonsterGridView : MonoBehaviour
{
    [Header("Database")]
    [SerializeField] private MonsterDatabase monsterDatabase;

    [Header("UI")]
    [SerializeField] private Transform contentParent;
    [SerializeField] private MonsterSlotUI slotPrefab;

    [Header("Detail Panel")]
    [SerializeField] private MonsterDetailPanel detailPanel;

    void Start()
    {
        BuildGrid();
    }

    /// <summary>
    /// 図鑑スロットを全生成
    /// </summary>
    void BuildGrid()
    {
        // ✅初期化
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        // ✅全モンスターを並べる
        foreach (var monster in monsterDatabase.All)
        {
            var slot = Instantiate(slotPrefab, contentParent);

            // ✅遭遇済みかチェック（Seenなら解放）
            bool unlocked =
    ZukanProgressManager.Instance.IsEncountered(monster.id);


            // スロット初期化
            slot.Setup(monster, unlocked);

            // タップイベント
            slot.OnClickSlot += OnClickMonster;
        }

        Debug.Log("モンスター図鑑Grid生成完了（遭遇解放式）");
    }

    /// <summary>
    /// スロットを押したら詳細表示
    /// </summary>
    void OnClickMonster(MonsterData monster)
    {
        detailPanel.Show(monster);
    }

    /// <summary>
    /// 外部から図鑑を更新したい場合用
    /// </summary>
    public void Refresh()
    {
        BuildGrid();
    }
}
