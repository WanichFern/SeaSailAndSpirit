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
    public GameObject upgradePanel; // keep for reference but don't toggle directly

    private Transform player;
    private float closeDistance = 2f;
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

    // Called by UpgradeUIManager when confirmed or ad completed
    public void ClosePanel()
    {
        isOpen = false;
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
        currentLevel++;
        Debug.Log($"{furnitureName} upgraded to level {currentLevel}!");

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
        currentLevel++;
        Debug.Log($"{furnitureName} free upgraded to level {currentLevel}!");

        SaveManager.Instance?.SaveGame();
    }

    // Extracted to avoid duplicating the switch in both upgrade methods
    void ApplyBonus(UpgradeLevel level)
    {
        PlayerStats playerStats = GameObject
            .FindGameObjectWithTag("Player")
            .GetComponent<PlayerStats>();

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
        }

        playerStats.CalculateStats();
    }
}