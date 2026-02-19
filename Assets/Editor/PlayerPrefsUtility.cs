using UnityEditor;
using UnityEngine;

public static class PlayerPrefsUtility
{
    [MenuItem("Tools/PlayerPrefs/Delete All")]
    public static void DeleteAll()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Debug.Log("PlayerPrefs: DeleteAll");
    }

    [MenuItem("Tools/PlayerPrefs/Delete Tutorials")]
    public static void DeleteTutorials()
    {
        foreach (TutorialID id in System.Enum.GetValues(typeof(TutorialID)))
        {
            PlayerPrefs.DeleteKey($"Tutorial_{id}");
        }
        PlayerPrefs.Save();
        Debug.Log("PlayerPrefs: Tutorial data deleted");
    }
}
