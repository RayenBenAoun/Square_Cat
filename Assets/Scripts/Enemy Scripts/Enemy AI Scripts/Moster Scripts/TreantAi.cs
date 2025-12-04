using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class TreantAI : MonoBehaviour
{
    [Header("Treant Settings")]
    public string treantName = "Plant_Treant";
    public float moveSpeed = 1.3f;
    public float aggroRange = 4.2f;
    public float attackRange = 1.5f;
    public float attackCooldown = 1.3f;
    public int meleeDamage = 1;
    public bool canUseReviveAbility = true;

    [Header("Flash Effect")]
    public SpriteRenderer spriteRenderer;
    public float flashDuration = 0.4f;

    Transform player;
    Rigidbody2D rb;
    Animator anim;
    EnemyHealth health;

    bool isAttacking = false;
    float lastAttackTime = 0f;

    bool hasBeenDownedBefore = false;
    bool usedReviveAbility = false;

    Vector2 lastDir = Vector2.down;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        health = GetComponentInChildren<EnemyHealth>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        rb.freezeRotation = true;
        rb.gravityScale = 0;
    }

    void Update()
    {
        if (player == null) return;

        if (health.IsCurrentlyDowned())
        {
            hasBeenDownedBefore = true;
            return;
        }

        if (hasBeenDownedBefore && !usedReviveAbility && canUseReviveAbility)
        {
            TriggerReviveAbility();
            return;
        }

        if (isAttacking)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        float dist = Vector2.Distance(transform.position, player.position);

        if (dist <= attackRange)
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
            PlayIdle();
        }
    }

    void MoveTowardPlayer()
    {
        Vector2 dir = (player.position - transform.position).normalized;
        lastDir = dir;
        rb.linearVelocity = dir * moveSpeed;

        anim.Play(Anim("Move"));
    }

    void TryAttack()
    {
        if (Time.time < lastAttackTime + attackCooldown) return;

        lastAttackTime = Time.time;
        isAttacking = true;

        anim.Play(Anim("Attack"));

        Invoke(nameof(DoDamage), 0.32f);
        Invoke(nameof(EndAttack), 0.6f);
    }

    void DoDamage()
    {
        if (Vector2.Distance(transform.position, player.position) <= attackRange + 0.3f)
            player.GetComponent<PlayerHealth>()?.TakeDamage(meleeDamage);
    }

    void EndAttack()
    {
        isAttacking = false;
    }

    // ===========================
    // REVIVE ABILITY LOGIC
    // ===========================

    void TriggerReviveAbility()
    {
        usedReviveAbility = true;
        rb.linearVelocity = Vector2.zero;
        isAttacking = true;

        anim.Play(Anim("Ability"));

        Invoke(nameof(ApplyReviveHeal), 0.25f);
        StartCoroutine(FlashWhite());
        Invoke(nameof(EndAttack), 0.8f);
    }

    void ApplyReviveHeal()
    {
        health.Health = Mathf.Min(health.Health + 2, health.MaxHealth);
    }

    System.Collections.IEnumerator FlashWhite()
    {
        Color originalColor = spriteRenderer.color;
        spriteRenderer.color = Color.white;
        yield return new WaitForSeconds(flashDuration);
        spriteRenderer.color = originalColor;
    }

    void PlayIdle()
    {
        anim.Play(Anim("Idle"));
    }

    string Anim(string prefix)
    {
        if (Mathf.Abs(lastDir.x) > Mathf.Abs(lastDir.y))
            return lastDir.x > 0 ? $"{treantName}_{prefix}_R" : $"{treantName}_{prefix}_L";
        else
            return lastDir.y > 0 ? $"{treantName}_{prefix}_U" : $"{treantName}_{prefix}_D";
    }
}
