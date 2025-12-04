using UnityEngine;
using System.Collections;

public class DragonAI : MonoBehaviour
{
    [Header("References")]
    public Transform perchPoint;
    public Transform player;
    public GameObject fireballPrefab;
    public Collider2D teleportArea;

    [Header("Phase1 (Flying)")]
    public float flySpeed = 2f;
    public float fireRateP1 = 1.3f;
    public int phase1HP = 12;

    [Header("Phase2 (Perch Barrage)")]
    public float barrageRate = 0.28f;
    public float phase2BarrageDuration = 10f;
    public float phase2VulnerabilityTime = 3f;
    public int phase2HP = 12;

    [Header("Phase3 (FINAL)")]
    public float teleportInterval = 2f;
    public float aoeRadius = 2.5f;
    public float aoeKnockBack = 8;
    public int traceHitsRequired = 3;
    public float endVulnerabilityWait = 5f;

    Rigidbody2D rb;
    Animator anim;

    enum Phase { Entering, Phase1, Phase2, Phase3, Dead }
    Phase currentPhase = Phase.Entering;

    int currentHP;
    bool canBeHit = false;
    int traceHits = 0;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        rb.gravityScale = 0;
        rb.linearVelocity = Vector2.zero;

        transform.position = new Vector2(-12f, 1f);
        StartCoroutine(EnterScene());
    }

    IEnumerator EnterScene()
    {
        anim.SetBool("Move", true);

        while (Vector2.Distance(transform.position, player.position) > 4f)
        {
            Vector2 dir = (player.position - transform.position).normalized;
            rb.linearVelocity = dir * (flySpeed + 3);

            anim.SetFloat("Horizontal", dir.x);
            anim.SetFloat("Vertical", dir.y);

            yield return null;
        }

        rb.linearVelocity = Vector2.zero;
        anim.SetBool("Move", false);
        yield return new WaitForSeconds(0.4f);

        StartPhase1();
    }

    void StartPhase1()
    {
        currentPhase = Phase.Phase1;
        currentHP = phase1HP;
        canBeHit = true;
        StartCoroutine(Phase1Loop());
    }

    IEnumerator Phase1Loop()
    {
        while (currentPhase == Phase.Phase1)
        {
            Vector2 dir = (player.position - transform.position).normalized;
            rb.linearVelocity = dir * flySpeed;

            anim.SetBool("Move", true);
            anim.SetFloat("Horizontal", dir.x);
            anim.SetFloat("Vertical", dir.y);

            ShootFireball(dir);
            yield return new WaitForSeconds(fireRateP1);
        }
    }

    void ShootFireball(Vector2 dir)
    {
        // Force dragon to face DOWN while perched
        bool isPerched = (currentPhase == Phase.Phase2);

        if (isPerched)
            dir = Vector2.down;

        dir.Normalize();
        float safeDistance = 1.6f;
        Vector2 spawnPos = (Vector2)transform.position + dir * safeDistance;

        GameObject f = Instantiate(fireballPrefab, spawnPos, Quaternion.identity);
        DragonFireball fb = f.GetComponent<DragonFireball>();
        fb.Launch(dir);

        // ---- FIX ANIMATION ----
        if (dir.y < -0.5f)
            anim.SetTrigger("Attack_Down");
        else if (dir.y > 0.5f)
            anim.SetTrigger("Attack_Up");
        else if (dir.x > 0.5f)
            anim.SetTrigger("Attack_Right");
        else if (dir.x < -0.5f)
            anim.SetTrigger("Attack_Left");
        else
            anim.SetTrigger("Attack_Down"); // fallback
    }



    public void TakeProjectileHit(int dmg)
    {
        if (!canBeHit) return;

        currentHP -= dmg;
        StartCoroutine(DamageFlash());

        if (currentPhase == Phase.Phase1 && currentHP <= 0)
            StartCoroutine(EnterPhase2());

        else if (currentPhase == Phase.Phase2 && currentHP <= 0)
            StartCoroutine(EnterPhase3());
    }

    IEnumerator DamageFlash()
    {
        SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
        Color c = sr.color;

        sr.color = Color.white;
        yield return new WaitForSeconds(0.15f);
        sr.color = c;
    }

    IEnumerator EnterPhase2()
    {
        currentPhase = Phase.Phase2;
        canBeHit = false;

        rb.linearVelocity = Vector2.zero;
        anim.SetTrigger("LiftOff");
        yield return new WaitForSeconds(0.8f);

        transform.position = perchPoint.position;
        anim.SetBool("Move", false);
        currentHP = phase2HP;

        yield return new WaitForSeconds(0.5f);
        StartCoroutine(Phase2Loop());
    }

    IEnumerator Phase2Loop()
    {
        float t = 0;

        while (t < phase2BarrageDuration && currentPhase == Phase.Phase2)
        {
            Vector2 dir = (player.position - transform.position).normalized;
            ShootFireball(dir);

            t += barrageRate;
            yield return new WaitForSeconds(barrageRate);
        }

        // vulnerability window
        anim.SetTrigger("Landing");
        yield return new WaitForSeconds(1f);

        transform.position = player.position + Vector3.up;
        canBeHit = true;

        yield return new WaitForSeconds(phase2VulnerabilityTime);

        // go back to perch
        if (currentPhase == Phase.Phase2)
        {
            transform.position = perchPoint.position;
            canBeHit = false;
            currentHP = phase2HP;
            StartCoroutine(Phase2Loop());
        }
    }


    IEnumerator EnterPhase3()
    {
        currentPhase = Phase.Phase3;
        canBeHit = false;
        traceHits = 0;

        rb.linearVelocity = Vector2.zero;
        anim.SetBool("Move", false);

        yield return new WaitForSeconds(0.6f);
        StartCoroutine(Phase3Loop());
    }

    IEnumerator Phase3Loop()
    {
        while (currentPhase == Phase.Phase3)
        {
            TeleportRandomSpot();
            anim.SetTrigger("Attack");
            AOE();

            yield return new WaitForSeconds(endVulnerabilityWait);

            canBeHit = true;
            yield return new WaitForSeconds(1);
            canBeHit = false;
        }
    }

    void TeleportRandomSpot()
    {
        Bounds b = teleportArea.bounds;

        float x = Random.Range(b.min.x, b.max.x);
        float y = Random.Range(b.min.y, b.max.y);

        transform.position = new Vector2(x, y);
    }

    void AOE()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, aoeRadius);

        foreach (var h in hits)
        {
            if (h.CompareTag("Player"))
            {
                Vector2 dir = (h.transform.position - transform.position).normalized;
                h.attachedRigidbody.linearVelocity = dir * aoeKnockBack;
            }
        }
    }

    public void TraceHit()
    {
        if (currentPhase != Phase.Phase3) return;

        traceHits++;

        if (traceHits >= traceHitsRequired)
            KillDragon();
    }

    void KillDragon()
    {
        currentPhase = Phase.Dead;
        anim.SetTrigger("Die");
        Destroy(gameObject, 1.5f);
    }
}
