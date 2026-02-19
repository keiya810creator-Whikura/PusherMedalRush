using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
public class MainMenuController : MonoBehaviour
{
    [SerializeField] private WaveRangeSliderUI waveRangeUI;
    [SerializeField] private GameObject meinMenuPanel;
    [SerializeField] private GameObject statusMenuPanel;
    [SerializeField] private GameObject soubiMenuPanel;
    [SerializeField] private GameObject resultPanel;
    [SerializeField] private GameObject overlayRoot;
    [SerializeField] private Button rewardBoostButton;
    [SerializeField] private TMP_Text rewardBoostButtonText;
    [SerializeField] private TMP_Text dropInfoText;
    [SerializeField] private GameObject sonotaPanel;
    [SerializeField] private GameObject shopPanel;

    [SerializeField] private GameObject mainMenuTutorialPanel;
    [SerializeField] private GameObject mainMenuTutorialPanel2;

    private void Start()
    {
        /*foreach (TutorialID id in System.Enum.GetValues(typeof(TutorialID)))
            PlayerPrefs.DeleteKey($"Tutorial_{id}");

        PlayerPrefs.Save();*/

        AudioManager.Instance.PlayBGM(AudioManager.Instance.titleBGM);
        
        if(AdventureSession.IsAutoRun==false)
        ResultManager.Instance.Clear();

        if (TutorialManager.Instance.CanShow(TutorialID.MainMenu))
            mainMenuTutorialPanel.SetActive(true);

        /*if(TutorialManager.Instance.IsMainMenuSecondCondition())
        {
            mainMenuTutorialPanel2.SetActive(true);
        }*/

        AdBuffManager.Instance.ClearRewardBuff();
    }
    public void RefreshRewardBoostButton()
    {// ✅広告削除課金済なら常時ON表示
        if (PurchaseState.HasRemoveAds)
        {
            rewardBoostButton.interactable = false;
            rewardBoostButtonText.text =
                string.Format(TextManager.Instance.GetUI("ui_mainmenu_1_3"));
            rewardBoostButton.image.color =
                new Color(0.8f, 0.8f, 1f);

            RefreshFinalDropText();
            return;
        }

        if (AdBuffManager.Instance.IsRewardBoostActive)
        {
            rewardBoostButton.interactable = false;
            rewardBoostButtonText.text =
                string.Format(TextManager.Instance.GetUI("ui_mainmenu_1_12"));
            rewardBoostButton.interactable = false;
            rewardBoostButton.image.color = new Color(0.7f, 0.7f, 0.7f);
            RefreshFinalDropText();
        }
        else
        {
            rewardBoostButton.interactable = true;
            rewardBoostButtonText.text =
                string.Format(TextManager.Instance.GetUI("ui_mainmenu_1_4"));
        }
    }
   public void RefreshFinalDropText()
    {
        // ★ BattleManager は使わない
        BattleStatus status = BattleStatusBuilder.Build();

        string equipText =
            string.Format(
        TextManager.Instance.GetUI("ui_mainmenu_1_5"),
        status.EquipDropRate*100f
        );
        string titleText =
            string.Format(
        TextManager.Instance.GetUI("ui_mainmenu_1_6"),
        status.TitleDropRate * 100f
        );

        string goldText =
            string.Format(
        TextManager.Instance.GetUI("ui_mainmenu_1_7"),
        status.GoldDropRate * 100f
        );

        dropInfoText.text =
            $"{equipText}{titleText}\n{goldText}";
    }
    public void OnClickStartAdventure()
{
    // ✅倉庫満杯なら出発できない
    if (InventoryManager.Instance.TotalCount >= InventoryManager.Instance.MaxStorage)
    {
            ToastManager.Instance.ShowToast(
                string.Format(TextManager.Instance.GetUI("ui_toast_4")));
        return;
    }

    AdventureSession.StartWave = WaveRangeSliderUI.instance.StartWave;

    if (!AdventureSession.IsEndless)
        AdventureSession.EndWave = WaveRangeSliderUI.instance.EndWave;

    // ✅広告削除課金なら冒険開始時に常時2倍適用
    if (PurchaseState.HasRemoveAds)
    {
        AdBuffManager.Instance.ActivateRewardBuff(2f);
    }

    SceneManager.LoadScene("Stage1");
}
    public void OnClickAutoRun()
    {
        // ✅ まず倉庫チェック（最優先）
        if (InventoryManager.Instance.TotalCount >= InventoryManager.Instance.MaxStorage)
        {
            ToastManager.Instance.ShowToast(
                string.Format(TextManager.Instance.GetUI("ui_toast_4"))
            );
            return;
        }

        // ✅広告削除済なら即開始
        if (PurchaseState.HasRemoveAds)
        {
            StartAutoRunDirect();
            return;
        }

        // ✅AdMob Rewarded
        if (AdMobRewardedManager.Instance != null &&
            AdMobRewardedManager.Instance.IsReady())
        {
            AdMobRewardedManager.Instance.ShowRewarded(() =>
            {
                StartAutoRunDirect();
            });
        }
        else
        {
            ToastManager.Instance.ShowToast(
                string.Format(TextManager.Instance.GetUI("ui_toast_ad_loading"))
            );
        }
    }


    private void StartAutoRunDirect()
    {
        // ★ これを必ず最初に
        Time.timeScale = 1f;
        // ✅倉庫満杯なら出発できない
        if (InventoryManager.Instance.TotalCount >= InventoryManager.Instance.MaxStorage)
        {
            ToastManager.Instance.ShowToast(string.Format(TextManager.Instance.GetUI("ui_toast_4")));
            return;
        }

        AdventureSession.StartWave = waveRangeUI.StartWave;
        AdventureSession.EndWave = waveRangeUI.EndWave;

        AutoRunController.Instance.StartAutoRun();
        SceneManager.LoadScene("Stage1");
    }


