using UnityEngine;

// Saves and loads the best score, so it survives closing the game.
// "static" means there's only one, shared by everything; it needs no object in the scene.
public static class HighscoreManager
{
    private const string Key = "Highscore";   // the name the value is saved under

    // The saved best score (0 if nothing has been saved yet)
    public static int Highscore => PlayerPrefs.GetInt(Key, 0);

    // Saves the score if it beats the old best. Returns true if it's a new highscore.
    public static bool SubmitScore(int score)
    {
        if (score <= Highscore) return false;

        PlayerPrefs.SetInt(Key, score);
        PlayerPrefs.Save();                   // write to disk right away
        return true;
    }
}