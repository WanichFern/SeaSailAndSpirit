using UnityEngine;
using System.Collections;

public class HouseDoor : MonoBehaviour, IInteractable
{
    public Transform destination;
    public Transform player;
    public SeaSpiritAI seaSpirit;
    public ScreenFader fader;

    public void Interact()
    {
        StartCoroutine(TeleportRoutine());
    }

    IEnumerator TeleportRoutine()
    {
        yield return fader.Fade(true);

        player.position = destination.position;
        seaSpirit.transform.position =
            destination.position + new Vector3(1, 1, 0);

        InventoryManager.Instance.TransferToChest();

        // Save after items are transferred to chest
        SaveManager.Instance?.SaveGame();

        yield return new WaitForSeconds(0.5f);
        yield return fader.Fade(false);
    }
}