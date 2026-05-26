using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class UpgradeUIManager : MonoBehaviour
{
    public static UpgradeUIManager Instance;

    [Header("UI Panels")]
    public GameObject upgradePanel;

    [Header("Text Elements")]
    public TextMeshProUGUI furnitureNameText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI statText;

    [Header("Requirements")]
    public Transform reqContainer;
    public GameObject reqSlotPrefab;

    private Furniture currentFurniture;

    void Awake() => Instance = this;

    public void OpenUpgradeMenu(Furniture furniture)
    {
        currentFurniture = furniture;
        upgradePanel.SetActive(true);

        furnitureNameText.text = furniture.furnitureName;
        levelText.text = $"Level: {furniture.currentLevel} → {furniture.currentLevel + 1}";

        if (furniture.currentLevel < furniture.upgradeLevels.Count)
        {
            var nextLevel = furniture.upgradeLevels[furniture.currentLevel];

            statText.text = $"{nextLevel.statType} +{nextLevel.bonusValue}";

            DrawRequirements(nextLevel.requirements);
        }
        else
        {
            statText.text = "Max Level Reached!";
            foreach (Transform child in reqContainer)
                Destroy(child.gameObject);
        }
    }

    private void DrawRequirements(List<Requirement> requirements)
    {
        foreach (Transform child in reqContainer)
            Destroy(child.gameObject);

        foreach (var req in requirements)
        {
            GameObject slot = Instantiate(reqSlotPrefab, reqContainer);
            slot.GetComponent<UpgradeRequirementSlot>().Setup(req.itemName, req.amount);
        }
    }

    public void ConfirmUpgrade()
    {
        if (currentFurniture != null)
        {
            currentFurniture.PerformUpgrade();
            upgradePanel.SetActive(false);
        }
    }
}