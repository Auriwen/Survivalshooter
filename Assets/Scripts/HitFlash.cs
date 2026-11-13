using System.Collections;
using UnityEngine;

// Makes this sprite flash white for a moment, using the HitFlash shader.
// Each enemy flashes on its own, even though they all share one material.
[RequireComponent(typeof(SpriteRenderer))]
public class HitFlash : MonoBehaviour
{
    [SerializeField] private float flashDuration = 0.1f;     // how long the flash fades out
    [SerializeField] private Color flashColor = Color.white;

    // Look up the shader property names once (faster than using the text every time)
    private static readonly int FlashAmountId = Shader.PropertyToID("_FlashAmount");
    private static readonly int FlashColorId = Shader.PropertyToID("_FlashColor");

    private SpriteRenderer spriteRenderer;
    private MaterialPropertyBlock propertyBlock;
    private Coroutine flashRoutine;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        propertyBlock = new MaterialPropertyBlock();
    }

    // Called by Enemy when it takes damage
    public void Flash()
    {
        // If already flashing, restart from full white
        if (flashRoutine != null) StopCoroutine(flashRoutine);
        flashRoutine = StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        float elapsed = 0f;
        while (elapsed < flashDuration)
        {
            elapsed += Time.deltaTime;
            float amount = 1f - (elapsed / flashDuration);   // goes from 1 down to 0
            SetFlashAmount(amount);
            yield return null;                                // wait one frame
        }
        SetFlashAmount(0f);
        flashRoutine = null;
    }

    private void SetFlashAmount(float amount)
    {
        // Read the current settings first, so we keep the sprite image the renderer sets itself
        spriteRenderer.GetPropertyBlock(propertyBlock);
        propertyBlock.SetFloat(FlashAmountId, amount);
        propertyBlock.SetColor(FlashColorId, flashColor);
        spriteRenderer.SetPropertyBlock(propertyBlock);
    }

    
}