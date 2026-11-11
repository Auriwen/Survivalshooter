using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

// Handles the main menu buttons and shows the highscore
public class MainMenu : MonoBehaviour
{
    [SerializeField] private string gameSceneName = "Game";
    [SerializeField] private TMP_Text highscoreText;   // NEW

    // NEW: show the saved best score when the menu opens
    void Start()
    {
        if (highscoreText != null)
        {
            highscoreText.text = $"Highest score: {HighscoreManager.Highscore}";
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
        Debug.Log("Exit pressed");
        Application.Quit();   // closes the built game

#if UNITY_EDITOR
        // Application.Quit does nothing inside the editor, so stop Play mode instead
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}