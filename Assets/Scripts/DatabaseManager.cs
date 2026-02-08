using UnityEngine;

public class DatabaseManager : MonoBehaviour
{
    public static DatabaseManager Instance { get; private set; }

    [Header("Databases")]
    [SerializeField] private SoubiDatabase soubiDatabase;
    [SerializeField] private SyougouDatabase shogoDatabase;
    public SyougouDatabase sinnsouSyougouDatabase;

    [SerializeField] private MonsterDatabase monsterDatabase;

    public SoubiDatabase SoubiDB => soubiDatabase;
    public SyougouDatabase ShogoDB => shogoDatabase;
    public SyougouDatabase SinsouSyougouShogoDB => sinnsouSyougouDatabase;
    public MonsterDatabase MonsterDB => monsterDatabase;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
       // DontDestroyOnLoad(gameObject);
    }
}
