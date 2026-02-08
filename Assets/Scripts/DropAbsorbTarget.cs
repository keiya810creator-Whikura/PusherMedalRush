using UnityEngine;

public class DropAbsorbTarget : MonoBehaviour
{
    public static DropAbsorbTarget Instance;

    private void Awake()
    {
        Instance = this;
    }

    public Vector3 WorldPosition => transform.position;
}
