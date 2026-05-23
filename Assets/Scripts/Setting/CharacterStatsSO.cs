using UnityEngine;

[CreateAssetMenu(fileName = "New Character Stats", menuName = "ScriptableObjects/CharacterStats")]
public class CharacterStatsSO : BaseStatsSO
{
    [Header("Core Stats")]
    public float defense = 0f;

    [Header("Combat Stats")]
    public float swordDamage = 15f;
    public float attackCooldown = 1.0f;

    [Header("Gathering Stats")]
    public float axeDamage = 10f;
    public float pickaxeDamage = 10f;
    public float gatherCooldown = 0.5f;

    [Header("Utility Stats")]
    public int inventoryCapacity = 10;
    //public float dropRateBonus = 0f; //โอกาสได้ของหายาก
}