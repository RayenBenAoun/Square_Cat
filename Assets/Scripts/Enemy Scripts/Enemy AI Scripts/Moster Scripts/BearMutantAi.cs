using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class BearMutantAI : MonoBehaviour
{
    [Header("Phase 1 (normal bear)")]
    public float phase1MoveSpeed = 2f;
    public float phase1AttackCooldown = 1.4f;
    public int phase1Damage = 1;

    [Header("Phase 2 (mutated)")]
    public float phase2MoveSpeed = 3f;
    public float phase2AttackCooldown = 0.9f;
    public int phase2Damage = 2;
    [Tooltip("HP % at which the bear mutates (0–1).")]
    public float mutateHpThreshold = 0.5f;
    public float mutateAnimDuration = 1.5f;   // how long the Mutate animation takes

    [Header("General")]
    public float aggroRange = 6f;
    public float attackRange = 1.6f;

    Transform player;
    Rigidbody2D rb;
    Animator anim;
    EnemyHealth health;

    bool isMutated = false;
    bool isAttacking = false;
    float lastAttackTime = 0f;

    Vector2 lastMoveDir = Vector2.down;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        health = GetComponent<EnemyHealth>();

        rb.gravityScale = 0f;
        rb.freezeRotation = true;
    }

    void Update()
    {
        if (player == null) return;

        // If your downed system is active, stop AI and just let the
        // downed animation / behavior play.
        if (health.IsCurrentlyDowned())
        {
            rb.linearVelocity = Vector2.zero;
            SetMove(false);
            SetAttack(false);
            SetAttack2(false);
            return;
        }

        // Check for mutation trigger
        float hpPercent = (float)health.Health / health.MaxHealth;
        if (!isMutated && hpPercent <= mutateHpThreshold)
        {
            StartCoroutine(MutateRoutine());
            return; // let the mutate anim play
        }

        if (isAttacking)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        float dist = Vector2.Distance(transform.position, player.position);

        if (dist <= attackRange && Time.time >= lastAttackTime + GetAttackCooldown())
        {
            StartCoroutine(AttackRoutine());
        }
        else if (dist <= aggroRange)
        {
            MoveTowardPlayer();
        }
        else
        {
            // Idle
            rb.linearVelocity = Vector2.zero;
            SetMove(false);
        }
    }

    // ------------- PHASE CHANGE -------------

    IEnumerator MutateRoutine()
    {
        isMutated = true;

        rb.linearVelocity = Vector2.zero;
        SetMove(false);
        SetAttack(false);
        SetAttack2(false);

        // Ability trigger → Mutate state → Idle 2 (your transitions)
        anim.ResetTrigger("Ability");
        anim.SetTrigger("Ability");

        yield return new WaitForSeconds(mutateAnimDuration);
    }

    // ------------- MOVEMENT -------------

    void MoveTowardPlayer()
    {
        Vector2 dir = (player.position - transform.position).normalized;
        lastMoveDir = dir;

        rb.linearVelocity = dir * GetMoveSpeed();

        SetDirection(dir);
        SetMove(true);
    }

    // ------------- ATTACK -------------

    IEnumerator AttackRoutine()
    {
        isAttacking = true;
        lastAttackTime = Time.time;

        rb.linearVelocity = Vector2.zero;
        SetMove(false);

        // face the player
        Vector2 dir = (player.position - transform.position).normalized;
        lastMoveDir = dir;
        SetDirection(dir);

        // Choose which attack param to use
        if (isMutated)
        {
            anim.ResetTrigger("Attack 2");
            anim.SetTrigger("Attack 2");
        }
        else
        {
            anim.ResetTrigger("Attack");
            anim.SetTrigger("Attack");
        }

        // wind-up time (tune to your animation)
        yield return new WaitForSeconds(0.3f);

        // deal damage if still in range
        float dist = Vector2.Distance(transform.position, player.position);
        if (dist <= attackRange + 0.3f)
        {
            PlayerHealth ph = player.GetComponent<PlayerHealth>();
            if (ph != null)
                ph.TakeDamage(GetDamage());
        }

        // small recovery
        yield return new WaitForSeconds(0.4f);
        isAttacking = false;
    }

    // ------------- ANIMATOR HELPERS -------------

    void SetDirection(Vector2 dir)
    {
        anim.SetFloat("Horizontal", dir.x);
        anim.SetFloat("Vertical", dir.y);
    }

    void SetMove(bool value)
    {
        anim.SetBool("Move", value);
    }

    void SetAttack(bool value)
    {
        anim.SetBool("Attack", value);
    }

    void SetAttack2(bool value)
    {
        anim.SetBool("Attack 2", value);
    }

    float GetMoveSpeed() =>
        isMutated ? phase2MoveSpeed : phase1MoveSpeed;

    float GetAttackCooldown() =>
        isMutated ? phase2AttackCooldown : phase1AttackCooldown;

    int GetDamage() =>
        isMutated ? phase2Damage : phase1Damage;
}
