using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class AbilityMushroomAI : MonoBehaviour
{
    [Header("Animation Prefix (IMPORTANT)")]
    public string animPrefix = "Mushroom_Party";
    // or "Plant_Seeder"

    [Header("Movement")]
    public bool movesTowardPlayer = true;
    public float moveSpeed = 1.4f;
    public float aggroRange = 5f;

    [Header("Melee Attack")]
    public bool hasMeleeAttack = false;
    public float attackRange = 1.25f;
    public float attackCooldown = 1.4f;
    public int meleeDamage = 1;
    public float attackWindup = 0.25f;
    public float attackAnimTime = 0.6f;

    [Header("AOE Ability")]
    public bool hasAbility = true;
    public float abilityCooldown = 6f;
    public float abilityRadius = 2.8f;
    public int abilityDamage = 2;
    public float abilityWindup = 0.8f;
    public float abilityAnimTime = 1.4f;

    Transform player;
    Rigidbody2D rb;
    Animator anim;

    float lastAttackTime = 0f;
    float lastAbilityTime = 0f;
    bool isAttacking = false;
    bool isAbility = false;

    Vector2 lastDir = Vector2.down;
    EnemyHealth health;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        health = GetComponentInChildren<EnemyHealth>();
        rb.gravityScale = 0;
        rb.freezeRotation = true;
    }

    void Update()
    {
        if (player == null) return;

        if (health != null && health.IsCurrentlyDowned())
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (isAttacking || isAbility)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        float dist = Vector2.Distance(transform.position, player.position);

        // Try ability FIRST
        if (hasAbility && Time.time >= lastAbilityTime + abilityCooldown)
        {
            TriggerAbility();
            return;
        }

        // Try normal melee attack
        if (hasMeleeAttack && dist <= attackRange)
        {
            TriggerMelee();
            return;
        }

        if (movesTowardPlayer && dist <= aggroRange)
            MoveTowardPlayer();
        else
            rb.linearVelocity = Vector2.zero;

        UpdateAnim();
    }

    void MoveTowardPlayer()
    {
        Vector2 dir = (player.position - transform.position).normalized;
        lastDir = dir;
        rb.linearVelocity = dir * moveSpeed;
    }

    // =======================
    // MELEE ATTACK
    // =======================
    void TriggerMelee()
    {
        if (Time.time < lastAttackTime + attackCooldown)
            return;

        lastAttackTime = Time.time;
        isAttacking = true;

        anim.Play(Anim("Attack"));
        Invoke(nameof(DoMeleeDamage), attackWindup);
        Invoke(nameof(FinishMelee), attackAnimTime);
    }

    void DoMeleeDamage()
    {
        if (Vector2.Distance(transform.position, player.position) <= attackRange + 0.3f)
            player.GetComponent<PlayerHealth>()?.TakeDamage(meleeDamage);
    }

    void FinishMelee()
    {
        isAttacking = false;
    }

    // =======================
    // ABILITY — AOE
    // =======================
    void TriggerAbility()
    {
        lastAbilityTime = Time.time;
        isAbility = true;

        anim.Play(Anim("Ability"));
        Invoke(nameof(DoAOE), abilityWindup);
        Invoke(nameof(FinishAbility), abilityAnimTime);
    }

    void DoAOE()
    {
        // Damage all players in radius
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, abilityRadius);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
                hit.GetComponent<PlayerHealth>()?.TakeDamage(abilityDamage);
        }
    }

    void FinishAbility()
    {
        isAbility = false;
    }

    // =======================
    // ANIMATION HANDLING
    // =======================
    void UpdateAnim()
    {
        if (isAttacking || isAbility) return;

        if (rb.linearVelocity.sqrMagnitude > 0.1f)
            anim.Play(Anim("Move"));
        else
            anim.Play(Anim("Idle"));
    }

    string Anim(string prefix)
    {
        if (Mathf.Abs(lastDir.x) > Mathf.Abs(lastDir.y))
            return lastDir.x > 0 ? $"{animPrefix}_{prefix}_R" : $"{animPrefix}_{prefix}_L";
        else
            return lastDir.y > 0 ? $"{animPrefix}_{prefix}_U" : $"{animPrefix}_{prefix}_D";
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, abilityRadius);
    }
}
