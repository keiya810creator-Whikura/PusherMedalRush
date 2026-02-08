using UnityEngine;
using System.IO;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }
    public SaveData Data { get; private set; }

    string SavePath => Path.Combine(Application.persistentDataPath, "save.json");

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        //DontDestroyOnLoad(gameObject);

        Load();
    }

    public void SaveToDisk()
    {
        string json = JsonUtility.ToJson(Data, true);
        File.WriteAllText(SavePath, json);
        //Debug.Log($"✅ Save: {SavePath}");
    }

    public void Load()
    {
        if (!File.Exists(SavePath))
        {
            Data = new SaveData();
            Debug.Log("🆕 No save file. New SaveData created.");
            return;
        }

        string json = File.ReadAllText(SavePath);
        Data = JsonUtility.FromJson<SaveData>(json) ?? new SaveData();
        //Debug.Log("✅ Load ok");
    }

    public void DeleteSave()
    {
        if (File.Exists(SavePath)) File.Delete(SavePath);
        Data = new SaveData();
        Debug.Log("🗑 Save deleted");
    }
}
