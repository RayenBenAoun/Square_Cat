using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSettings : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Animator animator;
    public GameObject pauseMenuUI;

    public GameObject pauseOverlay;

    private bool isPaused = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        animator.Play("SlideOutPause");
        // StartCoroutine(HideAfterAnimation());
        Time.timeScale = 1f;
        isPaused = false;
        pauseOverlay.SetActive(false);
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        animator.Play("SlideInPause");
        Time.timeScale = 0f;
        isPaused = true;
        pauseOverlay.SetActive(true);
    }

    IEnumerator HideAfterAnimation()
    {
        yield return new WaitForSecondsRealtime(0.3f);
        pauseMenuUI.SetActive(false);

    }

    public void ExitGame()
    {
        SceneManager.LoadSceneAsync(1);
    }

    public void Restart()
    {
        SceneManager.LoadSceneAsync(2);
    }
}
