using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

// Flashes the post-processing vignette red when the player takes damage
[RequireComponent(typeof(Volume))]
public class DamageVignette : MonoBehaviour
{
    [SerializeField] private Color hurtColor = new Color(0.8f, 0f, 0f);   // dark red
    [SerializeField] private float hurtIntensity = 0.5f;                  // how strong the red edges get
    [SerializeField] private float fadeDuration = 0.4f;                   // seconds to fade back

    private Vignette vignette;
    private Color normalColor;
    private float normalIntensity;
    private Coroutine pulseRoutine;

    void Awake()
    {
        // ".profile" gives a runtime copy, so the saved profile file is never changed
        Volume volume = GetComponent<Volume>();
        if (volume.profile.TryGet(out vignette))
        {
            // Make sure these settings are active, then remember the normal look
            vignette.color.overrideState = true;
            vignette.intensity.overrideState = true;
            normalColor = vignette.color.value;
            normalIntensity = vignette.intensity.value;
        }
        else
        {
            Debug.LogWarning("DamageVignette: the Volume has no Vignette override.");
        }
    }

    // Called by PlayerHealth when the player is hit
    public void Pulse()
    {
        if (vignette == null) return;
        if (pulseRoutine != null) StopCoroutine(pulseRoutine);
        pulseRoutine = StartCoroutine(PulseRoutine());
    }

    private IEnumerator PulseRoutine()
    {
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeDuration;   // goes from 0 to 1

            // Start fully red and strong, blend back to the normal dark vignette
            vignette.color.value = Color.Lerp(hurtColor, normalColor, t);
            vignette.intensity.value = Mathf.Lerp(hurtIntensity, normalIntensity, t);

            yield return null;
        }

        vignette.color.value = normalColor;
        vignette.intensity.value = normalIntensity;
        pulseRoutine = null;
    }
}