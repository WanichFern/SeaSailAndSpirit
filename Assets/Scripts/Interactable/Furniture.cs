using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Requirement
{
    public string itemName;
    public int amount;
}

[System.Serializable]
public class UpgradeLevel
{
    public List<Requirement> requirements;
    public StatType statType;
    public float bonusValue;
}

public class Furniture : MonoBehaviour, IInteractable
{
    public string furnitureName;
    public int currentLevel = 0;
    public List<UpgradeLevel> upgradeLevels;
    public GameObject upgradePanel;

    private Transform player;
    private float closeDistance = 3f;
    private bool isOpen = false;
    private float openTime = 0f;
    private float openDelay = 0.3f;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public void Interact()
    {
        Debug.Log($"Furniture.Interact() called on {furnitureName}");
        isOpen = true;
        openTime = Time.time;
        UpgradeUIManager.Instance.OpenUpgradeMenu(this);
    }

    public void ClosePanel()
    {
        isOpen = false;
    }

    public void PerformUpgrade()
    {
        if (currentLevel >= upgradeLevels.Count)
        {
            Debug.Log("Max Level!");
            return;
        }

        UpgradeLevel next = upgradeLevels[currentLevel];

        foreach (var req in next.requirements)
        {
            if (!ChestInventoryManager.Instance.HasEnoughItems(
                req.itemName, req.amount))
            {
                Debug.Log($"Not enough {req.itemName}");
                return;
            }
        }

        foreach (var req in next.requirements)
            ChestInventoryManager.Instance.ConsumeItems(
                req.itemName, req.amount);

        ApplyBonus(next);

        SaveManager.Instance?.SaveGame();
    }

    public void PerformFreeUpgrade()
    {
        if (currentLevel >= upgradeLevels.Count)
        {
            Debug.Log("Max Level!");
            return;
        }

        ApplyBonus(upgradeLevels[currentLevel]);

        SaveManager.Instance?.SaveGame();
    }

    void ApplyBonus(UpgradeLevel level)
    {
        PlayerStats playerStats = GameObject
            .FindGameObjectWithTag("Player")
            .GetComponent<PlayerStats>();

        if (playerStats == null)
        {
            Debug.LogError("PlayerStats not found on Player!");
            return;
        }

        switch (level.statType)
        {
            case StatType.MaxHP:
                playerStats.bonusMaxHP += level.bonusValue; break;
            case StatType.Defense:
                playerStats.bonusDefense += level.bonusValue; break;
            case StatType.SwordDamage:
                playerStats.bonusSwordDamage += level.bonusValue; break;
            case StatType.AxeDamage:
                playerStats.bonusAxeDamage += level.bonusValue; break;
            case StatType.PickaxeDamage:
                playerStats.bonusPickaxeDamage += level.bonusValue; break;
            case StatType.WalkSpeed:
                playerStats.bonusWalkSpeed += level.bonusValue; break;
            case StatType.InventoryCapacity:                          // ← new
                playerStats.bonusInventoryCapacity
                    += (int)level.bonusValue; break;
        }

        playerStats.CalculateStats();
        currentLevel++;
        Debug.Log($"{furnitureName} upgraded to level {currentLevel}!");
    }

    void Update()
    {
        if (!isOpen) return;
        if (Time.time < openTime + openDelay) return;

        if (Vector3.Distance(transform.position, player.position)
            > closeDistance)
        {
            isOpen = false;
            UpgradeUIManager.Instance.ClosePanel();
        }
    }
}