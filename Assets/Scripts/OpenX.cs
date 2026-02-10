using UnityEngine;

public class OpenX : MonoBehaviour
{
    // ŒÅ’èƒŠƒ“ƒNiInspector‚Å•Ï‚¦‚½‚¢‚È‚ç[SerializeField]‚É‚µ‚Ä‚àOKj
    private const string Url = "https://x.com/Fireball_srash";

    // Button‚ÌOnClick‚É“o˜^‚·‚é
    public void Open()
    {
        Application.OpenURL(Url);
    }
}
