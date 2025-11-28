using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerShooting : MonoBehaviour
{
    public float projectileSpeed = 16f;

    [Header("Projectile Prefabs")]
    public GameObject redProjectile;
    public GameObject blueProjectile;
    public GameObject greenProjectile;
    public GameObject yellowProjectile;

    [Header("Cooldowns")]
    public float cooldownRed = 3f;
    public float cooldownBlue = 3f;
    public float cooldownGreen = 3f;
    public float cooldownYellow = 3f;

    private float cdR = 0;
    private float cdB = 0;
    private float cdG = 0;
    private float cdY = 0;

    private Camera cam;

    [Header("Upgrades")]
    public bool upgradeSpeed = false;
    public bool upgradeDamage = false;
    public bool upgradeSize = false;
    public bool upgradeScatter = false;
    public bool upgradeBounce = false;
    public bool upgradePierce = false;

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        // Reduce cooldowns over time
        if (cdR > 0) cdR -= Time.deltaTime;
        if (cdB > 0) cdB -= Time.deltaTime;
        if (cdG > 0) cdG -= Time.deltaTime;
        if (cdY > 0) cdY -= Time.deltaTime;

        var kb = Keyboard.current;

        if (kb.digit1Key.wasPressedThisFrame) TryShoot(redProjectile, ref cdR, cooldownRed);
        if (kb.digit2Key.wasPressedThisFrame) TryShoot(blueProjectile, ref cdB, cooldownBlue);
        if (kb.digit3Key.wasPressedThisFrame) TryShoot(greenProjectile, ref cdG, cooldownGreen);
        if (kb.digit4Key.wasPressedThisFrame) TryShoot(yellowProjectile, ref cdY, cooldownYellow);
    }

    void TryShoot(GameObject prefab, ref float cooldownTimer, float cooldownTime)
    {
        if (prefab == null) return;
        if (cooldownTimer > 0) return; // still cooling down

        cooldownTimer = cooldownTime;
        Shoot(prefab);
    }

    void Shoot(GameObject prefab)
    {
        Vector2 mouseWorld = cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector2 origin = transform.position;
        Vector2 dir = (mouseWorld - origin).normalized;

        if (upgradeScatter)
        {
            for (int i = -1; i <= 1; i++)
            {
                Vector2 angled = Quaternion.Euler(0, 0, i * 12) * dir;
                ShootSingle(prefab, angled.normalized);
            }
        }
        else
        {
            ShootSingle(prefab, dir);
        }
    }

    void ShootSingle(GameObject prefab, Vector2 direction)
    {
        Vector2 spawnPos = (Vector2)transform.position + direction * 0.8f;
        GameObject proj = Instantiate(prefab, spawnPos, Quaternion.identity);

        Projectile p = proj.GetComponent<Projectile>();
        p.direction = direction;
        p.speed = upgradeSpeed ? projectileSpeed * 1.4f : projectileSpeed;
        p.damage = upgradeDamage ? 2 : 1;
        p.bounce = upgradeBounce ? 3 : 0;
        p.pierce = upgradePierce ? 3 : 0;

        if (upgradeSize)
            proj.transform.localScale *= 1.6f;
    }
}
