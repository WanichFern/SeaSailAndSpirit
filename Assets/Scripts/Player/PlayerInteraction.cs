using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    public float interactRange = 2f;
    private PlayerInputActions controls;
    private PlayerStats pStats;
    public static PlayerInteraction Instance;
    private PlayerAnimationController animController;

    private float lastAttackTime = 0f;
    private float lastGatherTime = 0f;

    void Awake() { Instance = this; }

    void Start()
    {
        pStats = GetComponent<PlayerStats>();
        animController = GetComponentInChildren<PlayerAnimationController>();
    }

    void InitInput()
    {
        if (controls == null)
        {
            controls = new PlayerInputActions();
            controls.Player.Interact.performed += ctx => InteractWithNearby();
        }
    }

    void OnEnable()
    {
        InitInput();
        controls.Enable();
    }

    void OnDisable()
    {
        controls?.Disable();
    }

    void InteractWithNearby()
    {
        Collider[] hitColliders = Physics.OverlapSphere(
            transform.position, interactRange);

        foreach (var hitCollider in hitColliders)
        {
            IDamageable damageable = hitCollider.GetComponent<IDamageable>();
            if (damageable != null)
            {
                EnemyAI enemy = hitCollider.GetComponent<EnemyAI>();
                if (enemy != null)
                {
                    if (Time.time >= lastAttackTime + pStats.attackCooldown)
                    {
                        damageable.TakeDamage(pStats.totalSwordDamage);
                        lastAttackTime = Time.time;
                        animController?.TriggerAttack();
                        Debug.Log("Hit Enemy!");
                    }
                    return;
                }

                ResourceNode resource = hitCollider.GetComponent<ResourceNode>();
                if (resource != null)
                {
                    if (Time.time >= lastGatherTime + pStats.gatherCooldown)
                    {
                        float damage = resource.resourceType == ResourceType.Wood
                            ? pStats.totalAxeDamage
                            : pStats.totalPickaxeDamage;

                        damageable.TakeDamage(damage);
                        lastGatherTime = Time.time;
                        animController?.TriggerGather();
                    }
                    return;
                }
            }

            IInteractable interactable = hitCollider.GetComponent<IInteractable>();
            if (interactable != null)
            {
                interactable.Interact();
                animController?.TriggerInteract();
                return;
            }
        }
    }
}