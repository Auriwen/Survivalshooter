using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

// Handles the main menu buttons and shows the highscore
public class MainMenu : MonoBehaviour
{
    [SerializeField] private string gameSceneName = "Game";
    [SerializeField] private TMP_Text highscoreText;
    [SerializeField] private GameObject exitButton;

    void Start()
    {
        // Show the saved best score when the menu opens
        if (highscoreText != null)
        {
            highscoreText.text = $"BEST {HighscoreManager.Highscore}";
        }

        // A web page can't close itself, so hide EXIT in the browser version
#if UNITY_WEBGL && !UNITY_EDITOR
        if (exitButton != null) exitButton.SetActive(false);
#endif
    }

    // Called by the Play button
    public void PlayGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    // Called by the Exit button
    public void ExitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}