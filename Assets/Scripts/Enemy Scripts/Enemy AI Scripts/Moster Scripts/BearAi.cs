using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BearAI : MonoBehaviour
{
    [Header("Bear Settings")]
    public int meleeDamage = 1;
    public float moveSpeed = 2f;
    public float aggroRange = 6f;
    public float attackRange = 1.5f;
    public float attackCooldown = 1.5f;

    private float lastAttackTime = 0;

    [Header("References")]
    Transform player;
    Rigidbody2D rb;
    Animator anim;
    EnemyHealth health;

    Vector2 lastDir = Vector2.down;

    bool isAttacking = false;
    float attackAnimTime = 0.6f;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        if (player == null)
        {
            Debug.Log("❗ BEAR AI ERROR: player reference is NULL");
            return;
        }
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        health = GetComponentInChildren<EnemyHealth>();

        rb.freezeRotation = true;
        rb.gravityScale = 0;          // IMPORTANT
    }

    void Update()
    {
        if (player == null) return;

        // DOWNED BEHAVIOR
        if (health.IsCurrentlyDowned())
        {
            RunAwayFromPlayer();
            return;
        }

        float dist = Vector2.Distance(transform.position, player.position);

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
        rb.linearVelocity = dir * (moveSpeed * 0.5f);
        anim.Play(BearAnimDir("Move"));
        lastDir = dir;
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

            anim.Play(BearAnimDir("Attack"));

            Invoke(nameof(DelayedDamage), 0.35f);
            Invoke(nameof(EndAttack), attackAnimTime);
        }
    }

    void DelayedDamage()
    {
        if (health.IsCurrentlyDowned()) return;

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

    void UpdateAnimation()
    {
        if (isAttacking) return;

        if (rb.linearVelocity.sqrMagnitude > 0.1f)
            anim.Play(BearAnimDir("Move"));
        else
            anim.Play(BearAnimDir("Idle"));
    }

    string BearAnimDir(string prefix)
    {
        if (Mathf.Abs(lastDir.x) > Mathf.Abs(lastDir.y))
            return lastDir.x > 0 ? $"Bear_{prefix}_R" : $"Bear_{prefix}_L";
        else
            return lastDir.y > 0 ? $"Bear_{prefix}_U" : $"Bear_{prefix}_D";
    }
}
