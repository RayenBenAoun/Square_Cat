using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float aggroRange = 10f;
    public float attackRange = 1.5f;
    public float attackCooldown = 3f; // <<< cooldown here

    Transform player;
    Rigidbody2D rb;
    Animator anim;
    EnemyHealth health;

    float lastAttackTime = 0f;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        health = GetComponentInChildren<EnemyHealth>();
    }

    void Update()
    {
        if (player == null) return;

        float dist = Vector2.Distance(transform.position, player.position);

        if (dist <= attackRange && !health.IsDowned)
        {
            rb.linearVelocity = Vector2.zero;
            anim.SetBool("Move", false);
            TryAttack();
        }
        else if (dist <= aggroRange)
        {
            MoveTowardPlayer();
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
            anim.SetBool("Move", false);
        }

        UpdateAnimation();
    }

    void MoveTowardPlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = direction * moveSpeed;
    }

    void TryAttack()
    {
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            lastAttackTime = Time.time;
            anim.SetTrigger("Attack");
        }
    }

    void UpdateAnimation()
    {
        Vector2 v = rb.linearVelocity;
        anim.SetFloat("Horizontal", Mathf.Clamp(v.x, -1f, 1f));
        anim.SetFloat("Vertical", Mathf.Clamp(v.y, -1f, 1f));
        anim.SetBool("Move", v.sqrMagnitude > 0.05f);
    }
}
