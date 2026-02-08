using UnityEngine;

public class ActiveSkillData : ScriptableObject
{
    [Header("Skill Info")]
    public string skillNameKey;

    [Header("Prefab")]
    public GameObject effectPrefab;

    [Header("発射数")]
    public int amount;
    // ✅子クラスで上書きできるようにする
    public virtual void Execute()
    {
        Debug.Log($"🔥 ActiveSkill Execute: {skillNameKey}");

        if (effectPrefab != null)
        {
            GameObject.Instantiate(
                effectPrefab,
                Vector3.zero,
                Quaternion.identity
            );
        }
    }
}
public enum ActiveSkillSpawnType
{
    FromPlayer,
    AboveEnemy,
    RandomEnemy
}