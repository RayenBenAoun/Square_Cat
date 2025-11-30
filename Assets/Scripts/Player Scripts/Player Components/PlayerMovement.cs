using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Base Movement")]
    public float speed = 4f;          // base move speed
    private Vector2 moveDir;
    private Rigidbody2D rb;
    private Animator animator;

    [Header("Dash Ability")]
    public bool canDash = false;      // unlocked by upgrade
    public bool dashInvincible = false;
    public bool doubleDash = false;   // unlocked by upgrade
    public float dashSpeed = 12f;
    public float dashDuration = 0.18f;
    public float dashCooldown = 0.75f;

    private bool isDashing = false;
    private bool dashOnCooldown = false;
    private int remainingDashes = 1;  // 1 normally, 2 if doubleDash

    private int playerLayer;
    private int invincibleLayer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        playerLayer = LayerMask.NameToLayer("Player");
        invincibleLayer = LayerMask.NameToLayer("Invincible");

        remainingDashes = 1;
    }

    private void Update()
    {
        // normal movement when not mid-dash
        if (!isDashing)
            HandleMovement();

        // dash input
        if (canDash && Input.GetKeyDown(KeyCode.F))
        {
            if (!dashOnCooldown && remainingDashes > 0)
                StartCoroutine(Dash());
        }
    }

    private void HandleMovement()
    {
        moveDir = Vector2.zero;

        // horizontal
        if (Input.GetKey(KeyCode.A))
        {
            moveDir.x = -1;
            animator.SetInteger("Direction", 3); // left
        }
        else if (Input.GetKey(KeyCode.D))
        {
            moveDir.x = 1;
            animator.SetInteger("Direction", 2); // right
        }

        // vertical
        if (Input.GetKey(KeyCode.W))
        {
            moveDir.y = 1;
            animator.SetInteger("Direction", 1); // up
        }
        else if (Input.GetKey(KeyCode.S))
        {
            moveDir.y = -1;
            animator.SetInteger("Direction", 0); // down
        }

        moveDir.Normalize();
        animator.SetBool("IsMoving", moveDir.sqrMagnitude > 0f);

        rb.linearVelocity = moveDir * speed;
    }

    private IEnumerator Dash()
    {
        isDashing = true;
        remainingDashes--;

        // if we used our last dash, start cooldown after this dash finishes
        if (remainingDashes <= 0)
            dashOnCooldown = true;

        // default dash direction if standing still
        Vector2 dashVector = moveDir == Vector2.zero ? Vector2.down : moveDir;

        if (dashInvincible)
            gameObject.layer = invincibleLayer;

        rb.linearVelocity = dashVector * dashSpeed;

        yield return new WaitForSeconds(dashDuration);

        if (dashInvincible)
            gameObject.layer = playerLayer;

        isDashing = false;

        // only wait cooldown when we’ve exhausted our dashes
        if (dashOnCooldown)
        {
            yield return new WaitForSeconds(dashCooldown);
            remainingDashes = doubleDash ? 2 : 1;
            dashOnCooldown = false;
        }
    }

    // ===== called by upgrades =====

    // +amount is absolute; if you want +10% per node, call IncreaseSpeed(speed * 0.1f)
    public void IncreaseSpeed(float amount)
    {
        speed += amount;
    }

    public void UnlockDash()
    {
        canDash = true;
        remainingDashes = 1;
    }

    public void UnlockDoubleDash()
    {
        doubleDash = true;
        // if dash already unlocked, immediately allow 2 dashes
        if (canDash)
            remainingDashes = 2;
    }

    public void UnlockDashInvincibility()
    {
        dashInvincible = true;
    }
}
