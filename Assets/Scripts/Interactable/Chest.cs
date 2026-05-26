using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour, IInteractable
{
    public GameObject chestPanel;
    public Transform chestContent;

    private Transform player;
    public float closeDistance = 3f;

    void Start() { player = GameObject.FindGameObjectWithTag("Player").transform; }

    void Update()
    {
        if (chestPanel.activeSelf && Vector3.Distance(transform.position, player.position) > closeDistance)
        {
            chestPanel.SetActive(false);
        }
    }

    public void Interact()
    {
        if (chestPanel.activeSelf)
        {
            chestPanel.SetActive(false);
        }
        else
        {
            chestPanel.SetActive(true);
            InventoryManager.Instance.RenderInventory(ChestInventoryManager.Instance.chestInventory, chestContent);
        }
    }
}