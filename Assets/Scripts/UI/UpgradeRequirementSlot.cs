using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeRequirementSlot : MonoBehaviour
{
    public Image iconImage;
    public TextMeshProUGUI amountText;

    public void Setup(string itemName, int amount)
    {
        amountText.text = "x" + amount.ToString();

        Sprite icon = InventoryManager.Instance.GetIconByName(itemName);

        if (icon != null)
        {
            iconImage.sprite = icon;
            iconImage.enabled = true;
        }
        else
        {
            iconImage.enabled = false;
        }
    }
}