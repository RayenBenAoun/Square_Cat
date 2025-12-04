using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Base Movement")]
    public float speed = 4f;
    private Vector2 moveDir;
    private Rigidbody2D rb;
    private Animator animator;

    [Header("Dash Ability")]
    public bool canDash = false;
    public bool dashInvincible = false;
    public bool doubleDash = false;
    public float dashSpeed = 12f;
    public float dashDuration = 0.18f;
    public float dashCooldown = 0.75f;

    private bool isDashing = false;
    private bool dashOnCooldown = false;
    private int remainingDashes = 1;

    private int playerLayer;
    private int invincibleLayer;

    // ⭐ NEW — Hit feedback
    [Header("Hit Feedback")]
    public float knockbackForce = 6f;
    public float flashTime = 0.1f;
    private SpriteRenderer sr;
    private Color originalColor;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        playerLayer = LayerMask.NameToLayer("Player");
        invincibleLayer = LayerMask.NameToLayer("Invincible");

        remainingDashes = 1;

        // Flash on hit
        sr = GetComponentInChildren<SpriteRenderer>();
        if (sr != null) originalColor = sr.color;
    }

    private void Update()
    {
        if (!isDashing)
            HandleMovement();

        if (canDash && Input.GetKeyDown(KeyCode.F))
        {
            if (!dashOnCooldown && remainingDashes > 0)
                StartCoroutine(Dash());
        }
    }

    private void HandleMovement()
    {
        moveDir = Vector2.zero;

        if (Input.GetKey(KeyCode.A)) { moveDir.x = -1; animator.SetInteger("Direction", 3); }
        else if (Input.GetKey(KeyCode.D)) { moveDir.x = 1; animator.SetInteger("Direction", 2); }

        if (Input.GetKey(KeyCode.W)) { moveDir.y = 1; animator.SetInteger("Direction", 1); }
        else if (Input.GetKey(KeyCode.S)) { moveDir.y = -1; animator.SetInteger("Direction", 0); }

        moveDir.Normalize();
        animator.SetBool("IsMoving", moveDir.sqrMagnitude > 0f);

        rb.linearVelocity = moveDir * speed;
    }

    private IEnumerator Dash()
    {
        isDashing = true;
        remainingDashes--;

        if (remainingDashes <= 0)
            dashOnCooldown = true;

        Vector2 dashVector = moveDir == Vector2.zero ? Vector2.down : moveDir;

        // ⭐ PLAY DASH SOUND EXACTLY ONCE PER DASH
        PlayerAudio.Instance.Play(PlayerAudio.Instance.dashSFX);

        if (dashInvincible)
            gameObject.layer = invincibleLayer;

        rb.linearVelocity = dashVector * dashSpeed;

        yield return new WaitForSeconds(dashDuration);

        if (dashInvincible)
            gameObject.layer = playerLayer;

        isDashing = false;

        if (dashOnCooldown)
        {
            yield return new WaitForSeconds(dashCooldown);
            remainingDashes = doubleDash ? 2 : 1;
            dashOnCooldown = false;
        }
    }

    // ========================
    // ⭐ NEW HIT FEEDBACK API
    // ========================
    public void TakeHit(Vector2 hitDirection)
    {
        rb.linearVelocity = hitDirection.normalized * knockbackForce;

        if (sr != null)
            StartCoroutine(FlashRed());

        // ⭐ PLAY HURT SOUND
        PlayerAudio.Instance.Play(PlayerAudio.Instance.hurtSFX);
    }

    private IEnumerator FlashRed()
    {
        sr.color = Color.red;
        yield return new WaitForSeconds(flashTime);
        sr.color = originalColor;
    }

    // ===== UPGRADE HOOKS (unchanged) =====

    public void IncreaseSpeed(float amount) => speed += amount;
    public void UnlockDash() { canDash = true; remainingDashes = 1; }
    public void UnlockDoubleDash() { doubleDash = true; if (canDash) remainingDashes = 2; }
    public void UnlockDashInvincibility() => dashInvincible = true;
}
