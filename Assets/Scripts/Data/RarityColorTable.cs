using UnityEngine;

[System.Serializable]
public class RarityColor
{
    public int rarity;
    public Color color;
}

[CreateAssetMenu(menuName = "Game/Rarity Color Table")]
public class RarityColorTable : ScriptableObject
{
    public RarityColor[] colors;

    public Color GetColor(int rarity)
    {
        foreach (var r in colors)
        {
            if (r.rarity == rarity)
                return r.color;
        }
        return Color.white;
    }
}
