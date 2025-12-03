using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class GolemAI : MonoBehaviour
{
    [Header("Golem Animation Prefix")]
    public string golemName = "Golem_Red"; // e.g. Golem_Red / Golem_Blue

    [Header("Combat Settings")]
    public int meleeDamage = 2;
    public float moveSpeed = 1.4f;
    public float aggroRange = 6f;
    public float attackRange = 1.5f;
    public float attackCooldown = 1.5f;

    private float lastAttackTime = 0;
    private bool isAttacking = false;
    float attackAnimTime = 0.7f;

    Transform player;
    Rigidbody2D rb;
    Animator anim;
    EnemyHealth health;

    Vector2 lastDir = Vector2.down;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        health = GetComponentInChildren<EnemyHealth>();

        rb.freezeRotation = true;
        rb.gravityScale = 0;
    }

    void Update()
    {
        float dist = Vector2.Distance(transform.position, player.position);

        if (health.IsCurrentlyDowned())
        {
            RunAwayFromPlayer();
            return;
        }

        if (isAttacking)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

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
        }

        UpdateAnimation();
    }

    void RunAwayFromPlayer()
    {
        Vector2 dir = (transform.position - player.position).normalized;
        lastDir = dir;
        rb.linearVelocity = dir * (moveSpeed * 0.5f);
        anim.Play(GolemAnim("Move"));
    }

    void MoveTowardPlayer()
    {
        Vector2 moveDir = (player.position - transform.position).normalized;
        lastDir = moveDir;
        rb.linearVelocity = moveDir * moveSpeed;
    }

    void TryAttack()
    {
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            lastAttackTime = Time.time;
            isAttacking = true;
            anim.Play(GolemAnim("Attack"));

            Invoke(nameof(DealDamage), 0.4f);
            Invoke(nameof(EndAttack), attackAnimTime);
        }
    }

    void DealDamage()
    {
        if (Vector2.Distance(transform.position, player.position) <= attackRange + 0.4f)
        {
            player.GetComponent<PlayerHealth>()?.TakeDamage(meleeDamage);
        }
    }

    void EndAttack()
    {
        isAttacking = false;
    }

    void UpdateAnimation()
    {
        if (isAttacking) return;

        if (rb.linearVelocity.sqrMagnitude > 0.1f)
            anim.Play(GolemAnim("Move"));
        else
            anim.Play(GolemAnim("Idle"));
    }

    string GolemAnim(string prefix)
    {
        if (Mathf.Abs(lastDir.x) > Mathf.Abs(lastDir.y))
            return lastDir.x > 0 ? $"{golemName}_{prefix}_Right" : $"{golemName}_{prefix}_Left";
        else
            return lastDir.y > 0 ? $"{golemName}_{prefix}_Up" : $"{golemName}_{prefix}_Down";
    }
}
