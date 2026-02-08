using UnityEngine;
public class ResultManager : MonoBehaviour
{
    public static ResultManager Instance;
    public ResultData currentResult = new();

    private void Awake()
    {
        Instance = this;
        //DontDestroyOnLoad(gameObject);
    }

    public void Clear()
    {
        currentResult = new ResultData();
    }

    public void AddGainedEquipment(SoubiInstance soubi)
    {
        // Åö êVÇµÇ¢ List Ç…í«â¡Ç∑ÇÈÇæÇØ
        currentResult.gainedEquipments.Add(soubi);
    }

    public void AddGainedTitle(SyougouData title)
    {
        currentResult.gainedTitles.Add(title);
    }

}

