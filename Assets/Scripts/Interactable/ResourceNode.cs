using UnityEngine;

public enum ResourceType { Wood, Stone }

public class ResourceNode : MonoBehaviour, IDamageable
{
    [Header("Resource Settings")]
    public string nodeName = "Tree";
    public ResourceType resourceType = ResourceType.Wood;
    public float health = 30f;

    [Header("Drop Settings")]
    public GameObject dropPrefab;
    public int dropAmount = 3;

    public void TakeDamage(float damage)
    {
        health -= damage;
        Debug.Log($"{nodeName} hit! Took {damage} damage. HP left: {health}");

        if (health <= 0)
        {
            DropItems();
            Destroy(gameObject);
        }
    }

    void DropItems()
    {
        if (dropPrefab == null)
        {
            Debug.LogWarning($"No Drop Prefab assigned to {nodeName}");
            return;
        }

        for (int i = 0; i < dropAmount; i++)
        {
            float randomX = transform.position.x + Random.Range(-1.5f, 1.5f);
            float randomZ = transform.position.z + Random.Range(-1.5f, 1.5f);
            Vector3 dropPos = new Vector3(randomX, 0.5f, randomZ);
            Instantiate(dropPrefab, dropPos, Quaternion.identity);
        }
    }
}