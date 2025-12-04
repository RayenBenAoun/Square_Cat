using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 12f;
    public int damage = 1;
    public EnemyColor projectileColor = EnemyColor.None;

    public int bounce = 0;
    public int pierce = 0;

    public Vector2 direction;
    Rigidbody2D rb;
    float stuckTime = 0;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        rb.gravityScale = 0;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.linearVelocity = direction.normalized * speed;
    }

    void FixedUpdate()
    {
        // If projectile slows down → restore motion
        if (rb.linearVelocity.sqrMagnitude < 0.1f)
            rb.linearVelocity = direction * speed;

        // If projectile is stuck in a wall → destroy
        stuckTime += Time.fixedDeltaTime;
        if (stuckTime > 0.4f)
            Destroy(gameObject);
    }

    void Update()
    {
        rb.linearVelocity = direction.normalized * speed;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Enemy"))
            return;

        if (collision.collider.CompareTag("Player"))
            return;

        // Bounce logic
        if (bounce > 0)
        {
            Vector2 normal = collision.contacts[0].normal;
            direction = Vector2.Reflect(direction, normal).normalized;
            rb.linearVelocity = direction * speed;
            bounce--;
            stuckTime = 0; // reset stuck timer
            return;
        }

        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // ⭐ ADD — Player takes knockback + flash + hurt sound
        if (collision.CompareTag("Player"))
        {
            PlayerMovement pm = collision.GetComponent<PlayerMovement>();
            if (pm != null)
            {
                Vector2 dir = (collision.transform.position - transform.position).normalized;
                pm.TakeHit(dir);
            }

            Destroy(gameObject);
            return;
        }

        // Dragon hit
        DragonAI dragon = collision.GetComponentInParent<DragonAI>();
        if (dragon != null)
        {
            dragon.TakeProjectileHit(damage);
            Destroy(gameObject);
            return;
        }

        // Enemy hit
        EnemyHealth enemy = collision.GetComponentInParent<EnemyHealth>();
        if (enemy != null)
        {
            enemy.OnProjectileHit(projectileColor);
            Destroy(gameObject);
            return;
        }
    }



}
