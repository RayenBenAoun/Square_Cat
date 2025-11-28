using UnityEngine;

public class UpgradeMenuUI : MonoBehaviour
{
    [Header("Panels")]
    public GameObject InitialPrompt;        // "Press X to interact"
    public GameObject MainPanel;            // first menu with 4 big buttons
    public GameObject ProjectileTreePanel;  // panel that holds projectile nodes
    public GameObject DrawTreePanel;        // panel that holds draw nodes
    public GameObject SpeedTreePanel;       // panel that holds speed nodes

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
    }

    // Called by BlackSmithInteract when you press X
    public void OpenMainMenu()
    {
        if (InitialPrompt) InitialPrompt.SetActive(false);

        if (MainPanel) MainPanel.SetActive(true);
        if (ProjectileTreePanel) ProjectileTreePanel.SetActive(false);
        if (DrawTreePanel) DrawTreePanel.SetActive(false);
        if (SpeedTreePanel) SpeedTreePanel.SetActive(false);

        // Lock gameplay while in menu
        if (playerMovement) playerMovement.enabled = false;
        if (playerShooting) playerShooting.enabled = false;
        if (playerOutline) playerOutline.enabled = false;
    }

    // === PANEL SWITCHES (these are what the buttons should call) ===

  
    public void ShowProjectileTree()
    {
        Debug.Log("=== BUTTON CLICKED ===");

        MainPanel.SetActive(false);

        Debug.Log("Activating Projectile panel...");
        ProjectileTreePanel.SetActive(true);

        Debug.Log("ProjectileTreePanel active AFTER = " + ProjectileTreePanel.activeSelf);

        DrawTreePanel.SetActive(false);
        SpeedTreePanel.SetActive(false);
    }

    public void ShowDrawTree()
    {
        if (MainPanel) MainPanel.SetActive(false);
        if (ProjectileTreePanel) ProjectileTreePanel.SetActive(false);
        if (DrawTreePanel) DrawTreePanel.SetActive(true);
        if (SpeedTreePanel) SpeedTreePanel.SetActive(false);
    }

    public void ShowSpeedTree()
    {
        if (MainPanel) MainPanel.SetActive(false);
        if (ProjectileTreePanel) ProjectileTreePanel.SetActive(false);
        if (DrawTreePanel) DrawTreePanel.SetActive(false);
        if (SpeedTreePanel) SpeedTreePanel.SetActive(true);
    }

    // Back button on each tree panel
    public void BackToMain()
    {
        if (MainPanel) MainPanel.SetActive(true);
        if (ProjectileTreePanel) ProjectileTreePanel.SetActive(false);
        if (DrawTreePanel) DrawTreePanel.SetActive(false);
        if (SpeedTreePanel) SpeedTreePanel.SetActive(false);
    }

    // Exit button on main menu
    public void CloseAllMenus()
    {
        if (InitialPrompt) InitialPrompt.SetActive(false);
        if (MainPanel) MainPanel.SetActive(false);
        if (ProjectileTreePanel) ProjectileTreePanel.SetActive(false);
        if (DrawTreePanel) DrawTreePanel.SetActive(false);
        if (SpeedTreePanel) SpeedTreePanel.SetActive(false);

        // Re-enable gameplay
        if (playerMovement) playerMovement.enabled = true;
        if (playerShooting) playerShooting.enabled = true;
        if (playerOutline) playerOutline.enabled = true;
    }
}
