using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 12f;
    public int damage = 1;
    public int bounce = 0;
    public int pierce = 0;

    [HideInInspector] public Vector2 direction;

    private Rigidbody2D rb;
    private CircleCollider2D col;
    private LayerMask wallMask;
    private LayerMask playerMask;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<CircleCollider2D>();
    }

    void Start()
    {
        // Force no gravity
        rb.gravityScale = 0;

        // Maintain movement
        rb.linearVelocity = direction * speed;

        // Set layer masks
        wallMask = LayerMask.GetMask("Environment");
        playerMask = LayerMask.GetMask("Player");

        // Ignore collision with the player layer
        Collider2D player = Physics2D.OverlapCircle(transform.position, 0.1f, playerMask);
        if (player != null)
            Physics2D.IgnoreCollision(col, player, true);
    }

    void Update()
    {
        // Enforce no gravity every frame
        if (rb.gravityScale != 0)
            rb.gravityScale = 0;

        // Enforce movement every frame
        rb.linearVelocity = direction * speed;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject hitObj = collision.gameObject;

        // Ignore collision with player ALWAYS
        if (hitObj.layer == LayerMask.NameToLayer("Player"))
            return;

        // If bounces remain — reflect
        if (bounce > 0)
        {
            direction = Vector2.Reflect(direction, collision.contacts[0].normal).normalized;
            rb.linearVelocity = direction * speed;
            bounce--;
            return;
        }

        // Otherwise destroy
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // Enemy detection
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
