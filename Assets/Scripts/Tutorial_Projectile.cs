using UnityEngine;

public class ProjectileTutorialTrigger : MonoBehaviour
{
    public GameObject popup;

    private bool triggered = false;

    private bool pressed1 = false;
    private bool pressed2 = false;
    private bool pressed3 = false;
    private bool pressed4 = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !triggered)
        {
            popup.SetActive(true);
            triggered = true;
        }
    }

    private void Update()
    {
        if (!triggered) return;

        // Track key presses
        if (Input.GetKeyDown(KeyCode.Alpha1)) pressed1 = true;
        if (Input.GetKeyDown(KeyCode.Alpha2)) pressed2 = true;
        if (Input.GetKeyDown(KeyCode.Alpha3)) pressed3 = true;
        if (Input.GetKeyDown(KeyCode.Alpha4)) pressed4 = true;

        // When all four are pressed at least once
        if (pressed1 && pressed2 && pressed3 && pressed4)
        {
            popup.SetActive(false);
            gameObject.SetActive(false); // optional: remove trigger forever
        }
    }
}
