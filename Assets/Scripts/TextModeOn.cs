using UnityEngine;

public class TextModeOn : MonoBehaviour
{
    public int count=0;
    public void TestModeOn()
    {
        count++;
        if(count>=5)
        {
            TestManager.Instance.textMode = true;
        }
    }
}
