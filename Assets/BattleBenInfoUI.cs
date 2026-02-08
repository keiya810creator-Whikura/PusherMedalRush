using UnityEngine;
using TMPro;

public class BattleBetInfoUI : MonoBehaviour
{
    [Header("UI Text")]
    [SerializeField] private TMP_Text shotRemainText;
    [SerializeField] private TMP_Text damageEstimateText;

    [Header("Warning Settings")]
    [SerializeField] private int warningShotThreshold = 10;

    void Update()
    {
        UpdateShotRemain();
        UpdateDamageEstimate();
    }

    // =========================
    // ✅残り弾数表示
    // =========================
    void UpdateShotRemain()
    {
        // ✅回収タイム中は撃ち放題なので∞表示
        if (WaveManager.Instance.CurrentState == WaveState.Recovery)
        {
            shotRemainText.text = string.Format(TextManager.Instance.GetUI("ui_sentou_4"));

            shotRemainText.color = Color.white;
            return;
        }

        long medal = MoneyManager.instance.Medal;

        // ✅Wave消費
        long baseConsume =
            WaveManager.Instance.GetConsumeMedalPerShot();

        // ✅Bet倍率
        int bet =
            BetManager.Instance.CurrentBet;

        // ✅最終消費
        long finalConsume =
            baseConsume * bet;

        // ✅残り弾数（何発撃てるか）
        long remainingShots =
            (finalConsume > 0) ? medal / finalConsume : 0;

        shotRemainText.text =
            string.Format(TextManager.Instance.GetUI("ui_sentou_2"), remainingShots);

        // ✅残弾が少ないと赤くする
        if (remainingShots <= warningShotThreshold)
        {
            shotRemainText.color = Color.red;
        }
        else
        {
            shotRemainText.color = Color.white;
        }
    }


    // =========================
    // ✅与ダメージ目安表示
    // =========================
    void UpdateDamageEstimate()
    {
        var status = BattleManager.Instance.Status;

        // 基本攻撃力
        float baseDamage = status.Attack;

        // Bet倍率
        int bet = BetManager.Instance.CurrentBet;

        // 目安（乱数やクリティカルは平均化して省略）
        float estimateDamage = baseDamage * bet;

        damageEstimateText.text =
            string.Format(TextManager.Instance.GetUI("ui_sentou_1"),estimateDamage);

    }
}
