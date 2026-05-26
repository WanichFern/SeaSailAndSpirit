using UnityEngine;

public class LandingMarker : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        // GetComponent only looks on THIS object, not children
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer == null)
            Debug.LogError("LandingMarker: No SpriteRenderer on this object!");

        Hide();
    }

    void Update()
    {
        if (spriteRenderer == null || !spriteRenderer.enabled) return;

        if (BoatController.Instance != null)
        {
            Vector3 boatPos = BoatController.Instance.transform.position;
            transform.position = new Vector3(
                boatPos.x,
                0.6f,
                boatPos.z
            );
        }
    }

    public void Show()
    {
        if (spriteRenderer != null)
            spriteRenderer.enabled = true;
    }

    public void Hide()
    {
        if (spriteRenderer != null)
            spriteRenderer.enabled = false;
    }
}