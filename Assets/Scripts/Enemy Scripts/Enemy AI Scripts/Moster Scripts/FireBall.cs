using UnityEngine;

public class DragonFireball : MonoBehaviour
{
    public float speed = 7f;
    public float lifetime = 6f;
    public float playerDamage = 1f;

    Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    // Called by DragonAI
    public void Launch(Vector2 dir)
    {
        dir.Normalize();
        rb.linearVelocity = dir * speed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Ignore dragon
        if (other.CompareTag("Enemy"))
            return;

        // Ignore perch wall
        if (other.CompareTag("DragonWall"))
            return;

        // Hit Player
        if (other.CompareTag("Player"))
        {
            PlayerHealth ph = other.GetComponent<PlayerHealth>();
            if (ph != null)
                ph.TakeDamage((int)playerDamage);

            Destroy(gameObject);
            return;
        }

        // Hit any solid object → destroy
        if (other.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            Destroy(gameObject);
            return;
        }
    }
}
