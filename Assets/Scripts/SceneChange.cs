using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneChange : MonoBehaviour
{
    public void BattleSceneChange()
    {
        SceneManager.LoadScene("戦闘シーン");
    }
    public void MainMenuChange()
    {
        SceneManager.LoadScene("メインメニュー");
    }
}
