using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 3;
    public int currentHealth;

    [Header("UI References")]
    [SerializeField] private UIHearts uiHearts;

    // These are no longer used but kept so your inspector doesn't freak out
    public GameObject deathScreen;
    public Transform respawnPoint;

    private bool isDead = false;

    void Awake()
    {
        if (uiHearts == null)
            uiHearts = FindFirstObjectByType<UIHearts>(FindObjectsInactive.Include);
    }

    void Start()
    {
        currentHealth = Mathf.Max(1, maxHealth);

        if (uiHearts != null)
        {
            uiHearts.Build(maxHealth);
            uiHearts.UpdateHearts(currentHealth, maxHealth);
        }

        // We DO NOT touch timeScale here anymore
        if (deathScreen != null)
            deathScreen.SetActive(false);
    }

    // =========================
    // DAMAGE
    // =========================
    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth = Mathf.Clamp(currentHealth - Mathf.Abs(amount), 0, maxHealth);

        if (uiHearts != null)
            uiHearts.UpdateHearts(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            DieAndRestartScene();
        }
    }

    // =========================
    // HEAL
    // =========================
    public void Heal(int amount)
    {
        if (isDead) return;

        currentHealth = Mathf.Clamp(currentHealth + Mathf.Abs(amount), 0, maxHealth);

        if (uiHearts != null)
            uiHearts.UpdateHearts(currentHealth, maxHealth);
    }

    // =========================
    // MAX HEALTH / UPGRADES
    // =========================
    public void SetMaxHealth(int newMax, bool keepRatio = false)
    {
        newMax = Mathf.Max(1, newMax);

        if (keepRatio)
        {
            float ratio = maxHealth > 0 ? (float)currentHealth / maxHealth : 1f;
            maxHealth = newMax;
            currentHealth = Mathf.Clamp(Mathf.RoundToInt(ratio * maxHealth), 0, maxHealth);
        }
        else
        {
            maxHealth = newMax;
            currentHealth = Mathf.Min(currentHealth, maxHealth);
        }

        if (uiHearts != null)
        {
            uiHearts.Build(maxHealth);
            uiHearts.UpdateHearts(currentHealth, maxHealth);
        }
    }

    // =========================
    // DEATH → RELOAD SCENE
    // =========================
    private void DieAndRestartScene()
    {
        isDead = true;

        // Just in case anything messed with timeScale
        Time.timeScale = 1f;

        // Reload the current scene from the beginning
        Scene current = SceneManager.GetActiveScene();
        SceneManager.LoadScene(current.buildIndex);
    }

    // Optional buttons still work if you use them in menus
    public void RestartGame()
    {
        Time.timeScale = 1f;
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.buildIndex);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("TitleScene");
    }
}
