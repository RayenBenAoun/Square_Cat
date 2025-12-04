using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DragonAI : MonoBehaviour
{
    [Header("References (Assigned At Runtime)")]
    public Transform perchPoint;
    public Transform player;
    public GameObject fireballPrefab;     // Prefab (assigned in Inspector)
    public Collider2D teleportArea;

    [Header("Minions To Spawn (For Phase 3)")]
    public GameObject[] possibleMinions;

    [Header("Phase 1 (Flying)")]
    public float flySpeed = 2f;
    public float fireRateP1 = 1.3f;
    public int phase1HP = 12;

    [Header("Phase 2 (Perch Barrage)")]
    public float barrageRate = 0.18f;
    public float barrageDuration = 8f;
    public float vulnerabilityWindow = 2.2f;
    public int phase2HP = 12;

    [Header("Phase 3 (Final)")]
    public int minionsToSpawn = 4;
    public float minionSpawnOffset = 2f;
    public float meleeSpeed = 3f;
    public float meleeRange = 1.3f;
    public float downedDuration = 4f;
    public int finalHitsRequired = 15;

    Rigidbody2D rb;
    Animator anim;

    enum Phase { Entering, Phase1, Phase2, Phase3Summon, Phase3Melee, Downed, Dead }
    Phase currentPhase = Phase.Entering;

    int currentHP;
    int finalHits;
    bool canBeHit = false;

    // --------------------------------------------------------
    // NEW: Allows spawner to assign scene references safely
    // --------------------------------------------------------
    public void Initialize(Transform perch, Transform playerTarget, Collider2D teleport)
    {
        perchPoint = perch;
        player = playerTarget;
        teleportArea = teleport;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        rb.gravityScale = 0;

        // --------------------------------------------------------
        // NEW: Auto-find references if not assigned by spawner
        // --------------------------------------------------------
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;

        if (perchPoint == null)
        {
            var perchObj = GameObject.Find("DragonPerch");
            if (perchObj != null) perchPoint = perchObj.transform;
        }

        if (teleportArea == null)
        {
            var teleObj = GameObject.Find("Dragon Teleport Area");
            if (teleObj != null) teleportArea = teleObj.GetComponent<Collider2D>();
        }
        // --------------------------------------------------------

        // Start offscreen
        transform.position = new Vector2(-12f, 1f);

        StartCoroutine(EnterScene());
    }

    // --------------------------------------------------------
    // ENTER SCENE
    // --------------------------------------------------------
    IEnumerator EnterScene()
    {
        anim.SetBool("Move", true);

        while (Vector2.Distance(transform.position, player.position) > 4f)
        {
            Vector2 dir = (player.position - transform.position).normalized;
            rb.linearVelocity = dir * (flySpeed + 3);
            SetDirection(dir);
            yield return null;
        }

        rb.linearVelocity = Vector2.zero;
        anim.SetBool("Move", false);
        yield return new WaitForSeconds(0.4f);

        StartPhase1();
    }

    // --------------------------------------------------------
    // PHASE 1
    // --------------------------------------------------------
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
            SetDirection(dir);

            ShootFireball(dir);

            yield return new WaitForSeconds(fireRateP1);
        }
    }

    // --------------------------------------------------------
    // FIREBALL
    // --------------------------------------------------------
    void ShootFireball(Vector2 dir)
    {
        GameObject f = Instantiate(fireballPrefab, transform.position, Quaternion.identity);

        Collider2D myCol = GetComponent<Collider2D>();
        Collider2D fCol = f.GetComponent<Collider2D>();
        if (myCol != null && fCol != null)
            Physics2D.IgnoreCollision(fCol, myCol);

        f.GetComponent<Rigidbody2D>().linearVelocity = dir * 7f;

        anim.SetTrigger("Attack");
    }

    // --------------------------------------------------------
    // DAMAGE
    // --------------------------------------------------------
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

    // --------------------------------------------------------
    // PHASE 2 — PERCH
    // --------------------------------------------------------
    IEnumerator EnterPhase2()
    {
        currentPhase = Phase.Phase2;
        canBeHit = false;
        rb.linearVelocity = Vector2.zero;

        anim.SetTrigger("LiftOff");
        yield return new WaitForSeconds(0.5f);

        transform.position = perchPoint.position;
        anim.SetBool("Move", false);

        currentHP = phase2HP;
        yield return new WaitForSeconds(0.5f);

        StartCoroutine(Phase2Loop());
    }

    IEnumerator Phase2Loop()
    {
        float t = 0;
        while (t < barrageDuration && currentPhase == Phase.Phase2)
        {
            Vector2 dir = (player.position - transform.position).normalized;
            ShootFireball(dir);
            t += barrageRate;
            yield return new WaitForSeconds(barrageRate);
        }

        anim.SetTrigger("Landing");
        yield return new WaitForSeconds(0.7f);

        transform.position = player.position + Vector3.up;
        canBeHit = true;

        yield return new WaitForSeconds(vulnerabilityWindow);

        if (currentPhase == Phase.Phase2)
        {
            canBeHit = false;
            transform.position = perchPoint.position;
            currentHP = phase2HP;
            StartCoroutine(Phase2Loop());
        }
    }

    // --------------------------------------------------------
    // PHASE 3 — FINAL
    // --------------------------------------------------------
    IEnumerator EnterPhase3()
    {
        currentPhase = Phase.Phase3Summon;
        rb.linearVelocity = Vector2.zero;

        anim.SetTrigger("LiftOff");
        yield return new WaitForSeconds(0.5f);

        transform.position = perchPoint.position;

        SpawnMinions();
    }

    void SpawnMinions()
    {
        for (int i = 0; i < minionsToSpawn; i++)
        {
            Vector2 pos = (Vector2)transform.position +
                          Vector2.down * minionSpawnOffset +
                          new Vector2(Random.Range(-1f, 1f), -7f);

            Instantiate(possibleMinions[Random.Range(0, possibleMinions.Length)], pos, Quaternion.identity);
        }
    }

    public void NotifyMinionKilled()
    {
        if (currentPhase != Phase.Phase3Summon) return;

        if (GameObject.FindGameObjectsWithTag("Enemy").Length <= 1)
            StartCoroutine(BeginPhase3Melee());
    }

    IEnumerator BeginPhase3Melee()
    {
        currentPhase = Phase.Phase3Melee;
        canBeHit = false;
        anim.SetBool("Move", true);

        while (currentPhase == Phase.Phase3Melee)
        {
            Vector2 dir = (player.position - transform.position).normalized;
            rb.linearVelocity = dir * meleeSpeed;
            SetDirection(dir);

            if (Vector2.Distance(transform.position, player.position) < meleeRange)
                anim.SetTrigger("Attack");

            yield return null;
        }
    }

    // --------------------------------------------------------
    // FINAL PHASE HITS
    // --------------------------------------------------------
    public void FinalPhaseHit()
    {
        if (currentPhase != Phase.Phase3Melee) return;

        finalHits++;
        if (finalHits >= finalHitsRequired)
            StartCoroutine(DownedState());
    }

    IEnumerator DownedState()
    {
        currentPhase = Phase.Downed;
        rb.linearVelocity = Vector2.zero;
        anim.SetBool("Move", false);

        yield return new WaitForSeconds(downedDuration);

        KillDragon();
    }

    void KillDragon()
    {
        currentPhase = Phase.Dead;
        rb.linearVelocity = Vector2.zero;
        anim.SetTrigger("Die");
        Destroy(gameObject, 2f);
    }

    // --------------------------------------------------------
    void SetDirection(Vector2 dir)
    {
        anim.SetFloat("Horizontal", dir.x);
        anim.SetFloat("Vertical", dir.y);
    }
}
