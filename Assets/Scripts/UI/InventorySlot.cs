using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IPointerClickHandler
{
    public Image iconImage;
    public TextMeshProUGUI amountText;
    private string itemNameInSlot;

    public bool isChestSlot = false;

    public void UpdateSlot(string name, Sprite icon, int amount)
    {
        itemNameInSlot = name;
        iconImage.sprite = icon;
        amountText.text = amount.ToString();
        iconImage.enabled = true;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isChestSlot) return;

        if (!string.IsNullOrEmpty(itemNameInSlot))
        {
            Debug.Log("Click on Slot Item: " + itemNameInSlot);
            InventoryManager.Instance.DropItemOneByOne(itemNameInSlot);
        }
    }
}