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
    public StatType statType;  // ← was string, now enum
    public float bonusValue;
}

public class Furniture : MonoBehaviour, IInteractable
{
    public string furnitureName;
    public int currentLevel = 0;
    public List<UpgradeLevel> upgradeLevels;
    public GameObject upgradePanel;
    public Transform itemContent;

    private Transform player;
    public float closeDistance = 3f;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public void Interact()
    {
        upgradePanel.SetActive(true);
        UpgradeUIManager.Instance.OpenUpgradeMenu(this);
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
            if (!ChestInventoryManager.Instance.HasEnoughItems(req.itemName, req.amount))
            {
                Debug.Log($"Not enough {req.itemName}");
                return;
            }
        }

        foreach (var req in next.requirements)
            ChestInventoryManager.Instance.ConsumeItems(req.itemName, req.amount);

        PlayerStats playerStats = GameObject
            .FindGameObjectWithTag("Player")
            .GetComponent<PlayerStats>();

        switch (next.statType)
        {
            case StatType.MaxHP:
                playerStats.bonusMaxHP += next.bonusValue;
                break;
            case StatType.Defense:
                playerStats.bonusDefense += next.bonusValue;
                break;
            case StatType.SwordDamage:
                playerStats.bonusSwordDamage += next.bonusValue;
                break;
            case StatType.AxeDamage:
                playerStats.bonusAxeDamage += next.bonusValue;
                break;
            case StatType.PickaxeDamage:
                playerStats.bonusPickaxeDamage += next.bonusValue;
                break;
            case StatType.WalkSpeed:
                playerStats.bonusWalkSpeed += next.bonusValue;
                break;
        }

        playerStats.CalculateStats();
        currentLevel++;
        Debug.Log($"{furnitureName} upgraded to level {currentLevel}!");
    }

    void Update()
    {
        if (upgradePanel.activeSelf &&
            Vector3.Distance(transform.position, player.position) > closeDistance)
        {
            upgradePanel.SetActive(false);
        }
    }
}