    public void OnClickRewardBoostAd()
    {
        if (AdBuffManager.Instance.IsRewardBoostActive)
            return;

        // ✅広告削除済なら即発動
        if (PurchaseState.HasRemoveAds)
        {
            ActivateRewardBoostDirect();
            return;
        }

        // ✅AdMob Rewarded
        if (AdMobRewardedManager.Instance != null &&
            AdMobRewardedManager.Instance.IsReady())
        {
            AdMobRewardedManager.Instance.ShowRewarded(() =>
            {
                ActivateRewardBoostDirect();
            });
        }
        else
        {
            ToastManager.Instance.ShowToast(
                string.Format(TextManager.Instance.GetUI("ui_toast_ad_loading"))
            );
        }
    }


    private void ActivateRewardBoostDirect()
    {
        AdBuffManager.Instance.ActivateRewardBuff(2f);
        RefreshRewardBoostButton();
    }



    public void OpenMeinMenu()
    {
        if (AdventureSession.IsAutoRun == true)
        {
            AdventureSession.IsAutoRun = false;
            AdBuffManager.Instance.ClearRewardBuff();
        }

        // ✅一旦バフ解除
        ApplyRemoveAdsPermanentBoost();

        resultPanel.SetActive(false);
        meinMenuPanel.SetActive(true);
        statusMenuPanel.SetActive(false);
        soubiMenuPanel.SetActive(false);
        sonotaPanel.SetActive(false);
        shopPanel.SetActive(false);

        overlayRoot.SetActive(false);
        overlayRoot.SetActive(true);

        ResultManager.Instance.Clear();

        // ✅ボタンと表示更新
        RefreshRewardBoostButton();
        RefreshFinalDropText();
        InventoryManager.Instance.isDismantleMode = false;

        if (TutorialManager.Instance.IsMainMenuSecondCondition())
        {
            mainMenuTutorialPanel2.SetActive(true);
        }
    }

    public void OpenStatusMenuPanel()
    {
        if (AdventureSession.IsAutoRun == true)
        {
            AdventureSession.IsAutoRun = false;
            AdBuffManager.Instance.ClearRewardBuff();
        }
            ApplyRemoveAdsPermanentBoost(); resultPanel.SetActive(false);
        meinMenuPanel.SetActive(false);
        statusMenuPanel.SetActive(true);
        soubiMenuPanel.SetActive(false);
        sonotaPanel.SetActive(false);
        shopPanel.SetActive(false);

        overlayRoot.SetActive(false);
        overlayRoot.SetActive(true);
        ResultManager.Instance.Clear();

    }
    public void OpenSoubiMenuPanel()
    {
        if (AdventureSession.IsAutoRun == true)
        {
            AdventureSession.IsAutoRun = false;
            AdBuffManager.Instance.ClearRewardBuff();
        }
        ApplyRemoveAdsPermanentBoost(); resultPanel.SetActive(false);
        meinMenuPanel.SetActive(false);
        statusMenuPanel.SetActive(false);
        soubiMenuPanel.SetActive(true);
        sonotaPanel.SetActive(false);
        shopPanel.SetActive(false);

        overlayRoot.SetActive(false);
        overlayRoot.SetActive(true);
        EquipmentGridView.Instance.Refresh();
        ResultManager.Instance.Clear();

    }
    public void OpenSonotaPanel()
    {
        if (AdventureSession.IsAutoRun == true)
        {
            AdventureSession.IsAutoRun = false;
            AdBuffManager.Instance.ClearRewardBuff();
        }
        ApplyRemoveAdsPermanentBoost(); resultPanel.SetActive(false);
        meinMenuPanel.SetActive(false);
        statusMenuPanel.SetActive(false);
        soubiMenuPanel.SetActive(false);
        sonotaPanel.SetActive(true);
        shopPanel.SetActive(false);
        overlayRoot.SetActive(false);
        overlayRoot.SetActive(true);
        ResultManager.Instance.Clear();

    }
    public void OpenSHOPPanel()
    {
        if (AdventureSession.IsAutoRun == true)
        {
            AdventureSession.IsAutoRun = false;
            AdBuffManager.Instance.ClearRewardBuff();
        }
        ApplyRemoveAdsPermanentBoost(); resultPanel.SetActive(false);
        meinMenuPanel.SetActive(false);
        statusMenuPanel.SetActive(false);
        soubiMenuPanel.SetActive(false);
        sonotaPanel.SetActive(false);
        shopPanel.SetActive(true);
        overlayRoot.SetActive(false);
        overlayRoot.SetActive(true);

        ResultManager.Instance.Clear();

    }
    public void OnClickBackToMenu()
    {
        if (AdventureSession.IsAutoRun == true)
        {
            AdventureSession.IsAutoRun = false;
            AdBuffManager.Instance.ClearRewardBuff();
        }

        // ✅課金恩恵を考慮して再適用
        ApplyRemoveAdsPermanentBoost();

        RefreshRewardBoostButton();
        RefreshFinalDropText();

        if (TutorialManager.Instance.IsMainMenuSecondCondition())
        {
            mainMenuTutorialPanel2.SetActive(true);
        }
    }

    private void ApplyRemoveAdsPermanentBoost()
    {
       //AdBuffManager.Instance.ClearRewardBuff();

        if (PurchaseState.HasRemoveAds)
        {
            AdBuffManager.Instance.ActivateRewardBuff(2f);
        }
    }

}
