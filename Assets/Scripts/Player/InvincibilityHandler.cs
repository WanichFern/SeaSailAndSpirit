using UnityEngine;
using System.Collections;

public class InvincibilityHandler : MonoBehaviour
{
    public static InvincibilityHandler Instance;

    [Header("Settings")]
    public float invincibilityDuration = 3f;
    public float blinkInterval = 0.15f;
    public Color shieldColor = new Color(0.3f, 0.6f, 1f, 1f); // blue tint

    private bool isInvincible = false;
    private SpriteRenderer spriteRenderer;

    public bool IsInvincible => isInvincible;

    void Awake()
    {
        Instance = this;
        // Get sprite renderer from Player_Visual child
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    public void ActivateInvincibility(float duration = -1f)
    {
        if (isInvincible)
            StopAllCoroutines();

        // If no duration specified, use the default
        // Pass a long duration for death invincibility
        float actualDuration = duration > 0
            ? duration
            : invincibilityDuration;

        StartCoroutine(InvincibilityRoutine(actualDuration));
    }

    IEnumerator InvincibilityRoutine(float duration)
    {
        isInvincible = true;
        float elapsed = 0f;
        Color originalColor = spriteRenderer.color;

        while (elapsed < duration)
        {
            spriteRenderer.color = shieldColor;
            yield return new WaitForSeconds(blinkInterval);
            spriteRenderer.color = originalColor;
            yield return new WaitForSeconds(blinkInterval);
            elapsed += blinkInterval * 2f;
        }

        spriteRenderer.color = originalColor;
        isInvincible = false;
        Debug.Log("Invincibility ended.");
    }
}