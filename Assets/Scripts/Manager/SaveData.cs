using System.Collections.Generic;

[System.Serializable]
public class FurnitureSaveData
{
    public string furnitureName;
    public int currentLevel;
}

[System.Serializable]
public class ChestItemData
{
    public string itemName;
    public int amount;
}

[System.Serializable]
public class SaveData
{
    // Player bonus stats from furniture
    public float bonusMaxHP;
    public float bonusDefense;
    public float bonusSwordDamage;
    public float bonusAxeDamage;
    public float bonusPickaxeDamage;
    public float bonusWalkSpeed;

    // Player live state
    public float currentHP;

    // Furniture levels
    public List<FurnitureSaveData> furnitureLevels
        = new List<FurnitureSaveData>();

    // Chest inventory
    public List<ChestItemData> chestItems
        = new List<ChestItemData>();
}