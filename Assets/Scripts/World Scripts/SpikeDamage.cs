using UnityEngine;
using System.Collections;

public class SpikeDamage : MonoBehaviour
{
    private bool isAttached = false;
    private bool damageActive = false;
    private Rigidbody2D rb;
    private Collider2D col;

    private EnemyColor spikeColor = EnemyColor.None;
    private SpriteRenderer sr;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    public void AttachToWall(EdgeCollider2D wall, Vector2 normal)
    {
        isAttached = true;
        damageActive = false;

        rb.isKinematic = true;
        rb.linearVelocity = Vector2.zero;

        Physics2D.IgnoreCollision(col, wall, true);
    }

    public void SetSpikeColor(EnemyColor color)
    {
        spikeColor = color;
        if (sr != null)
            sr.color = ColorFor(color);
    }

    public void Launch()
    {
        isAttached = false;
        rb.isKinematic = false;
        rb.linearVelocity = transform.up * 11f;
        StartCoroutine(EnableCollisionAfterDelay());
        Destroy(gameObject, 3f);
    }

    private IEnumerator EnableCollisionAfterDelay()
    {
        yield return new WaitForSeconds(0.15f);
        damageActive = true;
    }

    void OnCollisionEnter2D(Collision2D col2)
    {
        if (!damageActive) return;

        var enemy = col2.collider.GetComponentInParent<EnemyHealth>();
        if (enemy != null)
        {
            enemy.TakeDamage(1, spikeColor);
            Destroy(gameObject);
            return;
        }

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
            default: return Color.white;
        }
    }
}
