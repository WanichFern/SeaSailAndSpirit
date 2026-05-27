using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public CharacterStatsSO playerBaseStats;
    public Transform respawnPoint;

    [Header("Bonuses from Furniture")]
    public float bonusMaxHP;
    public float bonusDefense;
    public float bonusSwordDamage;
    public float bonusAxeDamage;
    public float bonusPickaxeDamage;
    public float bonusWalkSpeed;
    public int bonusInventoryCapacity;

    [Header("Total Stats (Used in Game)")]
    public float totalMaxHP;
    public float totalDefense;
    public float totalSwordDamage;
    public float totalAxeDamage;
    public float totalPickaxeDamage;
    public float totalWalkSpeed;
    public float attackCooldown;
    public float gatherCooldown;
    public float maxInventory;

    [Header("Live State")]
    public float currentHP;

    // Save where player died for revive
    private Vector3 deathPosition;

    void Awake()
    {
        CalculateStats();
        currentHP = totalMaxHP;
    }

    public void CalculateStats()
    {
        if (playerBaseStats == null)
        {
            Debug.LogError("PlayerStats: playerBaseStats SO not assigned!", this);
            return;
        }

        totalMaxHP = playerBaseStats.maxHP + bonusMaxHP;
        totalDefense = playerBaseStats.defense + bonusDefense;
        totalSwordDamage = playerBaseStats.swordDamage + bonusSwordDamage;
        totalAxeDamage = playerBaseStats.axeDamage + bonusAxeDamage;
        totalPickaxeDamage = playerBaseStats.pickaxeDamage + bonusPickaxeDamage;
        totalWalkSpeed = playerBaseStats.walkSpeed + bonusWalkSpeed;
        attackCooldown = playerBaseStats.attackCooldown;
        gatherCooldown = playerBaseStats.gatherCooldown;
        maxInventory = playerBaseStats.inventoryCapacity
        + bonusInventoryCapacity;
    }

    public void TakeDamage(float amount)
    {
        // Don't take damage while invincible
        if (InvincibilityHandler.Instance != null
            && InvincibilityHandler.Instance.IsInvincible)
        {
            Debug.Log("Damage blocked — player is invincible!");
            return;
        }

        float finalDamage = Mathf.Max(amount - totalDefense, 0);
        currentHP -= finalDamage;
        Debug.Log($"Player takes {finalDamage} damage. HP: {currentHP}");

        if (currentHP <= 0) Die();
    }


    void Die()
    {
        currentHP = 0;
        deathPosition = transform.position;

        // Activate invincibility immediately on death
        // so enemies can't keep damaging during the ad
        InvincibilityHandler.Instance?.ActivateInvincibility(60f);

        ReviveUI.Instance.Show(
            reviveAction: () =>
            {
                currentHP = totalMaxHP * 0.5f;
                transform.position = deathPosition;

                // Reset to normal 3 second invincibility after revive
                InvincibilityHandler.Instance?.ActivateInvincibility(3f);
                Debug.Log("Player revived!");
            },
            declineAction: () =>
            {
                Respawn();
            }
        );
    }

    void Respawn()
    {
        transform.position = respawnPoint.position;
        currentHP = totalMaxHP;
        InventoryManager.Instance.ClearInventoryOnDeath();
        BoatController.Instance?.RespawnAtDock();
        Debug.Log("Respawned at home.");
    }
}