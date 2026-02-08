using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class ToastManager : MonoBehaviour
{
    public static ToastManager Instance;

    [SerializeField] private ToastUI toastPrefab;
    [SerializeField] private RectTransform toastParent;

    private Queue<string> queue = new();
    private bool showing = false;

    void Awake()
    {
        Instance = this;


    }

    // =========================
    // ✅外部から呼ぶ
    // =========================
    public void ShowToast(string msg)
    {
        queue.Enqueue(msg);

        if (!showing)
            StartCoroutine(ProcessQueue());
    }

    IEnumerator ProcessQueue()
    {
        showing = true;

        while (queue.Count > 0)
        {
            string msg = queue.Dequeue();

            ToastUI toast =
    Instantiate(toastPrefab, toastParent, false);


            toast.Show(msg);

            // ✅次まで少し待つ
            yield return new WaitForSeconds(1.0f);
        }

        showing = false;
    }
}
