using UnityEngine;

public class MonsterProjectile : MonoBehaviour
{
    public int damage = 1;
    public float lifeTime = 4f;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    // --- shared hit logic ---
    void Hit(GameObject other)
    {
        // ignore enemies (casters, mages, skeletons, etc.)
        if (other.CompareTag("Enemy"))
            return;

        if (other.CompareTag("Player"))
        {
            PlayerHealth ph = other.GetComponent<PlayerHealth>();
            if (ph != null)
                ph.TakeDamage(damage);
        }

        Destroy(gameObject);
    }

    // trigger projectiles (arrow, bolt with "Is Trigger" on)
    void OnTriggerEnter2D(Collider2D col)
    {
        Hit(col.gameObject);
    }

    // non-trigger projectiles (your mage bolt that pushes you)
    void OnCollisionEnter2D(Collision2D col)
    {
        Hit(col.collider.gameObject);
    }
}
