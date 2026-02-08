using UnityEngine;

public class ResultSceneManager : MonoBehaviour
{
    [SerializeField] private GameObject resultPanel;

    private void Start()
    {
        if (ResultManager.Instance == null)
            return;

        // ¡‰ñ‚ÌƒvƒŒƒCŒ‹‰Ê‚ª‚ ‚é‚È‚ç•\Ž¦
        if (ResultManager.Instance.currentResult != null &&
            ResultManager.Instance.currentResult.clearedWave > 0)
        {
            resultPanel.SetActive(true);
        }
    }
}
