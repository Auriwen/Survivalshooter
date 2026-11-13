using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

// Pauses the game with Esc and handles the pause menu buttons
public class PauseMenu : MonoBehaviour
{
    // Other scripts (like the player) can check this to ignore input while paused
    public static bool IsPaused { get; private set; }

    [SerializeField] private GameObject pausePanel;
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    void Start()
    {
        // Always start unpaused, even after coming back from the main menu
        IsPaused = false;
        Time.timeScale = 1f;
        pausePanel.SetActive(false);
    }

    void Update()
    {
        if (Keyboard.current == null) return;

        // No pausing after the player died
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver) return;

        // Esc (or P) toggles the pause
        if (Keyboard.current.escapeKey.wasPressedThisFrame || Keyboard.current.pKey.wasPressedThisFrame)
        {
            if (IsPaused) Resume();
            else Pause();
        }
    }

    public void Pause()
    {
        IsPaused = true;
        Time.timeScale = 0f;          // freezes physics, movement, timers and coroutines
        pausePanel.SetActive(true);

        // NEW: pause the music too
        if (AudioManager.Instance != null) AudioManager.Instance.PauseMusic();
    }

    // Called by the Resume button
    public void Resume()
    {
        IsPaused = false;
        Time.timeScale = 1f;          // back to normal speed
        pausePanel.SetActive(false);

        // NEW: resume the music too
        if (AudioManager.Instance != null) AudioManager.Instance.ResumeMusic();
    }

    // Called by the Main Menu button
    public void QuitToMainMenu()
    {
        // Reset the time scale first, otherwise the next scene would start frozen
        IsPaused = false;
        Time.timeScale = 1f;
        // NEW: stop the music too
        if (AudioManager.Instance != null) AudioManager.Instance.StopMusic();
        SceneManager.LoadScene(mainMenuSceneName);
    }
}