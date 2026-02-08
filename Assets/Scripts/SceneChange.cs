using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneChange : MonoBehaviour
{
    public void BattleSceneChange()
    {
        SceneManager.LoadScene("Stage1");
    }
    public void MainMenuChange()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
