#if UNITY_EDITOR
using UnityEditor;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public class PlayFromTitle
{
    static PlayFromTitle()
    {
        EditorApplication.playModeStateChanged += state =>
        {
            if (state == PlayModeStateChange.EnteredPlayMode)
            {
                if (SceneManager.GetActiveScene().name != "TitleScene")
                {
                    SceneManager.LoadScene("TitleScene");
                }
            }
        };
    }
}
#endif
