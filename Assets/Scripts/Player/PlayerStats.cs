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

    [Header("Total Stats (Used in Game)")]
    public float totalMaxHP;
    public float totalDefense;
    public float totalSwordDamage;
    public float totalAxeDamage;
    public float totalPickaxeDamage;
    public float totalWalkSpeed;
    public float attackCooldown; // ถ้าจะอัปเกรดความเร็วโจมตี ให้ทำเหมือนอันบน
    public float gatherCooldown;
    public int maxInventory;

    [Header("Live State")]
    public float currentHP;

    void Awake()
    {
        CalculateStats();
        currentHP = totalMaxHP;
    }

    public void CalculateStats()
    {
        if (playerBaseStats == null)
        {
            Debug.LogError("PlayerStats: playerBaseStats SO is not assigned!", this);
            return;
        }

        // คำนวณค่ารวมจาก Base (SO) + Bonus (Furniture)
        totalMaxHP = playerBaseStats.maxHP + bonusMaxHP;
        totalDefense = playerBaseStats.defense + bonusDefense;
        totalSwordDamage = playerBaseStats.swordDamage + bonusSwordDamage;
        totalAxeDamage = playerBaseStats.axeDamage + bonusAxeDamage;
        totalPickaxeDamage = playerBaseStats.pickaxeDamage + bonusPickaxeDamage;
        totalWalkSpeed = playerBaseStats.walkSpeed + bonusWalkSpeed;

        // ค่าที่ไม่เปลี่ยน (ดึงตรงจาก SO)
        attackCooldown = playerBaseStats.attackCooldown;
        gatherCooldown = playerBaseStats.gatherCooldown;
        maxInventory = playerBaseStats.inventoryCapacity;
    }

    public void TakeDamage(float amount)
    {
        float finalDamage = Mathf.Max(amount - totalDefense, 0);
        currentHP -= finalDamage;
        Debug.Log($"Player takes {finalDamage} damage. HP left: {currentHP}");

        if (currentHP <= 0) Respawn();
    }

    void Respawn()
    {
        Debug.Log("Respawning...");
        transform.position = respawnPoint.position;
        currentHP = totalMaxHP;
        InventoryManager.Instance.ClearInventoryOnDeath();

        BoatController.Instance?.RespawnAtDock();
    }
}