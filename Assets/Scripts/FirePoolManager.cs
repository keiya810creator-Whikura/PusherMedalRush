using UnityEngine;
using System.Collections.Generic;

public class FirePoolManager : MonoBehaviour
{
    public static FirePoolManager Instance;

    [Header("Prefabs")]
    public GameObject fireNormal;
    public GameObject fire2x;
    public GameObject fire5x;
    public GameObject fire10x;

    [Header("Initial Pool Size")]
    public int normalCount = 60;
    public int count2x = 40;
    public int count5x = 30;
    public int count10x = 20;

    private Queue<GameObject> normalPool = new();
    private Queue<GameObject> pool2x = new();
    private Queue<GameObject> pool5x = new();
    private Queue<GameObject> pool10x = new();

    private void Awake()
    {
        Instance = this;

        CreatePool(fireNormal, normalCount, normalPool, FireType.Normal);
        CreatePool(fire2x, count2x, pool2x, FireType.X2);
        CreatePool(fire5x, count5x, pool5x, FireType.X5);
        CreatePool(fire10x, count10x, pool10x, FireType.X10);

        Debug.Log("✅ Fire Pool Initialized");
    }

    private void CreatePool(GameObject prefab, int count, Queue<GameObject> pool, FireType type)
    {
        if (prefab == null) return;

        for (int i = 0; i < count; i++)
        {
            var obj = Instantiate(prefab, transform);
            obj.SetActive(false);

            // type 付与（未付与なら付ける）
            var info = obj.GetComponent<FirePoolObject>();
            if (info == null) info = obj.AddComponent<FirePoolObject>();
            info.type = type;

            // 自動Return（未付与なら付ける）

            pool.Enqueue(obj);
        }
    }

    public GameObject Play(FireType type, Vector3 pos, Quaternion rot)
    {
        var obj = Get(type);
        obj.transform.SetPositionAndRotation(pos, rot);
        return obj;
    }

    public GameObject Get(FireType type)
    {
        var pool = GetQueue(type);
        int safety = pool.Count;

        while (safety-- > 0)
        {
            var obj = pool.Dequeue();

            if (!obj.activeSelf)
            {
                obj.SetActive(true);
                return obj;
            }

            pool.Enqueue(obj);
        }

        Debug.LogWarning($"⚠ Fire {type} Pool All Active → Expand!");
        Expand(type, 10);
        return Get(type);
    }

    public void Return(GameObject obj)
    {
        if (obj == null) return;

        var info = obj.GetComponent<FirePoolObject>();
        if (info == null)
        {
            Destroy(obj);
            return;
        }

        if (!obj.activeSelf) return;

        obj.SetActive(false);
        GetQueue(info.type).Enqueue(obj);
    }

    private void Expand(FireType type, int addCount)
    {
        var prefab = GetPrefab(type);
        var pool = GetQueue(type);
        CreatePool(prefab, addCount, pool, type);
    }

    private Queue<GameObject> GetQueue(FireType type)
    {
        return type switch
        {
            FireType.Normal => normalPool,
            FireType.X2 => pool2x,
            FireType.X5 => pool5x,
            FireType.X10 => pool10x,
            _ => normalPool
        };
    }

    private GameObject GetPrefab(FireType type)
    {
        return type switch
        {
            FireType.Normal => fireNormal,
            FireType.X2 => fire2x,
            FireType.X5 => fire5x,
            FireType.X10 => fire10x,
            _ => fireNormal
        };
    }
}