using UnityEngine;
using System.Collections;

public enum EnemyState { Alive, DownedColor, Dead }

public class EnemyHealth : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 3;
    int currentHealth;

    [Header("Color System")]
    public EnemyColor enemyColor;
    public float downedDuration = 3f;

    [Header("Flash Visuals")]
    public SpriteRenderer spriteRenderer;
    public float flashInterval = 0.2f;

    public event System.Action<GameObject> OnEnemyDied;

    private EnemyState state = EnemyState.Alive;
    public bool IsDowned => state == EnemyState.DownedColor;

    private Color originalColor;
    private Coroutine flashRoutine;
    private Coroutine downedRoutine;

    void Awake()
    {
        currentHealth = maxHealth;

        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        originalColor = spriteRenderer.color;
    }

    public void TakeDamage(int amount, EnemyColor? projectileColor = null)
    {
        if (state == EnemyState.Alive)
        {
            currentHealth -= amount;
            if (currentHealth <= 0)
            {
                EnterDownedState();
            }
        }
        else if (state == EnemyState.DownedColor)
        {
            if (projectileColor.HasValue && projectileColor.Value == enemyColor)
            {
                Die();
            }
        }
    }

    private void EnterDownedState()
    {
        state = EnemyState.DownedColor;

        if (flashRoutine != null)
            StopCoroutine(flashRoutine);

        flashRoutine = StartCoroutine(FlashEffect());

        if (downedRoutine != null)
            StopCoroutine(downedRoutine);

        downedRoutine = StartCoroutine(DownedTimer());
    }

    private IEnumerator DownedTimer()
    {
        yield return new WaitForSeconds(downedDuration);

        Revive();
    }

    private void Revive()
    {
        state = EnemyState.Alive;
        currentHealth = maxHealth;

        if (flashRoutine != null)
            StopCoroutine(flashRoutine);

        spriteRenderer.color = originalColor;
    }

    private IEnumerator FlashEffect()
    {
        Color flashColor = GetBrightColor(enemyColor);

        while (state == EnemyState.DownedColor)
        {
            spriteRenderer.color = flashColor;
            yield return new WaitForSeconds(flashInterval);
            spriteRenderer.color = originalColor;
            yield return new WaitForSeconds(flashInterval);
        }
    }

    private Color GetBrightColor(EnemyColor c)
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

    private void Die()
    {
        state = EnemyState.Dead;
        OnEnemyDied?.Invoke(gameObject);
        Destroy(gameObject);
    }
}
