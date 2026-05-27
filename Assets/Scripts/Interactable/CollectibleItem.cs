using UnityEngine;

public class CollectibleItem : MonoBehaviour, IInteractable
{
    public string itemName = "Log";

    public void Interact()
    {
        bool isPickedUp = InventoryManager.Instance.AddItem(itemName, 1);
        if (isPickedUp)
        {
            Destroy(gameObject);
        }
    }
}