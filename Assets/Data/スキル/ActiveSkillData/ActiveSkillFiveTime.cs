using UnityEngine;
using System.Collections;
[CreateAssetMenu(menuName = "Game/ActiveSkill/ActiveSkillFiveTime")]

public class ActiveSkillFiveTime : ActiveSkillData
{
    public float interval = 0.2f; // î≠ê∂ä‘äu

    public override void Execute()
    {
        if (effectPrefab == null) return;

        CoroutineRunner.Instance.StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        Vector3 startPos = MedalGenerator.Instance.transform.position;

        for (int a = 0; a < amount; a++)
        {
            Instantiate(effectPrefab, startPos, Quaternion.identity);

            yield return new WaitForSeconds(interval);
        }
    }
}