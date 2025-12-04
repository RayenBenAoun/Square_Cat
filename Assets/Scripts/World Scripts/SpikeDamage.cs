using UnityEngine;
using System.Collections;

public class SpikeDamage : MonoBehaviour
{
    private bool damageActive = false;
    private Rigidbody2D rb;
    private Collider2D col;
    private SpriteRenderer sr;

    private EnemyColor spikeColor = EnemyColor.None;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        sr = GetComponent<SpriteRenderer>();

        if (col != null)
            col.isTrigger = false;   // ⭐ CRITICAL: must be solid collider
    }

    // Called when attached to wall BEFORE launch
    public void AttachToWall(EdgeCollider2D wall, Vector2 normal)
    {
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.linearVelocity = Vector2.zero;
        }

        // Temporarily ignore the wall collider only
        if (col != null)
            Physics2D.IgnoreCollision(col, wall, true);
    }

    public void SetSpikeColor(EnemyColor color)
    {
        spikeColor = color;

        if (sr != null)
            sr.color = ColorFor(color);
    }

    public void Launch(Vector2 direction, float speed)
    {
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.linearVelocity = direction.normalized * speed;
        }

        // ⭐ ACTIVATE DAMAGE after small delay so it doesn't hit the wall instantly
        StartCoroutine(ActivateDamage());

        // Projectiles despawn eventually
        Destroy(gameObject, 3f);
    }

    private IEnumerator ActivateDamage()
    {
        yield return new WaitForSeconds(0.1f);

        damageActive = true;

        // Allow collisions with EVERYTHING again
        foreach (Collider2D other in FindObjectsOfType<Collider2D>())
        {
            Physics2D.IgnoreCollision(col, other, false);
        }
    }

    void OnCollisionEnter2D(Collision2D c)
    {
        if (!damageActive) return;

        EnemyHealth enemy = c.collider.GetComponentInParent<EnemyHealth>();

        if (enemy != null)
        {
            enemy.ApplySpikeDamage(1);
            Destroy(gameObject);
            return;
        }

        // Hit anything else → destroy spike
        Destroy(gameObject);
    }

    private Color ColorFor(EnemyColor c)
    {
        switch (c)
        {
            case EnemyColor.Red: return Color.red;
            case EnemyColor.Blue: return Color.blue;
            case EnemyColor.Green: return Color.green;
            case EnemyColor.Yellow: return Color.yellow;
        }
        return Color.white;
    }
}
