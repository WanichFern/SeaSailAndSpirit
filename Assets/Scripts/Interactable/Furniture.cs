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
    public string statType;
    public float bonusValue;
}

public class Furniture : MonoBehaviour
{
    public string furnitureName;
    public int currentLevel = 0;
    public List<UpgradeLevel> upgradeLevels;
    public GameObject upgradePanel;
    public Transform itemContent;

    private Transform player;
    public float closeDistance = 3f;

    void Start() { player = GameObject.FindGameObjectWithTag("Player").transform; }

    public void Interact()
    {
        upgradePanel.SetActive(true);

        UpgradeUIManager.Instance.OpenUpgradeMenu(this);
    }

    public void PerformUpgrade()
    {
        if (currentLevel >= upgradeLevels.Count) { Debug.Log("Max Level!"); return; }

        UpgradeLevel next = upgradeLevels[currentLevel];

        // 1. ตรวจสอบของ
        foreach (var req in next.requirements)
        {
            if (!ChestInventoryManager.Instance.HasEnoughItems(req.itemName, req.amount))
            {
                Debug.Log($"not enough {req.itemName}"); return;
            }
        }

        // 2. หักของ
        foreach (var req in next.requirements) ChestInventoryManager.Instance.ConsumeItems(req.itemName, req.amount);

        // 3. เพิ่มโบนัส
        PlayerStats player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();
        if (next.statType == "MaxHP") player.bonusMaxHP += next.bonusValue;
        else if (next.statType == "Defense") player.bonusDefense += next.bonusValue;
        else if (next.statType == "SwordDamage") player.bonusSwordDamage += next.bonusValue;
        else if (next.statType == "AxeDamage") player.bonusAxeDamage += next.bonusValue;
        else if (next.statType == "PickaxeDamage") player.bonusPickaxeDamage += next.bonusValue;
        else if (next.statType == "WalkSpeed") player.bonusWalkSpeed += next.bonusValue;

        // 4. อัปเดต Stats รวม
        player.CalculateStats();
        currentLevel++;
        Debug.Log($"{furnitureName} upgraded! level: {currentLevel}");
    }

    void Update()
    {
        if (upgradePanel.activeSelf && Vector3.Distance(transform.position, player.position) > closeDistance)
        {
            upgradePanel.SetActive(false);
        }
    }
}