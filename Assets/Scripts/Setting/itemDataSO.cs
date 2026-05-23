using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/ItemData")]
public class ItemDataSO : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public GameObject itemPrefab;
}