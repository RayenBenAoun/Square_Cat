using UnityEngine;

public class TrainingDummy : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;

    public float respawnTime = 3f;
    public GameObject model; // The visible mesh/sprite
    public Animator anim;    // For downed animation

    private Vector3 startPosition;
    private Quaternion startRotation;

    void Start()
    {
        startPosition = transform.position;
        startRotation = transform.rotation;
        Respawn();
    }

    public void TakeDamage(int dmg)
    {
        if (currentHealth <= 0) return; // Already dead

        currentHealth -= dmg;

        if (currentHealth <= 0)
        {
            Downed();
        }
    }

    void Downed()
    {
        // Trigger animation
        if (anim != null)
            anim.SetTrigger("Downed");

        // Disable hitbox or collision
        GetComponent<Collider2D>().enabled = false;

        // Hide or ragdoll after animation delay
        Invoke(nameof(Die), 0.4f);
    }

    void Die()
    {
        // Hide model
        if (model != null)
            model.SetActive(false);

        // Start respawn timer
        Invoke(nameof(Respawn), respawnTime);
    }

    void Respawn()
    {
        currentHealth = maxHealth;

        // Reset position
        transform.position = startPosition;
        transform.rotation = startRotation;

        // Enable visuals & collider
        if (model != null)
            model.SetActive(true);

        GetComponent<Collider2D>().enabled = true;

        // Play idle animation
        if (anim != null)
            anim.SetTrigger("Idle");
    }
}
