using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    public GameObject chestPanel; // ลาก ChestPanel จาก UI มาใส่
    public Transform chestContent; // ลาก Content ใน ScrollView มาใส่

    private Transform player;
    public float closeDistance = 3f;

    void Start() { player = GameObject.FindGameObjectWithTag("Player").transform; }

    void Update()
    {
        // เช็คระยะห่าง ถ้าเดินไกลกว่า 3 เมตร ให้ปิด UI
        if (chestPanel.activeSelf && Vector3.Distance(transform.position, player.position) > closeDistance)
        {
            chestPanel.SetActive(false);
        }
    }

    public void Interact()
    {
        if (chestPanel.activeSelf)
        {
            chestPanel.SetActive(false); // ปิด
        }
        else
        {
            chestPanel.SetActive(true); // เปิด
            // สั่งให้ InventoryManager มาวาดของในกล่องนี้ให้หน่อย!
            InventoryManager.Instance.RenderInventory(ChestInventoryManager.Instance.chestInventory, chestContent);
        }
    }
}