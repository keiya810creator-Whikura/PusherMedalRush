using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

public class StatRow : MonoBehaviour,
    IPointerDownHandler,
    IPointerUpHandler,
    IPointerExitHandler

{
    [Header("Status")]
    public PlayerStatusDatabase.StatusType statusType;

    [Header("UI")]
    [SerializeField] private TMP_Text statusNameText;
    [SerializeField] private TMP_Text valueText;
    [SerializeField] private TMP_Text costText;
    [SerializeField] private Button upgradeButton;

    [Header("Button Color")]
    [SerializeField] private Image upgradeButtonImage;
    [SerializeField] private Color canUpgradeColor = Color.white;
    [SerializeField] private Color cannotUpgradeColor = new Color(0.6f, 0.2f, 0.2f);

    private bool isHolding = false;
    private Coroutine holdRoutine;

    [SerializeField] private float holdStartDelay = 0.4f; // 長押し開始まで
    [SerializeField] private float holdInterval = 0.05f;   // 連打速度

    private void Start()
    {
        var longPress = upgradeButton.GetComponent<LongPressButton>();

        longPress.OnClickOnce = () =>
        {
            TryUpgradeOnce();
        };

        longPress.OnHoldRepeat = () =>
        {
            TryUpgradeOnce();
        };
    }



    public void Refresh()
    {
        statusNameText.text = GetStatusDisplayName(statusType);

        float value =
            PlayerStatusDatabase.Instance.GetFinalStatusValue(statusType);

        valueText.text = FormatStatusValue(statusType, value);

        int count =
            PlayerStatusDatabase.Instance.GetUpgradeCount(statusType);

        int max =
            StatusUpgradeConfig.GetMaxUpgradeCount(statusType);

        bool isMax =
            StatusUpgradeConfig.IsMax(statusType, count);

        // ✅上限なら表示をMAXにする
        if (isMax)
        {
            costText.text = "MAX";
            upgradeButton.interactable = false;
            upgradeButtonImage.color = cannotUpgradeColor;
            return;
        }

        // ✅通常時
        int cost = StatusUpgradeConfig.GetUpgradeCost(statusType, count);
        costText.text = cost.ToString("N0") + "G";

        bool canUpgrade =
            MoneyManager.instance.Gold >= cost;

        upgradeButton.interactable = canUpgrade;
        upgradeButtonImage.color =
            canUpgrade ? canUpgradeColor : cannotUpgradeColor;
    }
    private bool TryUpgradeOnce()
    {
        int count =
            PlayerStatusDatabase.Instance.GetUpgradeCount(statusType);

        // ✅上限なら停止
        if (StatusUpgradeConfig.IsMax(statusType, count))
            return false;

        int cost =
            StatusUpgradeConfig.GetUpgradeCost(statusType, count);

        if (!MoneyManager.instance.TrySpendGold(cost))
            return false;

        PlayerStatusDatabase.Instance.AddStatusValue(
            statusType,
            StatusUpgradeConfig.GetUpgradeValue(statusType)
        );

        return true;
    }





    // ===============================
    // 仮：強化値・コスト
    // ===============================

    private float GetUpgradeValue()
    {
        return StatusUpgradeConfig.GetUpgradeValue(statusType);
    }

    private int GetUpgradeCost()
    {
        int count =
            PlayerStatusDatabase.Instance.GetUpgradeCount(statusType);

        return StatusUpgradeConfig.GetUpgradeCost(statusType, count);
    }

    private string GetStatusDisplayName(
    PlayerStatusDatabase.StatusType type)
    {
        switch (type)
        {
            case PlayerStatusDatabase.StatusType.MaxMedal:
                return string.Format(
        TextManager.Instance.GetUI("ui_mainmenu_5_1")        
    );

            case PlayerStatusDatabase.StatusType.Attack:
                return string.Format(
        TextManager.Instance.GetUI("ui_mainmenu_5_2")
    );

            case PlayerStatusDatabase.StatusType.CriticalRate:
                return string.Format(
        TextManager.Instance.GetUI("ui_mainmenu_5_3")
    );

            case PlayerStatusDatabase.StatusType.CriticalDamageRate:
                return string.Format(
        TextManager.Instance.GetUI("ui_mainmenu_5_4")
    );

            case PlayerStatusDatabase.StatusType.ShotCount:
                return string.Format(
        TextManager.Instance.GetUI("ui_mainmenu_5_5")
    );

            case PlayerStatusDatabase.StatusType.RecoveryWaveExtendSeconds:
                return string.Format(
        TextManager.Instance.GetUI("ui_mainmenu_5_6")
    );

            case PlayerStatusDatabase.StatusType.RecoveryWaveMedalValue:
                return string.Format(
        TextManager.Instance.GetUI("ui_mainmenu_5_7")
    );

            case PlayerStatusDatabase.StatusType.MedalConsumeZeroRate:
                return string.Format(
        TextManager.Instance.GetUI("ui_mainmenu_5_8")
    );

            case PlayerStatusDatabase.StatusType.Attack2xMedalRate:
                return string.Format(
        TextManager.Instance.GetUI("ui_mainmenu_5_9")
        );

            case PlayerStatusDatabase.StatusType.Attack5xMedalRate:
                return string.Format(
        TextManager.Instance.GetUI("ui_mainmenu_5_10")
    );

            case PlayerStatusDatabase.StatusType.Attack10xMedalRate:
                return string.Format(
        TextManager.Instance.GetUI("ui_mainmenu_5_11")
    );

            case PlayerStatusDatabase.StatusType.GoldDropRate:
                return string.Format(
        TextManager.Instance.GetUI("ui_mainmenu_5_12")
    );

            case PlayerStatusDatabase.StatusType.EquipDropRate:
                return string.Format(
        TextManager.Instance.GetUI("ui_mainmenu_5_13")
    );

            case PlayerStatusDatabase.StatusType.TitleDropRate:
                return string.Format(
        TextManager.Instance.GetUI("ui_mainmenu_5_14")
    );

            default:
                return type.ToString();
        }
    }
    private void OnEnable()
    {
        if (MoneyManager.instance != null)
            MoneyManager.instance.OnGoldChanged += OnGoldChanged;

        PlayerStatusDatabase.Instance.OnStatusChanged += Refresh;
        Refresh();
    }

    private void OnDisable()
    {
        if (MoneyManager.instance != null)
            MoneyManager.instance.OnGoldChanged -= OnGoldChanged;

        PlayerStatusDatabase.Instance.OnStatusChanged -= Refresh;
    }

    private void OnGoldChanged(long currentGold)
    {
        Refresh();
    }
    private string FormatStatusValue(
    PlayerStatusDatabase.StatusType type,
    float value)
    {
        switch (type)
        {
            // =========================
            // ✅*5 / *10 だけ小数2桁で表示
            // =========================
            case PlayerStatusDatabase.StatusType.Attack5xMedalRate:
            case PlayerStatusDatabase.StatusType.Attack10xMedalRate:
                return (value * 100f).ToString("0.00") + "%";

            // =========================
            // ✅それ以外の確率系は小数1桁
            // =========================
            case PlayerStatusDatabase.StatusType.CriticalRate:
            case PlayerStatusDatabase.StatusType.CriticalDamageRate:
            case PlayerStatusDatabase.StatusType.MedalConsumeZeroRate:
            case PlayerStatusDatabase.StatusType.Attack2xMedalRate:
            case PlayerStatusDatabase.StatusType.EquipDropRate:
            case PlayerStatusDatabase.StatusType.TitleDropRate:
            case PlayerStatusDatabase.StatusType.GoldDropRate:
                return (value * 100f).ToString("0.0") + "%";
            // =========================
            // ✅通常ステータス（整数）
            // =========================
            default:
                return Mathf.RoundToInt(value).ToString("N0");
        }
    }

    public void Init(PlayerStatusDatabase.StatusType type)
    {
        statusType = type;
        Refresh();
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        int count =
            PlayerStatusDatabase.Instance.GetUpgradeCount(statusType);

        // ✅MAXなら長押し開始しない
        if (StatusUpgradeConfig.IsMax(statusType, count))
            return;

        if (!upgradeButton.interactable) return;

        isHolding = true;

        TryUpgradeOnce();

        holdRoutine = StartCoroutine(HoldUpgradeRoutine());
    }


    public void OnPointerUp(PointerEventData eventData)
    {
        StopHold();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StopHold();
    }

    private void StopHold()
    {
        isHolding = false;

        if (holdRoutine != null)
            StopCoroutine(holdRoutine);
    }
    private IEnumerator HoldUpgradeRoutine()
    {
        // ✅ちょっと待ってから高速連打開始
        yield return new WaitForSeconds(holdStartDelay);

        while (isHolding)
        {
            if (!TryUpgradeOnce())
            {
                // ✅上限 or お金不足で停止
                StopHold();
                yield break;
            }

            yield return new WaitForSeconds(holdInterval);
        }
    }


}
