using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // One shared GameManager that any script can reach with GameManager.Instance
    public static GameManager Instance { get; private set; }

    public int Score { get; private set; }
    public bool IsGameOver { get; private set; }

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
        Debug.Log($"GAME OVER! Final score: {Score}. Press R to restart.");
    }

    void Update()
    {
        // Temporary restart key until we build the game-over screen
        if (IsGameOver && Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}