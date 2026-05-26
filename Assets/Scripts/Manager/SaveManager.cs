using UnityEngine;
using System.Collections.Generic;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;

    private const string SAVE_KEY = "SeaSoulSaveData";

    [Header("References")]
    // Drag all furniture objects here in Inspector
    public List<Furniture> allFurniture = new List<Furniture>();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // -----------------------------------------------
    // SAVE
    // -----------------------------------------------

    public void SaveGame()
    {
        SaveData data = new SaveData();

        // 1. Save player stats
        GameObject playerObj =
            GameObject.FindGameObjectWithTag("Player");
        PlayerStats pStats =
            playerObj.GetComponent<PlayerStats>();

        data.bonusMaxHP = pStats.bonusMaxHP;
        data.bonusDefense = pStats.bonusDefense;
        data.bonusSwordDamage = pStats.bonusSwordDamage;
        data.bonusAxeDamage = pStats.bonusAxeDamage;
        data.bonusPickaxeDamage = pStats.bonusPickaxeDamage;
        data.bonusWalkSpeed = pStats.bonusWalkSpeed;
        data.currentHP = pStats.currentHP;

        // 2. Save furniture levels
        data.furnitureLevels.Clear();
        foreach (var furniture in allFurniture)
        {
            data.furnitureLevels.Add(new FurnitureSaveData
            {
                furnitureName = furniture.furnitureName,
                currentLevel = furniture.currentLevel
            });
        }

        // 3. Save chest inventory
        data.chestItems.Clear();
        foreach (var item in
            ChestInventoryManager.Instance.chestInventory)
        {
            data.chestItems.Add(new ChestItemData
            {
                itemName = item.Key,
                amount = item.Value
            });
        }

        // Serialize to JSON and save
        string json = JsonUtility.ToJson(data, prettyPrint: true);
        PlayerPrefs.SetString(SAVE_KEY, json);
        PlayerPrefs.Save();

        Debug.Log($"Game saved!\n{json}");
    }

    // -----------------------------------------------
    // LOAD
    // -----------------------------------------------

    public void LoadGame()
    {
        if (!HasSaveData())
        {
            Debug.Log("No save data found — starting fresh.");
            return;
        }

        string json = PlayerPrefs.GetString(SAVE_KEY);
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        // 1. Load player stats
        GameObject playerObj =
            GameObject.FindGameObjectWithTag("Player");
        PlayerStats pStats =
            playerObj.GetComponent<PlayerStats>();

        pStats.bonusMaxHP = data.bonusMaxHP;
        pStats.bonusDefense = data.bonusDefense;
        pStats.bonusSwordDamage = data.bonusSwordDamage;
        pStats.bonusAxeDamage = data.bonusAxeDamage;
        pStats.bonusPickaxeDamage = data.bonusPickaxeDamage;
        pStats.bonusWalkSpeed = data.bonusWalkSpeed;
        pStats.CalculateStats();
        pStats.currentHP = data.currentHP;

        // 2. Load furniture levels
        foreach (var savedFurniture in data.furnitureLevels)
        {
            Furniture match = allFurniture.Find(
                f => f.furnitureName == savedFurniture.furnitureName);

            if (match != null)
                match.currentLevel = savedFurniture.currentLevel;
        }

        // 3. Load chest inventory
        ChestInventoryManager.Instance.chestInventory.Clear();
        foreach (var item in data.chestItems)
        {
            ChestInventoryManager.Instance.chestInventory
                .Add(item.itemName, item.amount);
        }

        Debug.Log("Game loaded!");
    }

    // -----------------------------------------------
    // HELPERS
    // -----------------------------------------------

    public bool HasSaveData()
    {
        return PlayerPrefs.HasKey(SAVE_KEY);
    }

    public void DeleteSave()
    {
        PlayerPrefs.DeleteKey(SAVE_KEY);
        Debug.Log("Save data deleted.");
    }
}