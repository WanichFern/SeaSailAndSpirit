using Mono.Cecil;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    public float interactRange = 2f;
    private PlayerInputActions controls;
    private PlayerStats pStats;
    public static PlayerInteraction Instance;
    private PlayerAnimationController animController;

    // --- เพิ่มตัวจับเวลา Cooldown ---
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
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, interactRange);
        foreach (var hitCollider in hitColliders)
        {
            EnemyAI enemy = hitCollider.GetComponent<EnemyAI>();
            if (enemy != null)
            {
                // เช็ค Cooldown การโจมตี
                if (Time.time >= lastAttackTime + pStats.attackCooldown)
                {
                    enemy.TakeDamage(pStats.totalSwordDamage);
                    lastAttackTime = Time.time;
                    animController?.TriggerAttack();
                    Debug.Log("Hit Enemy!");
                }
                return; // ตีโดนแล้ว จบการทำงาน ไม่ไปตีตัวอื่นต่อ
            }

            ResourceNode resource = hitCollider.GetComponent<ResourceNode>();
            if (resource != null)
            {
                // เช็ค Cooldown การขุด/ตัด
                if (Time.time >= lastGatherTime + pStats.gatherCooldown)
                {
                    // 1. ตรวจสอบประเภทเพื่อดึงค่าดาเมจให้ถูกต้อง
                    float damageToDeal = 0f;
                    if (resource.resourceType == ResourceType.Wood)
                    {
                        damageToDeal = pStats.totalAxeDamage;
                    }
                    else if (resource.resourceType == ResourceType.Stone)
                    {
                        damageToDeal = pStats.totalPickaxeDamage;
                    }

                    // 2. ส่งค่าดาเมจไปให้ต้นไม้/หิน
                    resource.TakeHit(damageToDeal);
                    lastGatherTime = Time.time;
                    animController?.TriggerGather();
                }
                return;
            }

            // ... (ส่วนการเก็บของ และ HouseDoor ปล่อยไว้เหมือนเดิมได้เลย เพราะไม่มี Cooldown) ...

            CollectibleItem item = hitCollider.GetComponent<CollectibleItem>();
            if (item != null)
            {
                bool isPickedUp = InventoryManager.Instance.AddItem(item.itemName, 1);
                if (isPickedUp) Destroy(hitCollider.gameObject);
                return;
            }

            HouseDoor door = hitCollider.GetComponent<HouseDoor>();
            if (door != null)
            {
                door.Interact();
                return;
            }

            Chest chest = hitCollider.GetComponent<Chest>();
            if (chest != null)
            {
                chest.Interact();
                return;
            }

            Furniture furniture = hitCollider.GetComponent<Furniture>();
            if (furniture != null)
            {
                furniture.Interact();
                return;
            }
        }
    }
}