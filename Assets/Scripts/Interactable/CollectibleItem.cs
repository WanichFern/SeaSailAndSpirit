using UnityEngine;

public class CollectibleItem : MonoBehaviour, IInteractable
{
    public string itemName = "Log";

    public void Interact()
    {
        // ย้าย Logic จาก PlayerInteraction มาไว้ที่นี่
        bool isPickedUp = InventoryManager.Instance.AddItem(itemName, 1);
        if (isPickedUp)
        {
            Destroy(gameObject);
        }
    }
}