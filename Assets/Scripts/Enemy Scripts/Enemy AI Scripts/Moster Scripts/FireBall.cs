using UnityEngine;

public class DragonFireball : MonoBehaviour
{
    public float speed = 8f;
    public int damage = 1;
    public float lifeTime = 4f;

    Rigidbody2D rb;

    public void Launch(Vector2 dir)
    {
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();

        rb.gravityScale = 0;
        rb.linearVelocity = dir.normalized * speed;
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Ignore dragon itself
        if (other.GetComponent<DragonAI>())
            return;

        // Hit player
        PlayerHealth player = other.GetComponentInParent<PlayerHealth>();
        if (player != null)
        {
            player.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }

        // Hit walls / environment
        if (!other.isTrigger)
            Destroy(gameObject);
    }
}
