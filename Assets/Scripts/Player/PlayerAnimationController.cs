using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    private Animator animator;
    private PlayerMovement playerMovement;
    private SpriteRenderer spriteRenderer;
    private Rigidbody rb;

    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerMovement = GetComponentInParent<PlayerMovement>();
        rb = GetComponentInParent<Rigidbody>(); // 3D Rigidbody on parent
    }

    void Update()
    {
        if (rb == null || playerMovement == null) return;

        // Use X and Z velocity since your game moves on the XZ plane
        float speed = new Vector2(rb.linearVelocity.x, rb.linearVelocity.z).magnitude;
        animator.SetFloat("Speed", speed);

        // Flip sprite based on facing direction
        if (Mathf.Abs(playerMovement.FacingDirection) > 0.1f)
            spriteRenderer.flipX = playerMovement.FacingDirection < 0;
    }

    public void TriggerAttack()
    {
        animator.SetBool("IsAttacking", true);
    }

    public void TriggerGather()
    {
        animator.SetBool("IsGathering", true);
    }
}