using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class UniversalMonsterAI : MonoBehaviour
{
    [Header("Combat & Movement")]
    public int meleeDamage = 1;
    public float moveSpeed = 2f;
    public float aggroRange = 6f;
    public float attackRange = 1.5f;
    public float attackCooldown = 1.2f;
    public float attackDamageTime = 0.35f;
    public float attackAnimTime = 0.6f;

    [Header("Downed Retreat")]
    public float retreatSpeedMultiplier = 0.5f;

    [Header("Animator Parameters")]
    public string paramHorizontal = "Horizontal";
    public string paramVertical = "Vertical";
    public string paramIsMoving = "IsMoving";
    public string paramIsAttacking = "IsAttacking";

    private Transform player;
    private Rigidbody2D rb;
    private Animator anim;
    private EnemyHealth health;

    private Vector2 lastDir = Vector2.down;
    private bool isAttacking = false;
    private float lastAttackTime = 0;

    void Awake()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null)
            player = p.transform;
        else
            Debug.LogWarning("UniversalMonsterAI: Player not found!");

        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        health = GetComponentInChildren<EnemyHealth>();

        rb.gravityScale = 0;
        rb.freezeRotation = true;
    }

    void Update()
    {
        if (player == null || anim == null) return;

        if (health != null && health.IsCurrentlyDowned())
        {
            Retreat();
            UpdateAnimator();
            return;
        }

        float dist = Vector2.Distance(transform.position, player.position);

        if (isAttacking)
        {
            rb.linearVelocity = Vector2.zero;
        }
        else if (dist <= attackRange)
        {
            rb.linearVelocity = Vector2.zero;
            TryAttack();
        }
        else if (dist <= aggroRange)
        {
            MoveTowardPlayer();
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }

        UpdateAnimator();
    }

    void MoveTowardPlayer()
    {
        Vector2 dir = (player.position - transform.position).normalized;
        rb.linearVelocity = dir * moveSpeed;
    }

    void Retreat()
    {
        Vector2 dir = (transform.position - player.position).normalized;
        rb.linearVelocity = dir * (moveSpeed * retreatSpeedMultiplier);
    }

    void TryAttack()
    {
        if (Time.time < lastAttackTime + attackCooldown)
            return;

        lastAttackTime = Time.time;
        isAttacking = true;

        Invoke(nameof(ApplyDamage), attackDamageTime);
        Invoke(nameof(EndAttack), attackAnimTime);
    }

    void ApplyDamage()
    {
        float dist = Vector2.Distance(transform.position, player.position);
        if (dist <= attackRange + 0.4f)
        {
            PlayerHealth ph = player.GetComponent<PlayerHealth>();
            if (ph != null)
                ph.TakeDamage(meleeDamage);
        }
    }

    void EndAttack()
    {
        isAttacking = false;
    }

    void UpdateAnimator()
    {
        Vector2 vel = rb.linearVelocity;

        // Direction
        if (vel.sqrMagnitude > 0.01f && !isAttacking)
            lastDir = vel.normalized;

        anim.SetFloat(paramHorizontal, lastDir.x);
        anim.SetFloat(paramVertical, lastDir.y);

        // Test if moving
        bool moving = vel.sqrMagnitude > 0.01f && !isAttacking;

        anim.SetBool(paramIsMoving, moving);
        anim.SetBool(paramIsAttacking, isAttacking);
    }
}
