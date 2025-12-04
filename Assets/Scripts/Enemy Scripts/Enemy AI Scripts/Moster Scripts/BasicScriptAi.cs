using UnityEngine;
using System.Collections;

public class BasicEnemyAI : MonoBehaviour
{
    [Header("Enemy Name (Prefix for animations)")]
    public string EnemyName = "Mob";   // ex: Slime, Spider

    [Header("Settings")]
    public float moveSpeed = 2f;
    public float aggroRange = 6f;
    public float attackRange = 1.2f;
    public float attackCooldown = 1.3f;
    public int damage = 1;

    private Transform player;
    private Rigidbody2D rb;
    private Animator anim;
    private EnemyHealth hp;

    private bool isAttacking = false;
    private Vector2 lastDir;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        hp = GetComponent<EnemyHealth>();
    }

    void Update()
    {
        if (player == null) return;

        if (hp.IsCurrentlyDowned())
        {
            rb.linearVelocity = Vector2.zero;
            PlayAnim("Idle");
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
            StartCoroutine(DoAttack());
        }
        else if (dist <= aggroRange)
        {
            MoveTowardPlayer();
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
            PlayAnim("Idle");
        }
    }

    void MoveTowardPlayer()
    {
        Vector2 dir = (player.position - transform.position).normalized;
        lastDir = dir;
        rb.linearVelocity = dir * moveSpeed;
        PlayAnim("Move");
    }

    IEnumerator DoAttack()
    {
        isAttacking = true;

        rb.linearVelocity = Vector2.zero;
        PlayAnim("Attack");

        yield return new WaitForSeconds(0.3f);

        float dist = Vector2.Distance(transform.position, player.position);
        if (dist <= attackRange + 0.3f)
        {
            PlayerHealth ph = player.GetComponent<PlayerHealth>();
            if (ph != null)
                ph.TakeDamage(damage);
        }

        yield return new WaitForSeconds(attackCooldown);
        isAttacking = false;
    }

    void PlayAnim(string prefix)
    {
        string dirFull;
        string dirShort;

        // FULL WORD VERSION
        if (Mathf.Abs(lastDir.x) > Mathf.Abs(lastDir.y))
        {
            dirFull = lastDir.x > 0 ? "Right" : "Left";
            dirShort = lastDir.x > 0 ? "R" : "L";
        }
        else
        {
            dirFull = lastDir.y > 0 ? "Up" : "Down";
            dirShort = lastDir.y > 0 ? "U" : "D";
        }

        // Try full word first
        string animNameFull = $"{EnemyName}_{prefix}_{dirFull}";
        // Try short version
        string animNameShort = $"{EnemyName}_{prefix}_{dirShort}";

        // Try playing FULL first
        if (HasAnimation(anim, animNameFull))
        {
            anim.Play(animNameFull);
            return;
        }
        // Then SHORT
        if (HasAnimation(anim, animNameShort))
        {
            anim.Play(animNameShort);
            return;
        }

        // Debug warning one time
        Debug.LogWarning($"{EnemyName} missing animation: {animNameFull} or {animNameShort}");
    }

    bool HasAnimation(Animator animator, string animName)
    {
        foreach (var clip in animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == animName)
                return true;
        }
        return false;
    }
}
