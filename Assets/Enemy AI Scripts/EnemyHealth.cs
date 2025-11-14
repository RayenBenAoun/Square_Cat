using UnityEngine;
using System.Collections;

public enum EnemyState { Alive, DownedColor, Dead }

[DisallowMultipleComponent]
public class EnemyHealth : MonoBehaviour
{
    [Header("Stats")]
    public int maxHealth = 3;
    public EnemyColor enemyColor;

    [Header("Visuals")]
    public SpriteRenderer spriteRenderer;
    public float flashInterval = 0.2f;

    [Header("Downed State")]
    public float downedDuration = 3f;

    [Header("Death Handling")]
    public bool destroyRoot = true;
    public float destroyDelay = 0f;

    [Header("Drops")]
    public GameObject lightningDropPrefab;   // ⚡ assign in Inspector

    private int currentHealth;
    private EnemyState state = EnemyState.Alive;
    private bool dead;

    private Color originalColor;
    private Coroutine downedRoutine;
    private Coroutine flashRoutine;

    private SpriteRenderer outlineRenderer;

    public bool IsDowned => state == EnemyState.DownedColor;

    void Awake()
    {
        currentHealth = maxHealth;

        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        originalColor = spriteRenderer != null ? spriteRenderer.color : Color.white;

        GameObject outlineObj = new GameObject("Outline");
        outlineObj.transform.SetParent(spriteRenderer.transform);
        outlineObj.transform.localPosition = Vector3.zero;

        outlineRenderer = outlineObj.AddComponent<SpriteRenderer>();
        outlineRenderer.sprite = spriteRenderer.sprite;
        outlineRenderer.sortingLayerID = spriteRenderer.sortingLayerID;
        outlineRenderer.sortingOrder = spriteRenderer.sortingOrder - 1;
        outlineRenderer.enabled = false;
    }

    public void TakeDamage(int amount, EnemyColor? projectileColor = null)
    {
        if (dead) return;

        if (state == EnemyState.Alive)
        {
            currentHealth -= amount;
            if (currentHealth <= 0)
                EnterDownedColor();
        }
        else if (state == EnemyState.DownedColor)
        {
            if (projectileColor.HasValue && projectileColor.Value == enemyColor)
                Die();
        }
    }

    private void EnterDownedColor()
    {
        if (state == EnemyState.DownedColor) return;

        state = EnemyState.DownedColor;

        if (downedRoutine != null) StopCoroutine(downedRoutine);
        downedRoutine = StartCoroutine(DownedTimer());

        if (flashRoutine != null) StopCoroutine(flashRoutine);
        flashRoutine = StartCoroutine(FlashEffect());
    }

    private IEnumerator DownedTimer()
    {
        yield return new WaitForSeconds(downedDuration);
        if (!dead) Revive();
    }

    private IEnumerator FlashEffect()
    {
        Color flashColor = GetBrightColor(enemyColor);
        outlineRenderer.color = flashColor;

        float scaleUp = 1.15f;

        while (state == EnemyState.DownedColor)
        {
            spriteRenderer.color = flashColor;
            outlineRenderer.enabled = true;
            outlineRenderer.transform.localScale = Vector3.one * scaleUp;
            yield return new WaitForSeconds(flashInterval);

            spriteRenderer.color = originalColor;
            outlineRenderer.enabled = false;
            yield return new WaitForSeconds(flashInterval);
        }

        spriteRenderer.color = originalColor;
        outlineRenderer.enabled = false;
    }

    private void Revive()
    {
        state = EnemyState.Alive;
        currentHealth = maxHealth;

        if (flashRoutine != null) StopCoroutine(flashRoutine);
        spriteRenderer.color = originalColor;
        outlineRenderer.enabled = false;
    }

    private void Die()
    {
        state = EnemyState.Dead;
        dead = true;

        if (downedRoutine != null) StopCoroutine(downedRoutine);
        if (flashRoutine != null) StopCoroutine(flashRoutine);

        // ⚡ Spawn the lightning drop BEFORE hiding the enemy
        if (lightningDropPrefab != null)
            Instantiate(lightningDropPrefab, transform.position, Quaternion.identity);

        // Disable collisions
        foreach (var col in GetComponentsInChildren<Collider2D>())
            col.enabled = false;

        var rb = GetComponentInChildren<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.simulated = false;
        }

        // Hide sprites AFTER dropping
        foreach (var sr in GetComponentsInChildren<SpriteRenderer>())
            sr.enabled = false;

        GameObject target = destroyRoot ? transform.root.gameObject : gameObject;
        Destroy(target, destroyDelay);
    }

    private Color GetBrightColor(EnemyColor c)
    {
        Color baseC;
        switch (c)
        {
            case EnemyColor.Red: baseC = Color.red; break;
            case EnemyColor.Blue: baseC = Color.blue; break;
            case EnemyColor.Green: baseC = Color.green; break;
            case EnemyColor.Yellow: baseC = Color.yellow; break;
            default: baseC = Color.white; break;
        }

        baseC *= 6f;
        baseC.r = Mathf.Clamp01(baseC.r);
        baseC.g = Mathf.Clamp01(baseC.g);
        baseC.b = Mathf.Clamp01(baseC.b);
        baseC.a = 1f;

        return baseC;
    }
}
