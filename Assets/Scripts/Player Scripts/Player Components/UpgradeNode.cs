using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class UpgradeNode : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Visual UI References")]
    public Button button;
    public Image icon;
    public TMP_Text label;
    public TMP_Text costLabel;

    [Header("Upgrade Chain")]
    [Tooltip("Leave null for the first node in a chain.")]
    public UpgradeNode previousNode;
    public int cost = 1;
    public bool purchased = false;

    [Header("Tooltip")]
    public GameObject tooltipObject;
    public TMP_Text tooltipText;
    [TextArea]
    public string upgradeDescription;

    [Header("Trace / Wall Upgrades")]
    public bool unlockDuration;
    public bool unlockSpikeWalls;
    public bool unlockSpikeShot;
    public bool unlockStunWalls;

    [Header("Projectile Upgrades")]
    public bool projectileDamage;
    public bool projectileSize;
    public bool projectileScatter;
    public bool projectileBounce;
    public bool projectilePierce;
    public bool projectileSpeed;

    [Header("Speed Upgrades")]
    public bool increaseSpeed;       // +10% movement speed
    public bool unlockDash;
    public bool unlockDoubleDash;

    // ⭐⭐⭐ NEW HEART / REVIVE SYSTEM ⭐⭐⭐
    [Header("Heart Upgrades")]
    public bool heartUpgrade;        // adds +1 max health

    [Header("Revive Upgrade")]
    public bool reviveUpgrade;       // allows mid-fight revive (costs 10 currency)

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip purchasedSFX;
    public AudioClip cantAffordSFX;

    private void Start()
    {
        if (button != null)
            button.onClick.AddListener(OnClickUpgrade);

        RefreshState();
    }

    private void OnEnable()
    {
        RefreshState();
    }

    // =========================
    // STATE / UI REFRESH
    // =========================
    public void RefreshState()
    {
        if (costLabel != null)
            costLabel.text = cost.ToString();

        if (purchased)
        {
            if (button != null) button.interactable = false;

            if (icon != null) icon.color = Color.green;
            if (label != null) label.color = Color.green;
            return;
        }

        bool unlocked = (previousNode == null || previousNode.purchased);

        if (button != null)
            button.interactable = unlocked;

        float alpha = unlocked ? 1f : 0.35f;

        if (icon != null)
            icon.color = new Color(1f, 1f, 1f, alpha);

        if (label != null)
            label.color = new Color(label.color.r, label.color.g, label.color.b, alpha);
    }

    // =========================
    // CLICK HANDLER
    // =========================
    private void OnClickUpgrade()
    {
        if (purchased)
            return;

        if (previousNode != null && !previousNode.purchased)
        {
            PlayCantAffordSound();
            return;
        }

        if (PlayerCurrency.Instance == null)
        {
            Debug.LogWarning("PlayerCurrency.Instance is missing in scene.");
            return;
        }

        if (!PlayerCurrency.Instance.Spend(cost))
        {
            PlayCantAffordSound();
            return;
        }

        purchased = true;
        ApplyUpgradeEffects();
        PlayPurchaseSound();

        RefreshState();

        UpgradeNode[] allNodes = FindObjectsOfType<UpgradeNode>(true);
        foreach (var n in allNodes)
        {
            if (n == null) continue;
            if (n.previousNode == this)
                n.RefreshState();
        }
    }

    // =========================
    // APPLY GAMEPLAY EFFECTS
    // =========================
    private void ApplyUpgradeEffects()
    {
        PlayerOutline trace = FindAnyObjectByType<PlayerOutline>();
        PlayerShooting shoot = FindAnyObjectByType<PlayerShooting>();
        PlayerMovement move = FindAnyObjectByType<PlayerMovement>();
        PlayerHealth ph = FindAnyObjectByType<PlayerHealth>();   // ⭐ NEW

        // Trace / wall
        if (trace != null)
        {
            if (unlockDuration) trace.durationUpgrade = true;
            if (unlockSpikeWalls) trace.spikedWallsUpgrade = true;
            if (unlockSpikeShot) trace.spikeShotUpgrade = true;
            if (unlockStunWalls) trace.stunWallsUpgrade = true;
        }

        // Projectile
        if (shoot != null)
        {
            if (projectileDamage) shoot.upgradeDamage = true;
            if (projectileSize) shoot.upgradeSize = true;
            if (projectileScatter) shoot.upgradeScatter = true;
            if (projectileBounce) shoot.upgradeBounce = true;
            if (projectilePierce) shoot.upgradePierce = true;
            if (projectileSpeed) shoot.upgradeSpeed = true;
        }

        // Speed
        if (move != null)
        {
            if (increaseSpeed)
                move.IncreaseSpeed(move.speed * 0.10f);

            if (unlockDash)
                move.UnlockDash();

            if (unlockDoubleDash)
                move.UnlockDoubleDash();
        }
       
    }

    // =========================
    // AUDIO HELPERS
    // =========================
    private void PlayPurchaseSound()
    {
        if (audioSource != null && purchasedSFX != null)
            audioSource.PlayOneShot(purchasedSFX);
    }

    private void PlayCantAffordSound()
    {
        if (audioSource != null && cantAffordSFX != null)
            audioSource.PlayOneShot(cantAffordSFX);
    }

    // =========================
    // TOOLTIP HANDLERS
    // =========================
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (tooltipObject != null && tooltipText != null)
        {
            tooltipObject.SetActive(true);
            tooltipText.text = upgradeDescription;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (tooltipObject != null)
            tooltipObject.SetActive(false);
    }
}
