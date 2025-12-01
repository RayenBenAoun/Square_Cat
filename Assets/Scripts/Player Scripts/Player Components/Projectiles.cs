using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 12f;
    public int damage = 1;

    // restore these:
    public int bounce = 0;
    public int pierce = 0;

    public Vector2 direction;

    Rigidbody2D rb;
    CircleCollider2D col;
    LayerMask wallMask;
    LayerMask playerMask;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<CircleCollider2D>();
    }

    void Start()
    {
        rb.gravityScale = 0;
        rb.linearVelocity = direction * speed;

        wallMask = LayerMask.GetMask("Environment");
        playerMask = LayerMask.GetMask("Player");

        Collider2D player = Physics2D.OverlapCircle(transform.position, 0.1f, playerMask);
        if (player != null)
            Physics2D.IgnoreCollision(col, player, true);
    }

    void Update()
    {
        rb.linearVelocity = direction * speed;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject hitObj = collision.gameObject;

        if (hitObj.layer == LayerMask.NameToLayer("Player"))
            return;

        if (bounce > 0)
        {
            direction = Vector2.Reflect(direction, collision.contacts[0].normal).normalized;
            rb.linearVelocity = direction * speed;
            bounce--;
            return;
        }

        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        EnemyHealth enemy = collision.GetComponentInParent<EnemyHealth>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);

            if (pierce > 0)
            {
                pierce--;
                return;
            }

            Destroy(gameObject);
        }
    }
}
