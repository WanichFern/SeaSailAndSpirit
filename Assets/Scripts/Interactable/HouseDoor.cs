using UnityEngine;
using System.Collections;

public class HouseDoor : MonoBehaviour
{
    public Transform destination; // ลากจุดที่จะให้โผล่ไปวาง
    public Transform player;
    public SeaSpiritAI seaSpirit; // ลากตัว SeaSpirit มาใส่
    public ScreenFader fader;

    public void Interact()
    {
        StartCoroutine(TeleportRoutine());
    }

    IEnumerator TeleportRoutine()
    {
        yield return fader.Fade(true);

        player.position = destination.position;
        seaSpirit.transform.position = destination.position + new Vector3(1, 1, 0);

        InventoryManager.Instance.TransferToChest();

        yield return new WaitForSeconds(0.5f);

        yield return fader.Fade(false);
    }
}