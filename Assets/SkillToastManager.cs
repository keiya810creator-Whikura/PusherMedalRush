using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SkillToastManager : MonoBehaviour
{
    public static SkillToastManager Instance;

    [SerializeField] private SkillToastUIController toastPrefab;
    [SerializeField] private Transform toastParent;

    private Queue<SkillToastRequest> queue = new();
    private bool showing = false;
    [SerializeField] private int maxSimultaneous = 3;

    private List<SkillToastUIController> activeToasts = new();

    void Awake()
    {
        Instance = this;
    }

    // =========================
    // ✅スキル用リクエスト構造
    // =========================
    public class SkillToastRequest
    {
        public string message;
        public float minDuration;
        public System.Func<bool> closeCondition;

        public SkillToastRequest(string msg, float minTime, System.Func<bool> condition)
        {
            message = msg;
            minDuration = minTime;
            closeCondition = condition;
        }
    }

    // =========================
    // ✅外部呼び出し
    // =========================
    public void ShowSkillToast(string msg, float minTime, System.Func<bool> closeCondition)
    {
        StartCoroutine(ShowToastRoutine(msg, minTime, closeCondition));
    }
    IEnumerator ShowToastRoutine(string msg, float minTime, System.Func<bool> closeCondition)
    {
        // ✅上限超えたら一番古いのを閉じる
        if (activeToasts.Count >= maxSimultaneous)
        {
            var oldest = activeToasts[0];
            activeToasts.RemoveAt(0);
            oldest.AllowClose();
            yield return oldest.CloseRoutine();
        }

        SkillToastUIController toast =
            Instantiate(toastPrefab, toastParent);

        activeToasts.Add(toast);
        UpdateToastPositions();

        toast.Show(msg);

        yield return new WaitForSeconds(minTime);

        while (closeCondition != null && !closeCondition())
            yield return null;

        toast.AllowClose();
        yield return toast.CloseRoutine();

        activeToasts.Remove(toast);
        UpdateToastPositions();
    }
    private void UpdateToastPositions()
    {
        float yOffset = 60f;

        for (int i = 0; i < activeToasts.Count; i++)
        {
            RectTransform rt = activeToasts[i].GetComponent<RectTransform>();
            if (rt == null) continue;

            rt.anchoredPosition = new Vector2(0, -i * yOffset);
        }
    }


    // =========================
    // ✅処理
    // =========================
    IEnumerator ProcessQueue()
    {
        showing = true;

        while (queue.Count > 0)
        {
            var req = queue.Dequeue();

            SkillToastUIController toast =
                Instantiate(toastPrefab, toastParent);

            toast.Show(req.message);

            // ✅最低表示時間は保証
            yield return new WaitForSeconds(req.minDuration);

            // ✅条件が満たされるまで待つ
            while (req.closeCondition != null && !req.closeCondition())
                yield return null;

            // ✅閉じる許可
            toast.AllowClose();
            yield return toast.CloseRoutine();
        }

        showing = false;
    }
    public SkillToastUIController ShowSkillToastManual(string msg)
    {
        SkillToastUIController toast =
            Instantiate(toastPrefab, toastParent);

        toast.Show(msg);
        return toast;
    }
}