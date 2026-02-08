using UnityEngine;

public class MedalMultiplier : MonoBehaviour
{
    public int multiplier = 1; // 1, 2, 5, 10

    public bool isCollected = false;
    private void OnEnable()
    {
        isCollected = false;
    }
}
