using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Shows score, flasks (health) and wave on screen, and the game-over message
public class HUD : MonoBehaviour
{
    [Header("Text fields")]
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text waveText;
    [SerializeField] private GameObject gameOverText;

    [Header("Hearts")]
    [SerializeField] private Image[] heartImages;   // one image per HP, left to right
    [SerializeField] private Sprite fullHeart;
    [SerializeField] private Sprite emptyHeart;     // optional: leave empty to dim lost hearts instead
    [SerializeField] private float popScale = 1.5f; // how big a lost heart grows before shrinking
    [SerializeField] private float popDuration = 0.25f;

    [Header("Game references")]
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private WaveSpawner waveSpawner;

    // Remember the last shown values, so the UI is only updated when something changes
    private int lastScore = -1;
    private int lastHealth = -1;
    private int lastWave = -1;

    void Start()
    {
        gameOverText.SetActive(false);   // hidden until the player dies
    }

    void Update()
    {
        GameManager gm = GameManager.Instance;
        if (gm == null) return;

        // Score
        if (gm.Score != lastScore)
        {
            lastScore = gm.Score;
            scoreText.text = $"SCORE:{lastScore}";
        }

        // Health (the player object is destroyed on death, so it becomes null)
        int health = (playerHealth != null) ? playerHealth.CurrentHealth : 0;
        if (health != lastHealth)
        {
            // Pop every heart that was just lost (not on the very first frame)
            if (lastHealth != -1 && health < lastHealth)
            {
                for (int i = health; i < lastHealth && i < heartImages.Length; i++)
                {
                    StartCoroutine(PopHeart(heartImages[i].rectTransform));
                }
            }

            lastHealth = health;
            UpdateHearts(health);
        }

        // Wave
        if (waveSpawner.CurrentWave != lastWave)
        {
            lastWave = waveSpawner.CurrentWave;
            waveText.text = $"WAVE {lastWave}";
        }

        // Game over message
        if (gm.IsGameOver && !gameOverText.activeSelf)
        {
            gameOverText.SetActive(true);
        }
    }

    void UpdateHearts(int health)
    {
        for (int i = 0; i < heartImages.Length; i++)
        {
            bool isFull = i < health;

            if (emptyHeart != null)
            {
                // Swap between full and empty heart sprites
                heartImages[i].sprite = isFull ? fullHeart : emptyHeart;
                heartImages[i].color = Color.white;
            }
            else
            {
                // No empty sprite: keep the full heart, but make lost ones faint
                heartImages[i].sprite = fullHeart;
                heartImages[i].color = isFull ? Color.white : new Color(1f, 1f, 1f, 0.25f);
            }
        }
    }

    // Grows the heart, then shrinks it back to normal size over popDuration seconds
    IEnumerator PopHeart(RectTransform heart)
    {
        float elapsed = 0f;
        while (elapsed < popDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / popDuration;               // goes from 0 to 1
            float scale = Mathf.Lerp(popScale, 1f, t);     // goes from 1.5 down to 1
            heart.localScale = new Vector3(scale, scale, 1f);
            yield return null;                             // wait one frame
        }
        heart.localScale = Vector3.one;
    }
}