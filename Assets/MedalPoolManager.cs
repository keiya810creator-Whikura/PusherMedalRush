using UnityEngine;
using System.Collections.Generic;

public class MedalPoolManager : MonoBehaviour
{
    public static MedalPoolManager Instance;

    // ✅PrefabごとにPoolを自動生成するDictionary
    private Dictionary<GameObject, Queue<GameObject>> poolTable
        = new Dictionary<GameObject, Queue<GameObject>>();

    [Header("Default Expand Size")]
    public int expandCount = 20;

    void Awake()
    {
        Instance = this;
    }

    // ✅Prefab指定で取得（特殊弾も全部これ）
    public GameObject GetMedal(GameObject prefab)
    {
        if (prefab == null)
        {
            Debug.LogError("❌Prefabがnullです");
            return null;
        }

        // ✅Poolが無ければ作る
        if (!poolTable.ContainsKey(prefab))
        {
            poolTable[prefab] = new Queue<GameObject>();
            ExpandPool(prefab, expandCount);
        }

        Queue<GameObject> pool = poolTable[prefab];

        // ✅使える弾を探す
        int safety = pool.Count;
        while (safety-- > 0)
        {
            GameObject obj = pool.Dequeue();

            if (!obj.activeSelf)
            {
                obj.SetActive(true);
                return obj;
            }

            pool.Enqueue(obj);
        }

        // ✅足りなければ拡張
        ExpandPool(prefab, expandCount);

        return GetMedal(prefab);
    }

    // ✅Return（Prefab別Poolに戻す）
    public void ReturnMedal(GameObject medal)
    {
        if (medal == null) return;

        MedalPoolObject info = medal.GetComponent<MedalPoolObject>();

        // ✅Destroy指定なら消す
        if (info != null && info.destroyOnReturn)
        {
            Destroy(medal);
            return;
        }

        medal.SetActive(false);

        // ✅Prefab情報で戻す
        GameObject prefabKey = info.prefabKey;

        if (prefabKey == null)
        {
            Destroy(medal);
            return;
        }

        poolTable[prefabKey].Enqueue(medal);
    }

    // ✅Pool生成
    void ExpandPool(GameObject prefab, int count)
    {
        Queue<GameObject> pool = poolTable[prefab];

        for (int i = 0; i < count; i++)
        {
            GameObject obj = Instantiate(prefab, transform);
            obj.SetActive(false);

            // ✅PrefabKey登録
            MedalPoolObject info = obj.GetComponent<MedalPoolObject>();
            info.prefabKey = prefab;

            pool.Enqueue(obj);
        }
    }
}