using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraAspectFix : MonoBehaviour
{
    // 基準：iPhone 11〜15（19.5:9）
    public float baseAspect = 9f / 19.5f;

    // 19.5:9 時の基準サイズ
    public float baseOrthoSize = 11.7f;

    Camera cam;

    void Awake()
    {
        cam = GetComponent<Camera>();
        Apply();
    }

    void OnEnable()
    {
        Apply();
    }

    void Apply()
    {
        float screenAspect = (float)Screen.width / Screen.height;

        if (screenAspect > baseAspect)
        {
            // 横が広い端末（16:9）
            cam.orthographicSize = baseOrthoSize;
        }
        else
        {
            // 縦が長い端末
            cam.orthographicSize = baseOrthoSize * (baseAspect / screenAspect);
        }
    }
}
