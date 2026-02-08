using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TitleSlotUI : MonoBehaviour
{
    [SerializeField] Image icon;
    [SerializeField] TMP_Text nameText;

    public void Set(SyougouData title)
    {
        icon.sprite = title.icon;
        nameText.text = title.nameKey;
    }
}
