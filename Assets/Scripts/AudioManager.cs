using UnityEngine;

// Plays the music and all sound effects.
// Other scripts call it with e.g. AudioManager.Instance.PlayShoot();
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio sources")]
    [SerializeField] private AudioSource musicSource;   // plays the looping music
    [SerializeField] private AudioSource sfxSource;     // plays the short sound effects

    [Header("Music")]
    [SerializeField] private AudioClip music;
    [SerializeField, Range(0f, 1f)] private float musicVolume = 0.4f;

    [Header("Sound effects")]
    [SerializeField] private AudioClip shoot;
    [SerializeField] private AudioClip enemyHit;
    [SerializeField] private AudioClip enemyDeath;
    [SerializeField] private AudioClip playerHurt;
    [SerializeField] private AudioClip gameOver;
    [SerializeField] private AudioClip buttonClick;

    [Header("Volumes (0 = silent, 1 = full)")]
    [SerializeField, Range(0f, 1f)] private float shootVolume = 0.35f;
    [SerializeField, Range(0f, 1f)] private float enemyHitVolume = 0.6f;
    [SerializeField, Range(0f, 1f)] private float enemyDeathVolume = 0.8f;
    [SerializeField, Range(0f, 1f)] private float playerHurtVolume = 0.9f;
    [SerializeField, Range(0f, 1f)] private float gameOverVolume = 1f;
    [SerializeField, Range(0f, 1f)] private float buttonClickVolume = 0.6f;

    void Awake()
    {
        // Only one AudioManager at a time
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        // Start the background music, looping forever
        if (music != null)
        {
            musicSource.clip = music;
            musicSource.loop = true;
            musicSource.volume = musicVolume;
            musicSource.Play();
        }
    }

    // --- One method per sound, so other scripts stay simple ---
    public void PlayShoot()       => PlaySfx(shoot, shootVolume, true);
    public void PlayEnemyHit()    => PlaySfx(enemyHit, enemyHitVolume, true);
    public void PlayEnemyDeath()  => PlaySfx(enemyDeath, enemyDeathVolume, true);
    public void PlayPlayerHurt()  => PlaySfx(playerHurt, playerHurtVolume, false);
    public void PlayGameOver()    => PlaySfx(gameOver, gameOverVolume, false);
    public void PlayButtonClick() => PlaySfx(buttonClick, buttonClickVolume, false);

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void PauseMusic()
    {
        musicSource.Pause();
    }
    public void ResumeMusic()
    {
        musicSource.UnPause();
    }
    void PlaySfx(AudioClip clip, float volume, bool randomPitch)
    {
        if (clip == null) return;   // missing sound: just stay silent instead of crashing

        // A small random pitch change stops repeated sounds from sounding robotic
        sfxSource.pitch = randomPitch ? Random.Range(0.9f, 1.1f) : 1f;

        // PlayOneShot lets several sounds overlap on the same source
        sfxSource.PlayOneShot(clip, volume);
    }
}