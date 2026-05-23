using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    private int currentTotalItems = 0;

    [Header("UI Reference")]
    public TextMeshProUGUI capacityText;
    public Color normalColor = Color.white;
    public Color warningColor = Color.red;
    public Transform slotContainer;
    public GameObject slotPrefab;

    [Header("Data")]
    public List<ItemDataSO> itemDatabase = new List<ItemDataSO>();
    public Dictionary<string, int> inventory = new Dictionary<string, int>();

    // Lazy-loaded reference — never goes null silently
    private PlayerStats _pStats;
    private PlayerStats pStats
    {
        get
        {
            if (_pStats == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null) _pStats = player.GetComponent<PlayerStats>();
            }
            return _pStats;
        }
    }

    void Awake() => Instance = this;

    void Start()
    {
        if (pStats == null)
        {
            Debug.LogError("InventoryManager: Could not find PlayerStats!", this);
            return;
        }
        CalculateTotalItems();
        UpdateCapacityUI();
    }

    public bool AddItem(string itemName, int amount)
    {
        if (pStats == null) return false;

        if (currentTotalItems + amount > pStats.maxInventory)
        {
            StartCoroutine(FlashCapacityText());
            Debug.Log("Inventory Full!");
            return false;
        }

        if (inventory.ContainsKey(itemName))
            inventory[itemName] += amount;
        else
            inventory.Add(itemName, amount);

        CalculateTotalItems();
        RefreshUI();
        return true;
    }

    public bool IsFull()
    {
        if (pStats == null) return false;
        return currentTotalItems >= pStats.maxInventory;
    }

    private void CalculateTotalItems()
    {
        currentTotalItems = 0;
        foreach (var item in inventory)
            currentTotalItems += item.Value;

        UpdateCapacityUI();
    }

    void UpdateCapacityUI()
    {
        if (capacityText != null && pStats != null)
            capacityText.text = $"{currentTotalItems}/{pStats.maxInventory}";
    }

    void RefreshUI()
    {
        foreach (Transform child in slotContainer)
            Destroy(child.gameObject);

        foreach (var item in inventory)
        {
            GameObject newSlot = Instantiate(slotPrefab, slotContainer);
            InventorySlot slotScript = newSlot.GetComponent<InventorySlot>();

            if (slotScript != null)
            {
                Sprite icon = GetIconByName(item.Key);
                slotScript.UpdateSlot(item.Key, icon, item.Value);
            }
            else
            {
                Debug.LogError("InventorySlot script missing in Slot Prefab");
            }
        }
    }

    public void DropItemOneByOne(string itemName)
    {
        if (inventory.ContainsKey(itemName) && inventory[itemName] > 0)
        {
            inventory[itemName]--;
            if (inventory[itemName] <= 0)
                inventory.Remove(itemName);

            SpawnItemInWorld(itemName);
            CalculateTotalItems();
            RefreshUI();
        }
    }

    private void SpawnItemInWorld(string itemName)
    {
        foreach (var data in itemDatabase)
        {
            if (data.itemName == itemName)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                Vector3 spawnPos = player.transform.position + new Vector3(0, 0, -1.5f);
                Instantiate(data.itemPrefab, spawnPos, Quaternion.identity);
                break;
            }
        }
    }

    public void ClearInventoryOnDeath()
    {
        inventory.Clear();
        CalculateTotalItems();
        RefreshUI();
        Debug.Log("Inventory cleared due to death.");
    }

    public void TransferToChest()
    {
        foreach (var item in inventory)
            ChestInventoryManager.Instance.DepositItem(item.Key, item.Value);

        inventory.Clear();
        currentTotalItems = 0;
        CalculateTotalItems();
        RefreshUI();
        UpdateCapacityUI();
        Debug.Log("Items moved to chest.");
    }

    public void RenderInventory(Dictionary<string, int> inventoryData, Transform container)
    {
        foreach (Transform child in container) Destroy(child.gameObject);

        foreach (var item in inventoryData)
        {
            GameObject newSlot = Instantiate(slotPrefab, container);
            InventorySlot slotScript = newSlot.GetComponent<InventorySlot>();
            Sprite icon = GetIconByName(item.Key);
            slotScript.UpdateSlot(item.Key, icon, item.Value);
        }
    }

    System.Collections.IEnumerator FlashCapacityText()
    {
        capacityText.color = warningColor;
        yield return new WaitForSeconds(0.2f);
        capacityText.color = normalColor;
        yield return new WaitForSeconds(0.2f);
        capacityText.color = warningColor;
        yield return new WaitForSeconds(0.2f);
        capacityText.color = normalColor;
    }

    public Sprite GetIconByName(string name)
    {
        foreach (var data in itemDatabase)
            if (data.itemName == name) return data.icon;
        return null;
    }
}