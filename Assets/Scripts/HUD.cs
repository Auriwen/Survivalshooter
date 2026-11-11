using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Shows score, flasks (health) and wave on screen, and the game-over screen
public class HUD : MonoBehaviour
{
    [Header("Text fields")]
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text waveText;
    [SerializeField] private GameObject gameOverText;   // the GameOverGroup

    [Header("Game over details")]                         // NEW
    [SerializeField] private TMP_Text finalScoreText;     // NEW
    [SerializeField] private TMP_Text bestText;           // NEW
    [SerializeField] private GameObject newHighscoreText; // NEW

    [Header("Hearts")]
    [SerializeField] private Image[] heartImages;   // one image per HP, left to right
    [SerializeField] private Sprite fullHeart;
    [SerializeField] private Sprite emptyHeart;     // optional: leave empty to dim lost hearts instead

    [Header("Coin")]
    [SerializeField] private RectTransform coinIcon; // pops when the score goes up

    [Header("Pop effect")]
    [SerializeField] private float popScale = 1.5f;
    [SerializeField] private float popDuration = 0.25f;

    [Header("Game references")]
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private WaveSpawner waveSpawner;

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
            if (lastScore != -1 && gm.Score > lastScore && coinIcon != null)
            {
                StartCoroutine(Pop(coinIcon));
            }
            lastScore = gm.Score;
            scoreText.text = $"{lastScore:D5}"; // 5 digits, leading zeros
        }

        // Health
        int health = (playerHealth != null) ? playerHealth.CurrentHealth : 0;
        if (health != lastHealth)
        {
            if (lastHealth != -1 && health < lastHealth)
            {
                for (int i = health; i < lastHealth && i < heartImages.Length; i++)
                {
                    StartCoroutine(Pop(heartImages[i].rectTransform));
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

        // Game over screen (runs once, the moment the player dies)
        if (gm.IsGameOver && !gameOverText.activeSelf)
        {
            ShowGameOver(gm);   // CHANGED: now also fills in the scores
        }
    }

    // NEW: fill in the numbers, then show the whole game over group
    void ShowGameOver(GameManager gm)
    {
        finalScoreText.text = $"SCORE {gm.Score}";
        bestText.text = $"BEST {HighscoreManager.Highscore}";
        newHighscoreText.SetActive(gm.IsNewHighscore);   // only visible if you beat the record
        gameOverText.SetActive(true);
    }

    void UpdateHearts(int health)
    {
        for (int i = 0; i < heartImages.Length; i++)
        {
            bool isFull = i < health;

            if (emptyHeart != null)
            {
                heartImages[i].sprite = isFull ? fullHeart : emptyHeart;
                heartImages[i].color = Color.white;
            }
            else
            {
                heartImages[i].sprite = fullHeart;
                heartImages[i].color = isFull ? Color.white : new Color(1f, 1f, 1f, 0.25f);
            }
        }
    }

    // Grows the icon, then shrinks it back to normal size over popDuration seconds
    IEnumerator Pop(RectTransform icon)
    {
        float elapsed = 0f;
        while (elapsed < popDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / popDuration;
            float scale = Mathf.Lerp(popScale, 1f, t);
            icon.localScale = new Vector3(scale, scale, 1f);
            yield return null;
        }
        icon.localScale = Vector3.one;
    }
}