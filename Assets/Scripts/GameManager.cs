using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // One shared GameManager that any script can reach with GameManager.Instance
    public static GameManager Instance { get; private set; }

    public int Score { get; private set; }
    public bool IsGameOver { get; private set; }
    public bool IsNewHighscore { get; private set; }   // NEW: true if this run beat the best score

    void Awake()
    {
        // If a second GameManager ever appears, delete it so only one exists
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void AddScore(int amount)
    {
        if (IsGameOver) return;
        Score += amount;
        Debug.Log($"Score: {Score}");
    }

    public void GameOver()
    {
        if (IsGameOver) return;   // only run once
        IsGameOver = true;

        // NEW: save the score if it's the best so far
        IsNewHighscore = HighscoreManager.SubmitScore(Score);

        // NEW: play the game over sound
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopMusic();
            AudioManager.Instance.PlayGameOver();
        }


        Debug.Log($"GAME OVER! Final score: {Score}, best: {HighscoreManager.Highscore}");
    }

    void Update()
    {
        // Restart with R after dying
        if (IsGameOver && Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}