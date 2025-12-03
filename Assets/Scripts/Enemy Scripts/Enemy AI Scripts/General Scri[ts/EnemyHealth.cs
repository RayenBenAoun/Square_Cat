using UnityEngine;
using System.Collections;

public enum EnemyState { Alive, DownedColor, Dead }

public class EnemyHealth : MonoBehaviour
{
    [Header("Health")]
    public int MaxHealth = 3;
    public int Health = 3;
    public EnemyColor enemyColor;
    public bool downed = false;

    // One simple event: called when this enemy dies
    public event System.Action OnEnemyDied;

    private EnemyState state = EnemyState.Alive;
    public bool IsDead => state == EnemyState.Dead;

    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    public float flashInterval = 0.2f;
    public float downedDuration = 3f;
    private Coroutine flashRoutine;
    private Coroutine downedRoutine;

    Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        originalColor = spriteRenderer.color;
    }

    public bool IsCurrentlyDowned()
    {
        return state == EnemyState.DownedColor;
    }

    public void OnProjectileHit(EnemyColor hitColor)
    {
        if (IsDead) return;

        // First phase: take 3 hits, then go downed
        if (!IsCurrentlyDowned())
        {
            Health--;

            if (Health <= 0)
                EnterDownedState();
        }
        // Second phase: if downed, only matching color kills
        else
        {
            if (hitColor == enemyColor)
                KillEnemy();
        }
    }

    void EnterDownedState()
    {
        state = EnemyState.DownedColor;
        downed = true;

        if (flashRoutine != null) StopCoroutine(flashRoutine);
        flashRoutine = StartCoroutine(FlashEffect());

        if (downedRoutine != null) StopCoroutine(downedRoutine);
        downedRoutine = StartCoroutine(DownedTimer());
    }

    IEnumerator DownedTimer()
    {
        yield return new WaitForSeconds(downedDuration);
        Revive();
    }

    void Revive()
    {
        state = EnemyState.Alive;
        downed = false;
        Health = MaxHealth;
        spriteRenderer.color = originalColor;
    }

    IEnumerator FlashEffect()
    {
        Color c = GetBrightColor(enemyColor);

        while (IsCurrentlyDowned())
        {
            spriteRenderer.color = c;
            yield return new WaitForSeconds(flashInterval);
            spriteRenderer.color = originalColor;
            yield return new WaitForSeconds(flashInterval);
        }
    }

    Color GetBrightColor(EnemyColor c)
    {
        return c switch
        {
            EnemyColor.Red => Color.red,
            EnemyColor.Blue => Color.blue,
            EnemyColor.Green => Color.green,
            EnemyColor.Yellow => Color.yellow,
            _ => Color.white,
        };
    }

    public void KillEnemy()
    {
        if (IsDead) return;

        state = EnemyState.Dead;

        // Tell the wave system “this enemy died”
        OnEnemyDied?.Invoke();

        Destroy(gameObject);
    }
}
