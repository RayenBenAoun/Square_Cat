using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class AbilityMushroomAI : MonoBehaviour
{
    [Header("Animation Tag")]
    public string animPrefix = "Plant_Seeder";
    // Set per enemy:
    // Mushroom_Party
    // Plant_Seeder

    [Header("Role Type")]
    public bool movesTowardPlayer = false;
    // Plant Seeder = FALSE
    // Mushroom Party = TRUE or FALSE depending on desired behavior

    [Header("Common Stats")]
    public float moveSpeed = 1.4f;
    public float aggroRange = 5f;
    public float attackRange = 1.4f;
    public float attackCooldown = 1.5f;
    public float attackWindup = 0.35f;
    public float attackAnimTime = 0.7f;
    public int meleeDamage = 1;

    [Header("Ability Settings")]
    public bool hasAbility = true;
    public float abilityCooldown = 7f;
    public float abilityWindup = 0.5f;
    public float abilityAnimTime = 1.2f;

    // Plant Seeder: projectile or minions
    public GameObject abilityProjectilePrefab;
    public GameObject abilitySpawnPrefab;
    public int spawnCount = 0; // if > 0, it will spawn

    // Mushroom Party: Might do AoE  
    public float AoERadius = 2f;
    public int AoEDamage = 1;

    Transform player;
    Rigidbody2D rb;
    Animator anim;
    EnemyHealth health;

    Vector2 lastDir = Vector2.down;
    bool isAttacking = false;
    bool isAbility = false;

    float lastAttackTime = 0f;
    float lastAbilityTime = 0f;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        health = GetComponentInChildren<EnemyHealth>();

        rb.gravityScale = 0;
        rb.freezeRotation = true;
    }

    void Update()
    {
        if (player == null) return;

        float dist = Vector2.Distance(transform.position, player.position);

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

        if (hasAbility && Time.time >= lastAbilityTime + abilityCooldown)
        {
            StartAbility();
            return;
        }

        if (dist <= attackRange)
        {
            TryAttack();
            rb.linearVelocity = Vector2.zero;
        }
        else if (movesTowardPlayer && dist <= aggroRange)
        {
            MoveTowardPlayer();
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }

        UpdateAnim();
    }

    void MoveTowardPlayer()
    {
        Vector2 dir = (player.position - transform.position).normalized;
        lastDir = dir;
        rb.linearVelocity = dir * moveSpeed;
    }

    void TryAttack()
    {
        if (Time.time < lastAttackTime + attackCooldown)
            return;

        lastAttackTime = Time.time;
        isAttacking = true;

        anim.Play(Anim("Attack"));

        Invoke(nameof(DealMeleeDamage), attackWindup);
        Invoke(nameof(StopAttack), attackAnimTime);
    }

    void DealMeleeDamage()
    {
        if (Vector2.Distance(transform.position, player.position) <= attackRange + 0.4f)
            player.GetComponent<PlayerHealth>()?.TakeDamage(meleeDamage);
    }

    void StopAttack()
    {
        isAttacking = false;
    }

    void StartAbility()
    {
        lastAbilityTime = Time.time;
        isAbility = true;

        anim.Play(Anim("Ability"));
        Invoke(nameof(DoAbility), abilityWindup);
        Invoke(nameof(StopAbility), abilityAnimTime);
    }

    void DoAbility()
    {
        // ░░ PLANT SEEDER BEHAVIOR ░░
        if (abilityProjectilePrefab != null)
        {
            Vector2 dir = (player.position - transform.position).normalized;
            GameObject proj = Instantiate(abilityProjectilePrefab, transform.position, Quaternion.identity);
            proj.transform.right = dir;
            proj.GetComponent<Rigidbody2D>().linearVelocity = dir * 7f;
            return;
        }

        if (spawnCount > 0 && abilitySpawnPrefab != null)
        {
            for (int i = 0; i < spawnCount; i++)
            {
                Vector2 offset = Random.insideUnitCircle * 1.4f;
                Instantiate(abilitySpawnPrefab, transform.position + (Vector3)offset, Quaternion.identity);
            }
            return;
        }

        // ░░ MUSHROOM PARTY AoE ░░
        Collider2D[] hit = Physics2D.OverlapCircleAll(transform.position, AoERadius);
        foreach (var h in hit)
        {
            if (h.CompareTag("Player"))
                h.GetComponent<PlayerHealth>()?.TakeDamage(AoEDamage);
        }
    }

    void StopAbility()
    {
        isAbility = false;
    }

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
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, AoERadius);
    }
}
