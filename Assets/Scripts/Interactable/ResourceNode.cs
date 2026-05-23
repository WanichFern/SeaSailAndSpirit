using UnityEngine;

// 1. เพิ่ม Enum ไว้บนสุดเพื่อใช้แยกประเภททรัพยากร
public enum ResourceType { Wood, Stone }

public class ResourceNode : MonoBehaviour
{
    [Header("Resource Settings")]
    public string nodeName = "Tree";
    public ResourceType resourceType = ResourceType.Wood; // 👈 เลือกใน Inspector ว่าเป็นไม้หรือหิน
    public float health = 30f;            // 👈 เปลี่ยนเป็นเลือด (HP) แทนจำนวนครั้ง

    [Header("Drop Settings")]
    public GameObject dropPrefab;
    public int dropAmount = 3;

    // 2. ปรับให้ฟังก์ชันรับค่า "ดาเมจ" เข้ามา
    public void TakeHit(float damage)
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
            Debug.LogWarning($"ยังไม่ได้ใส่ Drop Prefab ให้กับ {nodeName}");
            return;
        }

        for (int i = 0; i < dropAmount; i++)
        {
            float randomX = transform.position.x + Random.Range(-1.5f, 1.5f);
            float randomZ = transform.position.z + Random.Range(-1.5f, 1.5f);
            float groundY = 0.5f;

            Vector3 dropPos = new Vector3(randomX, groundY, randomZ);
            Instantiate(dropPrefab, dropPos, Quaternion.identity);
        }
    }
}