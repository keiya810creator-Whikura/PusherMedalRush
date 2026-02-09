using UnityEngine;
using System.Linq;   // ★これを追加

public class DamageTextManager : MonoBehaviour
{
    public static DamageTextManager Instance;

    [SerializeField] private DamageText damageTextPrefab;
    [SerializeField] private Canvas worldCanvas;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    // ★ isCritical を受け取る
    public void Spawn(
    long damage,
    Vector3 worldPos,
    bool isCritical,
    bool isSkillHit,
    int medalMultiplier
)
    {
        DamageText text = Instantiate(damageTextPrefab, worldCanvas.transform);

        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);

        text.Set(damage, screenPos, isCritical, isSkillHit, medalMultiplier);
    }

}
