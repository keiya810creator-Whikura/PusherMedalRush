using UnityEngine;

public class StatusPanel : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private StatRow statRowPrefab;

    [Header("UI")]
    [SerializeField] private Transform contentRoot;
    public static StatusPanel Instance;
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    public GameObject statusMenuTutorialPanel;
    void Start()
    {
        if (TutorialManager.Instance.CanShow(TutorialID.StatusMenu))
            statusMenuTutorialPanel.SetActive(true);
    }

    private void OnEnable()
    {
        BuildStatusRows();
    }

    /// <summary>
    /// ステータス行をすべて生成する
    /// </summary>
    public void BuildStatusRows()
    {
        // 既存行をクリア（再生成対応）
        foreach (Transform child in contentRoot)
        {
            Destroy(child.gameObject);
        }

        // Enum順 = UI並び順
        foreach (var type in PlayerStatusDatabase.Instance.GetAllStatusTypes())
        {
            StatRow row = Instantiate(statRowPrefab, contentRoot);
            row.Init(type);
        }

    }
}
