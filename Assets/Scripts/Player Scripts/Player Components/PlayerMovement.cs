using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Base Movement")]
    public float speed = 4f;
    public float baseSpeed = 4f; // record original

    private Vector2 moveDir;
    private Rigidbody2D rb;
    private Animator animator;

    [Header("Dash Ability")]
    public bool canDash = false;
    public bool dashInvincible = false;
    public float dashSpeed = 12f;
    public float dashDuration = 0.18f;
    public float dashCooldown = 0.75f;
    private bool isDashing = false;
    private bool dashOnCooldown = false;

    private int playerLayer;
    private int invincibleLayer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        playerLayer = LayerMask.NameToLayer("Player");
        invincibleLayer = LayerMask.NameToLayer("Invincible");
    }

    private void Update()
    {
        if (!isDashing)
            HandleMovement();

        if (canDash && !dashOnCooldown && Input.GetKeyDown(KeyCode.Space))
            StartCoroutine(Dash());
    }

    private void HandleMovement()
    {
        moveDir = Vector2.zero;

        if (Input.GetKey(KeyCode.A)) { moveDir.x = -1; animator.SetInteger("Direction", 3); }
        else if (Input.GetKey(KeyCode.D)) { moveDir.x = 1; animator.SetInteger("Direction", 2); }

        if (Input.GetKey(KeyCode.W)) { moveDir.y = 1; animator.SetInteger("Direction", 1); }
        else if (Input.GetKey(KeyCode.S)) { moveDir.y = -1; animator.SetInteger("Direction", 0); }

        moveDir.Normalize();
        animator.SetBool("IsMoving", moveDir.sqrMagnitude > 0);

        rb.linearVelocity = moveDir * speed;
    }

    private System.Collections.IEnumerator Dash()
    {
        isDashing = true;
        dashOnCooldown = true;

        Vector2 dashVector = moveDir == Vector2.zero ? Vector2.down : moveDir;

        if (dashInvincible)
            gameObject.layer = invincibleLayer;

        rb.linearVelocity = dashVector * dashSpeed;

        yield return new WaitForSeconds(dashDuration);

        if (dashInvincible)
            gameObject.layer = playerLayer;

        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);
        dashOnCooldown = false;
    }

    public void IncreaseSpeed(float amount)
    {
        speed += amount;
        Debug.Log("PLAYER SPEED IS NOW: " + speed);
    }

    public void UnlockDash()
    {
        canDash = true;
    }

    public void UnlockDashInvincibility()
    {
        dashInvincible = true;
    }
}
