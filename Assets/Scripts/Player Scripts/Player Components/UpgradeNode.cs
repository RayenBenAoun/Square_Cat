using UnityEngine;

public class UpgradeNode : MonoBehaviour
{
    [TextArea]
    public string description;

    public enum UpgradeType
    {
        ProjectileSpeed,
        ProjectileDamage,
        ProjectileSize,
        ProjectileBounce,
        ProjectileScatter,
        ProjectilePierce,
        SpikedWalls,
        StunWalls,
        SpikeShot
    }

    public UpgradeType type;

    public void UnlockUpgrade()
    {
        PlayerShooting shooter = FindAnyObjectByType<PlayerShooting>();
        PlayerOutline outline = FindAnyObjectByType<PlayerOutline>();
        PlayerMovement movement = FindAnyObjectByType<PlayerMovement>();

        switch (type)
        {
            case UpgradeType.ProjectileSpeed:
                shooter.upgradeSpeed = true;
                shooter.projectileSpeed += 3f;
                Debug.Log("Unlocked: Projectile Speed");
                break;

            case UpgradeType.ProjectileDamage:
                shooter.upgradeDamage = true;
                Debug.Log("Unlocked: Projectile Damage");
                break;

            case UpgradeType.ProjectileSize:
                shooter.upgradeSize = true;
                Debug.Log("Unlocked: Projectile Size");
                break;

            case UpgradeType.ProjectileBounce:
                shooter.upgradeBounce = true;
                Debug.Log("Unlocked: Projectile Bounce");
                break;

            case UpgradeType.ProjectileScatter:
                shooter.upgradeScatter = true;
                Debug.Log("Unlocked: Projectile Scatter");
                break;

            case UpgradeType.ProjectilePierce:
                shooter.upgradePierce = true;
                Debug.Log("Unlocked: Projectile Pierce");
                break;

            case UpgradeType.SpikedWalls:
                outline.spikedWallsUpgrade = true;
                Debug.Log("Unlocked: Spiked Walls");
                break;

            case UpgradeType.StunWalls:
                outline.stunWallsUpgrade = true;
                Debug.Log("Unlocked: Stun Walls");
                break;

            case UpgradeType.SpikeShot:
                outline.spikeShotUpgrade = true;
                Debug.Log("Unlocked: Spike Shot");
                break;
        }
    }
}
