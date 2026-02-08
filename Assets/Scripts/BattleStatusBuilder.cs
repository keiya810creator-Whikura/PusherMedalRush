using UnityEngine;
public static class BattleStatusBuilder
{
    public static BattleStatus Build()
    {
        var db = PlayerStatusDatabase.Instance;

        return new BattleStatus
        {
            // =========================
            // 基本ステータス
            // =========================
            MaxMedal = Mathf.RoundToInt(
                db.GetFinalStatusValue(
                    PlayerStatusDatabase.StatusType.MaxMedal)),

            Attack = Mathf.RoundToInt(
                db.GetFinalStatusValue(
                    PlayerStatusDatabase.StatusType.Attack)),

            // =========================
            // 戦闘系
            // =========================
            CriticalRate =
                db.GetFinalStatusValue(
                    PlayerStatusDatabase.StatusType.CriticalRate),

            CriticalDamageRate =
                db.GetFinalStatusValue(
                    PlayerStatusDatabase.StatusType.CriticalDamageRate),

            ShotCount = Mathf.RoundToInt(
                db.GetFinalStatusValue(
                    PlayerStatusDatabase.StatusType.ShotCount)),

            // =========================
            // 回収Wave
            // =========================
            RecoveryWaveExtendSeconds =
                db.GetFinalStatusValue(
                    PlayerStatusDatabase.StatusType.RecoveryWaveExtendSeconds),

            RecoveryWaveMedalValue = Mathf.RoundToInt(
                db.GetFinalStatusValue(
                    PlayerStatusDatabase.StatusType.RecoveryWaveMedalValue)),

            MedalConsumeZeroRate =
                db.GetFinalStatusValue(
                    PlayerStatusDatabase.StatusType.MedalConsumeZeroRate),

            // =========================
            // 攻撃メダル倍率
            // =========================
            Attack2xMedalRate =
                db.GetFinalStatusValue(
                    PlayerStatusDatabase.StatusType.Attack2xMedalRate),

            Attack5xMedalRate =
                db.GetFinalStatusValue(
                    PlayerStatusDatabase.StatusType.Attack5xMedalRate),

            Attack10xMedalRate =
                db.GetFinalStatusValue(
                    PlayerStatusDatabase.StatusType.Attack10xMedalRate),

            // =========================
            // ドロップ系（広告バフ込み）
            // =========================
            GoldDropRate =
                db.GetFinalStatusValue(
                    PlayerStatusDatabase.StatusType.GoldDropRate)
                * AdBuffManager.Instance.GoldDropMultiplier,

            EquipDropRate =
                db.GetFinalStatusValue(
                    PlayerStatusDatabase.StatusType.EquipDropRate)
                * AdBuffManager.Instance.EquipDropMultiplier,

            TitleDropRate =
    db.GetFinalStatusValue(
        PlayerStatusDatabase.StatusType.TitleDropRate)

    // ✅広告バフ（2倍）
    * AdBuffManager.Instance.TitleDropMultiplier

    // ✅称号課金ブースト（1.5倍）
    * (PurchaseState.HasTitleBoost ? 1.5f : 1f),
        };
    }
}
