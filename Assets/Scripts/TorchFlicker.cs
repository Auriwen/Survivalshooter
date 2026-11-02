using UnityEngine;
using UnityEngine.Rendering.Universal;

// Makes a 2D light flicker like a real flame
[RequireComponent(typeof(Light2D))]
public class TorchFlicker : MonoBehaviour
{
    [SerializeField] private float baseIntensity = 1f;   // average brightness
    [SerializeField] private float flickerAmount = 0.2f; // how much it varies (+/-)
    [SerializeField] private float flickerSpeed = 3f;    // how fast it changes

    private Light2D light2D;
    private float seed;

    void Awake()
    {
        light2D = GetComponent<Light2D>();

        // A random starting point, so each torch flickers differently
        seed = Random.value * 100f;
    }

    void Update()
    {
        // Perlin noise gives smooth random values between 0 and 1
        float noise = Mathf.PerlinNoise(seed, Time.time * flickerSpeed);

        // Turn 0..1 into -1..1, then scale it by the flicker amount
        float offset = (noise - 0.5f) * 2f * flickerAmount;

        light2D.intensity = baseIntensity + offset;
    }
}