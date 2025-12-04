using UnityEngine;

public class UpgradeMenuUI : MonoBehaviour
{
    [Header("Panels")]
    public GameObject InitialPrompt;
    public GameObject MainPanel;
    public GameObject ProjectileTreePanel;
    public GameObject DrawTreePanel;
    public GameObject SpeedTreePanel;

    // ⭐ NEW HEART TREE PANEL
    public GameObject HeartsTreePanel;

    [Header("Player References")]
    public PlayerMovement playerMovement;
    public PlayerShooting playerShooting;
    public PlayerOutline playerOutline;

    void Start()
    {
        // Ensure everything starts hidden
        if (InitialPrompt) InitialPrompt.SetActive(false);
        if (MainPanel) MainPanel.SetActive(false);
        if (ProjectileTreePanel) ProjectileTreePanel.SetActive(false);
        if (DrawTreePanel) DrawTreePanel.SetActive(false);
        if (SpeedTreePanel) SpeedTreePanel.SetActive(false);

        // ⭐ NEW
        if (HeartsTreePanel) HeartsTreePanel.SetActive(false);
    }

    // Called by BlackSmithInteract when you press X
    public void OpenMainMenu()
    {
        if (InitialPrompt) InitialPrompt.SetActive(false);

        if (MainPanel) MainPanel.SetActive(true);
        if (ProjectileTreePanel) ProjectileTreePanel.SetActive(false);
        if (DrawTreePanel) DrawTreePanel.SetActive(false);
        if (SpeedTreePanel) SpeedTreePanel.SetActive(false);

        // ⭐ Hide Hearts panel when opening main menu
        if (HeartsTreePanel) HeartsTreePanel.SetActive(false);

        // Lock gameplay while in menu
        if (playerMovement) playerMovement.enabled = false;
        if (playerShooting) playerShooting.enabled = false;
        if (playerOutline) playerOutline.enabled = false;
    }

    // === PANEL SWITCHES ===

    public void ShowProjectileTree()
    {
        if (MainPanel) MainPanel.SetActive(false);
        if (ProjectileTreePanel) ProjectileTreePanel.SetActive(true);
        if (DrawTreePanel) DrawTreePanel.SetActive(false);
        if (SpeedTreePanel) SpeedTreePanel.SetActive(false);

        // ⭐ NEW
        if (HeartsTreePanel) HeartsTreePanel.SetActive(false);
    }

    public void ShowDrawTree()
    {
        if (MainPanel) MainPanel.SetActive(false);
        if (ProjectileTreePanel) ProjectileTreePanel.SetActive(false);
        if (DrawTreePanel) DrawTreePanel.SetActive(true);
        if (SpeedTreePanel) SpeedTreePanel.SetActive(false);

        // ⭐ NEW
        if (HeartsTreePanel) HeartsTreePanel.SetActive(false);
    }

    public void ShowSpeedTree()
    {
        if (MainPanel) MainPanel.SetActive(false);
        if (ProjectileTreePanel) ProjectileTreePanel.SetActive(false);
        if (DrawTreePanel) DrawTreePanel.SetActive(false);
        if (SpeedTreePanel) SpeedTreePanel.SetActive(true);

        // ⭐ NEW
        if (HeartsTreePanel) HeartsTreePanel.SetActive(false);
    }

    // ⭐ NEW HEART TREE BUTTON
    public void ShowHeartsTree()
    {
        if (MainPanel) MainPanel.SetActive(false);
        if (ProjectileTreePanel) ProjectileTreePanel.SetActive(false);
        if (DrawTreePanel) DrawTreePanel.SetActive(false);
        if (SpeedTreePanel) SpeedTreePanel.SetActive(false);

        if (HeartsTreePanel) HeartsTreePanel.SetActive(true);
    }

    // Back button on each tree panel
    public void BackToMain()
    {
        if (MainPanel) MainPanel.SetActive(true);
        if (ProjectileTreePanel) ProjectileTreePanel.SetActive(false);
        if (DrawTreePanel) DrawTreePanel.SetActive(false);
        if (SpeedTreePanel) SpeedTreePanel.SetActive(false);

        // ⭐ NEW
        if (HeartsTreePanel) HeartsTreePanel.SetActive(false);
    }

    // Exit button on main menu
    public void CloseAllMenus()
    {
        if (InitialPrompt) InitialPrompt.SetActive(false);
        if (MainPanel) MainPanel.SetActive(false);
        if (ProjectileTreePanel) ProjectileTreePanel.SetActive(false);
        if (DrawTreePanel) DrawTreePanel.SetActive(false);
        if (SpeedTreePanel) SpeedTreePanel.SetActive(false);

        // ⭐ NEW
        if (HeartsTreePanel) HeartsTreePanel.SetActive(false);

        // Re-enable gameplay
        if (playerMovement) playerMovement.enabled = true;
        if (playerShooting) playerShooting.enabled = true;
        if (playerOutline) playerOutline.enabled = true;
    }
}
