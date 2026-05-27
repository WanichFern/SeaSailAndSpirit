using UnityEngine;
using System.Collections.Generic;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;

    private const string SAVE_KEY = "SeaSoulSaveData";

    [Header("References")]
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

    void Start()
    {
        // Auto-load on game start
        LoadGame();
    }

    // -----------------------------------------------
    // SAVE
    // -----------------------------------------------

    public void SaveGame()
    {
        SaveData data = new SaveData();

        // 1. Save bonus stats only
        GameObject playerObj =
            GameObject.FindGameObjectWithTag("Player");
        PlayerStats pStats =
            playerObj?.GetComponent<PlayerStats>();

        if (pStats != null)
        {
            data.bonusMaxHP = pStats.bonusMaxHP;
            data.bonusDefense = pStats.bonusDefense;
            data.bonusSwordDamage = pStats.bonusSwordDamage;
            data.bonusAxeDamage = pStats.bonusAxeDamage;
            data.bonusPickaxeDamage = pStats.bonusPickaxeDamage;
            data.bonusWalkSpeed = pStats.bonusWalkSpeed;
            data.bonusInventoryCapacity = pStats.bonusInventoryCapacity;
        }

        // 2. Save furniture levels
        data.furnitureLevels.Clear();
        foreach (var furniture in allFurniture)
        {
            if (furniture == null) continue;
            data.furnitureLevels.Add(new FurnitureSaveData
            {
                furnitureName = furniture.furnitureName,
                currentLevel = furniture.currentLevel
            });
        }

        // 3. Save chest inventory
        data.chestItems.Clear();
        if (ChestInventoryManager.Instance != null)
        {
            foreach (var item in
                ChestInventoryManager.Instance.chestInventory)
            {
                data.chestItems.Add(new ChestItemData
                {
                    itemName = item.Key,
                    amount = item.Value
                });
            }
        }

        string json = JsonUtility.ToJson(data, prettyPrint: true);
        PlayerPrefs.SetString(SAVE_KEY, json);
        PlayerPrefs.Save();

        Debug.Log($"[Save] Game saved! Items in chest: " +
            $"{data.chestItems.Count}");
    }

    // -----------------------------------------------
    // LOAD
    // -----------------------------------------------

    public void LoadGame()
    {
        if (!HasSaveData())
        {
            Debug.Log("[Save] No save data found — fresh start.");
            return;
        }

        string json = PlayerPrefs.GetString(SAVE_KEY);

        if (string.IsNullOrEmpty(json))
        {
            Debug.LogWarning("[Save] Save key exists but data is empty.");
            return;
        }

        SaveData data = JsonUtility.FromJson<SaveData>(json);

        if (data == null)
        {
            Debug.LogError("[Save] Failed to parse save data.");
            return;
        }

        // 1. Load bonus stats → then recalculate totals
        GameObject playerObj =
            GameObject.FindGameObjectWithTag("Player");
        PlayerStats pStats =
            playerObj?.GetComponent<PlayerStats>();

        if (pStats != null)
        {
            pStats.bonusMaxHP = data.bonusMaxHP;
            pStats.bonusDefense = data.bonusDefense;
            pStats.bonusSwordDamage = data.bonusSwordDamage;
            pStats.bonusAxeDamage = data.bonusAxeDamage;
            pStats.bonusPickaxeDamage = data.bonusPickaxeDamage;
            pStats.bonusWalkSpeed = data.bonusWalkSpeed;
            pStats.bonusInventoryCapacity = data.bonusInventoryCapacity;

            // Recalculate totals with new bonuses
            pStats.CalculateStats();

            // Always start with full HP regardless of save
            pStats.currentHP = pStats.totalMaxHP;

            Debug.Log("[Save] Player stats loaded.");
        }

        // 2. Load furniture levels
        foreach (var savedFurniture in data.furnitureLevels)
        {
            Furniture match = allFurniture.Find(
                f => f.furnitureName == savedFurniture.furnitureName);

            if (match != null)
            {
                match.currentLevel = savedFurniture.currentLevel;
                Debug.Log($"[Save] {match.furnitureName} " +
                    $"loaded at level {match.currentLevel}");
            }
            else
            {
                Debug.LogWarning($"[Save] Furniture " +
                    $"'{savedFurniture.furnitureName}' not found in list.");
            }
        }

        // 3. Load chest inventory
        if (ChestInventoryManager.Instance != null)
        {
            ChestInventoryManager.Instance.chestInventory.Clear();
            foreach (var item in data.chestItems)
            {
                ChestInventoryManager.Instance
                    .chestInventory.Add(item.itemName, item.amount);
            }
            Debug.Log($"[Save] Chest loaded with " +
                $"{data.chestItems.Count} item types.");
        }
        else
        {
            Debug.LogError("[Save] ChestInventoryManager not found!");
        }

        Debug.Log("[Save] Game loaded successfully!");
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
        PlayerPrefs.Save();
        Debug.Log("[Save] Save data deleted.");
    }
}