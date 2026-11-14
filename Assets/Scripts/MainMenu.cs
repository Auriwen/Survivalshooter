using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

// Handles the main menu buttons and shows the highscore
public class MainMenu : MonoBehaviour
{
    [SerializeField] private string gameSceneName = "Game";
    [SerializeField] private TMP_Text highscoreText;

    void Start()
    {
        // Show the saved best score when the menu opens
        if (highscoreText != null)
        {
            highscoreText.text = $"BEST {HighscoreManager.Highscore}";
        }
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