using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class MushroomAi : MonoBehaviour
{
    [Header("Mushroom Settings")]
    public string mushroomName = "Mushroom";
    public bool isInfectious = false;

    public int meleeDamage = 1;
    public float moveSpeed = 1.8f;
    public float aggroRange = 4.5f;
    public float attackRange = 1.3f;

    [Header("Attack Timing")]
    public float attackCooldown = 0.8f;     // FASTER
    public float attackWindup = 0.25f;      // damage hits earlier
    public float attackAnimTime = 0.45f;    // fast chomp

    float lastAttackTime = -999;
    bool isAttacking = false;

    Transform player;
    Animator anim;
    Rigidbody2D rb;
    EnemyHealth health;

    Vector2 lastDir = Vector2.down;

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

        float dist = Vector2.Distance(player.position, transform.position);

        if (health != null && health.IsCurrentlyDowned())
        {
            RunAway();
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
            MoveForward();
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }

        UpdateAnim();
    }

    void TryAttack()
    {
        if (Time.time < lastAttackTime + attackCooldown)
            return;

        lastAttackTime = Time.time;
        isAttacking = true;

        anim.Play(MushAnim("Attack"));
        Invoke(nameof(DealDamage), attackWindup);
        Invoke(nameof(StopAttack), attackAnimTime);
    }

    void DealDamage()
    {
        if (health.IsCurrentlyDowned()) return;

        if (Vector2.Distance(transform.position, player.position) <= attackRange + 0.3f)
        {
            int dmg = isInfectious ? meleeDamage + 1 : meleeDamage;
            player.GetComponent<PlayerHealth>()?.TakeDamage(dmg);
        }
    }

    void StopAttack()
    {
        isAttacking = false;
    }

    void MoveForward()
    {
        Vector2 dir = (player.position - transform.position).normalized;
        lastDir = dir;
        rb.linearVelocity = dir * moveSpeed;
    }

    void RunAway()
    {
        Vector2 dir = (transform.position - player.position).normalized;
        rb.linearVelocity = dir * (moveSpeed * 0.5f);
        anim.Play(MushAnim("Move"));
    }

    void UpdateAnim()
    {
        if (isAttacking) return;

        if (rb.linearVelocity.magnitude > 0.1f)
            anim.Play(MushAnim("Move"));
        else
            anim.Play(MushAnim("Idle"));
    }

    string MushAnim(string prefix)
    {
        if (Mathf.Abs(lastDir.x) > Mathf.Abs(lastDir.y))
            return lastDir.x > 0 ? $"{mushroomName}_{prefix}_R" : $"{mushroomName}_{prefix}_L";
        else
            return lastDir.y > 0 ? $"{mushroomName}_{prefix}_U" : $"{mushroomName}_{prefix}_D";
    }
}
