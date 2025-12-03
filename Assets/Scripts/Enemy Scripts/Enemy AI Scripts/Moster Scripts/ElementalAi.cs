using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ElementalAI : MonoBehaviour
{
    [Header("Element Name Prefix")]
    public string elementalName = "Elemental_Fire_3";

    [Header("Stats")]
    public bool rangedElement = false;
    public int meleeDamage = 1;
    public float moveSpeed = 2f;
    public float aggroRange = 6f;
    public float attackRange = 1.8f;
    public float attackCooldown = 1.4f;

    bool isAttacking = false;
    float lastAttackTime = 0f;
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
        if (player == null) return;

        float dist = Vector2.Distance(transform.position, player.position);
        lastDir = (player.position - transform.position).normalized;

        if (health.IsCurrentlyDowned())
        {
            RunAwayFromPlayer();
            return;
        }

        if (isAttacking)
        {
            rb.linearVelocity = Vector2.zero;
            anim.Play(ElementalAnim("Attack"));
            return;
        }

        if (dist <= attackRange)
        {
            rb.linearVelocity = Vector2.zero;
            TryAttack();
            return;
        }

        if (dist <= aggroRange)
        {
            MoveTowardPlayer();
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }

        UpdateAnimation();
    }

    void MoveTowardPlayer()
    {
        Vector2 moveDir = lastDir;
        rb.linearVelocity = moveDir * moveSpeed;
    }

    void RunAwayFromPlayer()
    {
        Vector2 dir = (transform.position - player.position).normalized;
        lastDir = dir;
        rb.linearVelocity = dir * (moveSpeed * 0.5f);
        anim.Play(ElementalAnim("Move"));
    }

    void TryAttack()
    {
        if (Time.time < lastAttackTime + attackCooldown)
            return;

        lastAttackTime = Time.time;
        isAttacking = true;

        anim.Play(ElementalAnim("Attack"));

        Invoke(nameof(DealDamage), 0.35f);
        Invoke(nameof(EndAttack), attackAnimTime);
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
        if (isAttacking)
            return;

        if (rb.linearVelocity.sqrMagnitude > 0.1f)
            anim.Play(ElementalAnim("Move"));
        else
            anim.Play(ElementalAnim("Idle"));
    }

    string ElementalAnim(string prefix)
    {
        if (Mathf.Abs(lastDir.x) > Mathf.Abs(lastDir.y))
            return lastDir.x > 0 ? $"{elementalName}_{prefix}_R" : $"{elementalName}_{prefix}_L";
        else
            return lastDir.y > 0 ? $"{elementalName}_{prefix}_U" : $"{elementalName}_{prefix}_D";
    }
}
