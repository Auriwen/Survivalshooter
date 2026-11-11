using UnityEngine;

// Limits the frame rate for the whole game.
// Runs automatically before any scene loads, so it needs no object in the scene.
public static class FrameRateSettings
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Init()
    {
        QualitySettings.vSyncCount = 0;    // don't let VSync decide
        Application.targetFrameRate = 60;  // draw at most 60 frames per second
    }
}