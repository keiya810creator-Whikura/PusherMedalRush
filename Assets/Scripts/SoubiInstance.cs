using System.Collections.Generic;

[System.Serializable]
public class SoubiInstance
{
    public int instanceId;
    public SoubiData master;

    public bool isEquipped; // š’Ç‰Á
    public EquipSlotId? equippedSlotId;
    public List<SyougouData> attachedTitles = new();
    public bool isFavorite;
    // š •ª‰ğ‘I‘ğ’†ƒtƒ‰ƒO
    //public bool isMarkedForDisassemble;
    public bool isSelectedForDismantle;

    public IReadOnlyList<SyougouData> GetAttachedTitles()
    {
        return attachedTitles;
    }

}

