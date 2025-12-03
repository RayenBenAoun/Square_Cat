using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class SkeletonAi : MonoBehaviour
{
    public string skeletonName = "Skeleton_Archer";

    public enum AttackType { Melee, Ranged }
    public AttackType attackType = AttackType.Ranged;

    [Header("Stats")]
    public int meleeDamage = 1;
    public float projectileSpeed = 7f;
    public float moveSpeed = 2.2f;
    public float aggroRange = 7f;
    public float attackRange = 2f;          // melee
    public float rangedAttackDistance = 5f; // ranged

    [Header("Attack Timings")]
    public float attackCooldown = 1.4f;
    public float attackWindupTime = 0.35f;  // frame where hit / projectile happens
    public float attackAnimTime = 0.7f;     // full length of attack anim

    [Header("Projectile")]
    public GameObject projectilePrefab;

    bool isAttacking = false;
    float lastAttackTime = 0f;

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

        rb.gravityScale = 0;
        rb.freezeRotation = true;
    }

    void Update()
    {
        if (player == null) return;

        float dist = Vector2.Distance(transform.position, player.position);
        lastDir = (player.position - transform.position).normalized;

        if (health != null && health.IsCurrentlyDowned())
        {
            RunAwayFromPlayer();
            return;
        }

        if (isAttacking)
        {
            rb.linearVelocity = Vector2.zero;
            anim.Play(SkelAnim("Attack"));
            return;
        }

        // ranged
        if (attackType == AttackType.Ranged && dist <= rangedAttackDistance)
        {
            rb.linearVelocity = Vector2.zero;
            TryAttack();
            return;
        }

        // melee
        if (attackType == AttackType.Melee && dist <= attackRange)
        {
            rb.linearVelocity = Vector2.zero;
            TryAttack();
            return;
        }

        // move toward player
        if (dist <= aggroRange)
            MoveTowardPlayer();
        else
            rb.linearVelocity = Vector2.zero;

        UpdateAnimation();
    }

    void MoveTowardPlayer()
    {
        rb.linearVelocity = lastDir * moveSpeed;
    }

    void RunAwayFromPlayer()
    {
        Vector2 dir = (transform.position - player.position).normalized;
        lastDir = dir;
        rb.linearVelocity = dir * (moveSpeed * 0.5f);
        anim.Play(SkelAnim("Move"));
    }

    void TryAttack()
    {
        if (Time.time < lastAttackTime + attackCooldown) return;

        lastAttackTime = Time.time;
        isAttacking = true;

        anim.Play(SkelAnim("Attack"));

        if (attackType == AttackType.Melee)
            Invoke(nameof(DealMeleeDamage), attackWindupTime);
        else
            Invoke(nameof(FireProjectile), attackWindupTime);

        Invoke(nameof(EndAttack), attackAnimTime);
    }

    void DealMeleeDamage()
    {
        if (Vector2.Distance(transform.position, player.position) <= attackRange + 0.4f)
            player.GetComponent<PlayerHealth>()?.TakeDamage(meleeDamage);
    }

    void FireProjectile()
    {
        if (!projectilePrefab) return;

        Vector2 dir = (player.position - transform.position).normalized;

        GameObject proj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);

        // rotate arrow to face direction of travel
        proj.transform.right = dir;

        Rigidbody2D prb = proj.GetComponent<Rigidbody2D>();
        if (prb != null)
            prb.linearVelocity = dir * projectileSpeed;

        // ignore collisions with all skeleton colliders
        Collider2D projCol = proj.GetComponent<Collider2D>();
        if (projCol != null)
        {
            Collider2D[] myCols = GetComponentsInChildren<Collider2D>();
            foreach (var c in myCols)
                Physics2D.IgnoreCollision(projCol, c, true);
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
            anim.Play(SkelAnim("Move"));
        else
            anim.Play(SkelAnim("Idle"));
    }

    string SkelAnim(string prefix)
    {
        if (Mathf.Abs(lastDir.x) > Mathf.Abs(lastDir.y))
            return lastDir.x > 0 ? $"{skeletonName}_{prefix}_R" : $"{skeletonName}_{prefix}_L";
        else
            return lastDir.y > 0 ? $"{skeletonName}_{prefix}_U" : $"{skeletonName}_{prefix}_D";
    }
}
