using UnityEngine;

[CreateAssetMenu(menuName = "Game/ActiveSkill/ActiveSkillFromGenerator")]
public class ActiveSkillFromGenerator : ActiveSkillData
{
    public override void Execute()
    {
        if (effectPrefab == null) return;

        // ✅プレイヤー＝MedalGenerator位置から生成
        Vector3 startPos = MedalGenerator.Instance.transform.position;

        for (int a = 0; a <= amount; a++)
        {
            GameObject obj =
                Instantiate(effectPrefab, startPos, Quaternion.identity);
        }

        //Debug.Log("🌪 アクティブスキル 発動！");
    }
}