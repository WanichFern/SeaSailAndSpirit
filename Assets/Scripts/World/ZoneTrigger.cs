using UnityEngine;
using Unity.Cinemachine;

public enum Zone { Sea, Island }

public class ZoneTrigger : MonoBehaviour
{
    public Zone thisZone;

    [Header("All Cameras")]
    public CinemachineCamera seaCam;
    public CinemachineCamera islandCam;

    public static ZoneTrigger IslandZoneTrigger;

    // MainCam stays at Priority 10 always — never touched by triggers

    void Awake()
    {
        if (thisZone == Zone.Island)
            IslandZoneTrigger = this;
    }

    public static void DropIslandCam()
    {
        if (IslandZoneTrigger != null)
            IslandZoneTrigger.islandCam.Priority = 1;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        switch (thisZone)
        {
            case Zone.Sea:
                seaCam.Priority = 11;
                islandCam.Priority = 1;
                break;

            case Zone.Island:
                islandCam.Priority = 11;
                BoatController.Instance?.EnterIslandZone();
                break;
        }

        Debug.Log($"Entered zone: {thisZone}");
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        switch (thisZone)
        {
            case Zone.Sea:
                seaCam.Priority = 1;
                islandCam.Priority = 1;
                BoatController.Instance?.EnterHomeZone();
                break;

            case Zone.Island:
                // Only drop island cam if player is NOT on the boat
                // (if they're on boat, camera change is handled by ExitBoat())
                bool playerOnBoat = BoatController.Instance != null
                    && BoatController.Instance.IsPlayerOnBoard;

                if (!playerOnBoat)
                    islandCam.Priority = 1;
                break;
        }
    }
}