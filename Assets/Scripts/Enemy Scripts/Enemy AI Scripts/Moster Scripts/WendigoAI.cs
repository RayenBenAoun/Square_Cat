using UnityEngine;
using System.Collections;

public class WendigoAI : MonoBehaviour
{
    [Header("Projectile (Phase 2 Attack)")]
    public GameObject Phase2Projectile;
    public float projectileSpeed = 12f;

    [Header("Phase 1 Movement")]
    public float moveSpeed = 2.5f;
    public float attackCooldown = 1.8f;
    public float dashCooldown = 2f;
    public float dashSpeed = 6.5f;
    public float dashDuration = 0.18f;

    [Header("Phase 2 Movement")]
    public float phase2MoveSpeed = 3.5f;
    public float phase2AttackCooldown = 1.2f;

    [Header("Phase 2 Downed")]
    public float phase2StunDuration = 5f;

    public float aggroRange = 6f;
    public float attackRange = 4f;

    private Transform player;
    private Animator anim;
    private Rigidbody2D rb;
    private EnemyHealth hp;

    public bool isPhase2 = false;          // PUBLIC NOW
    private bool isAttacking = false;
    public bool isStunnedPhase2 = false;   // PUBLIC NOW
    public bool isDownedPhase2 => isStunnedPhase2;

    private float lastAttack;
    private float lastDash;
    private bool phase2Started = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        hp = GetComponent<EnemyHealth>();
    }

    public void OnHitByColorProjectile(EnemyColor hitColor)
    {
        // PHASE 1 DOWNED → NOT DIE → PHASE 2 TRANSITION
        if (hp.IsCurrentlyDowned() && !isPhase2)
        {
            EnterPhase2();
            return;
        }

        // PHASE 2 DOWNED → IGNORE PROJECTILE
        if (hp.IsCurrentlyDowned() && isPhase2)
        {
            Debug.Log("Wendigo Phase 2 downed — ignoring projectile, must use TRACE kill");
            return;
        }

        hp.OnProjectileHit(hitColor);
    }

    void Update()
    {
        if (isStunnedPhase2)
            return;

        Vector2 dir = (player.position - transform.position).normalized;

        if (hp.IsCurrentlyDowned())
        {
            if (isPhase2)
                StartCoroutine(Phase2Stun());
            return;
        }

        float dist = Vector2.Distance(transform.position, player.position);

        if (!isPhase2)
            TryDash(dir);

        if (!isAttacking && dist < attackRange && Time.time - lastAttack > GetAttackCooldown())
        {
            StartCoroutine(DoAttack(dir));
            return;
        }

        if (!isAttacking && dist < aggroRange)
        {
            MoveTowardsPlayer(dir);
            return;
        }

        Idle();
    }

    void EnterPhase2()
    {
        isPhase2 = true;
        phase2Started = true;

        rb.linearVelocity = Vector2.zero;
        anim.SetBool("Move", false);
        anim.SetTrigger("Phase2Start");
    }

    IEnumerator Phase2Stun()
    {
        if (isStunnedPhase2) yield break;
        isStunnedPhase2 = true;

        rb.linearVelocity = Vector2.zero;
        anim.SetBool("Move", false);
        anim.SetTrigger("Phase2Stun");

        yield return new WaitForSeconds(phase2StunDuration);

        isStunnedPhase2 = false;
    }

    IEnumerator DoAttack(Vector2 dir)
    {
        isAttacking = true;
        lastAttack = Time.time;
        rb.linearVelocity = Vector2.zero;
        anim.SetBool("Move", false);
        SetDirectionalAnim(dir);

        if (isPhase2)
        {
            anim.SetTrigger("Phase2Attack");
            yield return new WaitForSeconds(0.3f);
            ShootProjectile();
        }
        else
        {
            anim.SetTrigger("Attack");
            yield return new WaitForSeconds(0.25f);
            TryDealMeleeDamage();
        }

        yield return new WaitForSeconds(0.7f);
        isAttacking = false;
    }

    void ShootProjectile()
    {
        GameObject proj = Instantiate(Phase2Projectile, transform.position, Quaternion.identity);
        Vector2 dir = (player.position - transform.position).normalized;
        proj.GetComponent<Rigidbody2D>().linearVelocity = dir * projectileSpeed;
    }

    void TryDealMeleeDamage()
    {
        if (isPhase2) return;

        Collider2D col = Physics2D.OverlapCircle(transform.position, 1.2f);
        if (col && col.CompareTag("Player"))
            col.GetComponent<PlayerHealth>()?.TakeDamage(1);
    }

    bool TryDash(Vector2 dir)
    {
        if (isPhase2) return false;
        if (Time.time < lastDash + dashCooldown) return false;

        lastDash = Time.time;
        StartCoroutine(Dash(dir));
        return true;
    }

    IEnumerator Dash(Vector2 dir)
    {
        anim.SetTrigger("Dash");

        float end = Time.time + dashDuration;
        while (Time.time < end)
        {
            rb.linearVelocity = dir * dashSpeed;
            yield return null;
        }

        rb.linearVelocity = Vector2.zero;
    }

    void MoveTowardsPlayer(Vector2 dir)
    {
        anim.SetBool("Move", true);
        rb.linearVelocity = dir * GetMoveSpeed();
        SetDirectionalAnim(dir);
    }

    void Idle()
    {
        rb.linearVelocity = Vector2.zero;
        anim.SetBool("Move", false);
    }

    void SetDirectionalAnim(Vector2 dir)
    {
        anim.SetFloat("Horizontal", dir.x);
        anim.SetFloat("Vertical", dir.y);
    }

    float GetMoveSpeed() =>
        isPhase2 ? phase2MoveSpeed : moveSpeed;

    float GetAttackCooldown() =>
        isPhase2 ? phase2AttackCooldown : attackCooldown;

    // Called by TRACE ability kill
    public void KillFromTrace()
    {
        Debug.Log("WENDIGO KILLED BY TRACE!");
        hp.KillEnemy();
    }
}
