using UnityEngine;

public class SoubiDropEffect : DropItemEffect
{
    private SoubiData soubi;

    public void Init(SoubiData data, Color rarityColor, Vector3 target)
    {
        soubi = data;
        base.Init(rarityColor, target);
    }
}
