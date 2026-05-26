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
        rb = GetComponentInParent<Rigidbody>();
    }

    void Update()
    {
        if (rb == null || playerMovement == null) return;

        float speed = playerMovement.inputVector.magnitude;
        animator.SetFloat("Speed", speed);

        if (Mathf.Abs(playerMovement.FacingDirection) > 0.1f)
            spriteRenderer.flipX = playerMovement.FacingDirection < 0;
    }

    public void TriggerAttack()
    {
        animator.SetTrigger("IsAttacking");
    }

    public void TriggerGather()
    {
        animator.SetTrigger("IsGathering");
    }

    public void TriggerInteract()
    {
        animator.SetTrigger("IsInteracting");
    }
}