using System.Collections.Generic;
using UnityEngine;

public class ChestInventoryManager : MonoBehaviour
{
    public static ChestInventoryManager Instance;

    public Dictionary<string, int> chestInventory = new Dictionary<string, int>();

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

    public void DepositItem(string itemName, int amount)
    {
        if (chestInventory.ContainsKey(itemName))
            chestInventory[itemName] += amount;
        else
            chestInventory.Add(itemName, amount);

        Debug.Log($"{itemName} for {amount} has been added to the chest");
    }

    public bool HasEnoughItems(string itemName, int amount)
    {
        return chestInventory.ContainsKey(itemName) && chestInventory[itemName] >= amount;
    }

    public void ConsumeItems(string itemName, int amount)
    {
        if (chestInventory.ContainsKey(itemName))
        {
            chestInventory[itemName] -= amount;
        }
    }
}