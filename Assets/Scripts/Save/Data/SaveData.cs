using System;
using System.Collections.Generic;

[Serializable]
public class SaveData
{
    //戦闘設定
    public int startWave = 1;
    public int endWave = 10;
    // ✅追加：終了WaveがMaxだったか
    public bool isEndWaveMax = false;
    public int highestClearedWave = 0;
    public long gold;

    // Inventory
    public int nextInstanceId;
    public List<SoubiInstanceSaveData> soubiInstances = new();
    public List<SyougouStackSaveData> syougouStacks = new();

    // 図鑑（すでにやってるフラグもここに統合可能）
    public List<string> seenMonsterIds = new();
    public List<string> foundSoubiIds = new();
    public List<string> foundTitleIds = new();

    public List<StatusUpgradeSaveData> statusUpgrades = new();

    //自動分解
    public bool[] autoDisassembleRarity = new bool[7];

    //回収ウェーブ時間
    public float recoverySeconds = 30f;
    
    //ゲームスピード
    public float gameSpeedRate = 1f;

    //図鑑フラグ
    public List<string> encounteredMonsters = new();
    public List<string> obtainedEquipments = new();
    public List<string> obtainedTitles = new();

    // 撤退タイム（秒）
    public int tettaiTime = 0;

    //ベット倍率
    public int currentBet = 1;

    // =========================
    // 課金状態（IAP）
    // =========================

    public bool hasRemoveAds = false;       // 広告削除購入済み
    public bool hasSpeed3x = false;         // 3倍速購入済み
    public bool hasTitleDropBoost = false;  // 称号ドロップ1.5倍購入済み

    // 倉庫拡張（初期300）
    public int storageSize = 300;
    public int storageExpandCount = 0;
    public List<string> processedPurchases = new();
}

[Serializable]
public class SoubiInstanceSaveData
{
    public int instanceId;

    // master SO参照 → idで保存
    public string masterId;

    // 状態
    public bool isEquipped;
    public int equippedSlotId; // nullは-1で保存
    public bool isFavorite;

    // 付与称号 SO参照 → idで保存
    public List<string> attachedTitleIds = new();
}

[Serializable]
public class SyougouStackSaveData
{
    public string titleId;
    public int count;
}
[System.Serializable]
public class StatusUpgradeSaveData
{
    public int statusType;   // enumをintで保存
    public int upgradeCount;
}

