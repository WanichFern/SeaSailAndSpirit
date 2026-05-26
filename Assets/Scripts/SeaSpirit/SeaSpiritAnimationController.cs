using UnityEngine;

public class SeaSpiritAnimationController : MonoBehaviour
{
    private Animator animator;
    private SeaSpiritAI seaspiritAI;
    private SpriteRenderer spriteRenderer;
    private Rigidbody rb;

    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        seaspiritAI = GetComponentInParent<SeaSpiritAI>();
        rb = GetComponentInParent<Rigidbody>();
    }

    public void SetLungeAnimation(bool value)
    {
        if (animator != null)
        {
            animator.SetBool("IsLunging", value);
        }
    }
}