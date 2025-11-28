using UnityEngine;

public class BounceProjectile : MonoBehaviour
{
    public int bouncesRemaining = 3;
    public float speed = 12f;
    public int damage = 1;
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Launch(Vector2 dir, float s)
    {
        speed = s;
        rb.linearVelocity = dir * speed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (bouncesRemaining > 0)
        {
            Vector2 reflect = Vector2.Reflect(rb.linearVelocity.normalized, collision.contacts[0].normal);
            rb.linearVelocity = reflect * speed;
            bouncesRemaining--;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
