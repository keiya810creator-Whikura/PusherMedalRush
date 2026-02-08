using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Medal : MonoBehaviour
{
    // ✅追加：特殊メダルが回収されたら発動するスキル
    public ActiveSkillData linkedSkill;

    [Header("Physics Materials")]
    public PhysicsMaterial2D fallingMaterial;
    public PhysicsMaterial2D normalMaterial;

    [Header("Gravity Settings")]
    public float fallingGravityScale = 100f;
    public float normalGravityScale = 0f;

    [Header("Damping Settings")]
    public float fallingLinearDamping = 0f;
    public float fallingAngularDamping = 0f;
    public float normalLinearDamping = 5f;
    public float normalAngularDamping = 5f;

    // ✅追加：プッシャーに乗れるか
    public bool CanRidePusher { get; private set; } = false;

    private Rigidbody2D rb;
    private Collider2D col;
    private bool isFalling = true;


    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }

    private void OnEnable()
    {
        SetFallingState();

        // ✅回収スキルをリセット（Pool対策）
        linkedSkill = null;
    }

    void Update()
    {
        if (!isFalling && transform.position.y >= 7.5f)
        {
            MedalPoolManager.Instance.ReturnMedal(gameObject);
        }
    }

    void SetFallingState()
    {
        isFalling = true;
        CanRidePusher = false;

        gameObject.layer = LayerMask.NameToLayer("Medal_Falling");
        col.sharedMaterial = fallingMaterial;

        rb.gravityScale = fallingGravityScale;
        rb.linearDamping = fallingLinearDamping;
        rb.angularDamping = fallingAngularDamping;

   }

    void SetNormalState()
    {
        isFalling = false;

        gameObject.layer = LayerMask.NameToLayer("Medal_Normal");
        col.sharedMaterial = normalMaterial;

        rb.gravityScale = normalGravityScale;
        rb.linearDamping = normalLinearDamping;
        rb.angularDamping = normalAngularDamping;

        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;

        CanRidePusher = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!isFalling) return;

        if (other.CompareTag("UpperWallTrigger"))
        {
            SetNormalState();
        }
    }
}