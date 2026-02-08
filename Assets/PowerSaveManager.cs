using UnityEngine;
using UnityEngine.UI;

public class PowerSaveManager : MonoBehaviour
{
    public static PowerSaveManager Instance;

    [Header("Overlay UI")]
    [SerializeField] private GameObject overlayPanel;

    private int tapCount = 0;
    private bool isPowerSave = false;

    private void Awake()
    {
        // Singleton
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        //DontDestroyOnLoad(gameObject);

        overlayPanel.SetActive(false);
    }

    private void Update()
    {
        if (!isPowerSave) return;

        if (Input.GetMouseButtonDown(0))
        {
            tapCount++;

            if (tapCount >= 3)
            {
                ExitPowerSaveMode();
            }
        }
    }

    // ✅省電力ON
    public void EnterPowerSaveMode()
    {
        overlayPanel.SetActive(true);
        tapCount = 0;
        isPowerSave = true;
    }

    // ✅省電力OFF
    public void ExitPowerSaveMode()
    {
        overlayPanel.SetActive(false);
        isPowerSave = false;
        tapCount = 0;
    }
}
