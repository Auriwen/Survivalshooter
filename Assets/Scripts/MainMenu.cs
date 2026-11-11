
using UnityEngine;
using UnityEngine.SceneManagement;

// Handles the main menu buttons
public class MainMenu : MonoBehaviour
{
    [SerializeField] private string gameSceneName = "Game";

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