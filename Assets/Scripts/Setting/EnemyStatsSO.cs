using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy Stats", menuName = "ScriptableObjects/EnemyStats")]
public class EnemyStatsSO : BaseStatsSO
{
    public string enemyName;

    [Header("Combat Stats")]
    public float damage;
    public float attackCooldown;

    [Header("AI Settings")]
    public float chaseRange;
    public float attackRange;

    [Header("Drop Settings")]
    public GameObject dropItemPrefab;
    [Range(0, 100)] public float dropChance;
}