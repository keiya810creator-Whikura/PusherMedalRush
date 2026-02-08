using UnityEngine;

public class SyougouDropEffect : DropItemEffect
{
    private SyougouData syougou;

    public void Init(SyougouData data, Color rarityColor, Vector3 target)
    {
        syougou = data;
        base.Init(rarityColor, target);
    }
}
