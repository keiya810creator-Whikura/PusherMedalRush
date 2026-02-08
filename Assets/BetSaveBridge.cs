using System.Collections;
using UnityEngine;

public class BetSaveBridge : MonoBehaviour
{
    private IEnumerator Start()
    {
        yield return null;
        LoadBet();
    }

    // ✅セーブ
    public void SaveBet(int bet)
    {
        var data = SaveManager.Instance.Data;

        data.currentBet = bet;
        SaveManager.Instance.SaveToDisk();

        //Debug.Log($"✅Bet Save: {bet}");
    }

    // ✅ロード
    public void LoadBet()
    {
        var data = SaveManager.Instance.Data;

        BetManager.Instance.SetBet(data.currentBet);

        //Debug.Log($"✅Bet Load: {data.currentBet}");
    }
}
