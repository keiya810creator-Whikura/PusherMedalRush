using UnityEngine;

public class OverLayRoot : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true);
        }
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
    }

}
