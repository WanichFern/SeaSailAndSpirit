using UnityEngine;
using Unity.Cinemachine;

public enum Zone { Sea, Island }

public class ZoneTrigger : MonoBehaviour
{
    public Zone thisZone;

    [Header("Cameras")]
    public CinemachineCamera seaCam;
    // Island cam removed entirely

    public static ZoneTrigger IslandZoneTrigger;

    void Awake()
    {
        if (thisZone == Zone.Island)
            IslandZoneTrigger = this;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        switch (thisZone)
        {
            case Zone.Sea:
                seaCam.Priority = 11;
                break;

            case Zone.Island:
                BoatController boat = BoatController.Instance;
                if (boat != null && boat.IsHeadingToIsland)
                    boat.OnReachedIsland();
                break;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        switch (thisZone)
        {
            case Zone.Sea:
                seaCam.Priority = 1;
                BoatController.Instance?.OnReachedHome();
                break;

            case Zone.Island:
                break;
        }
    }

    public static void DropIslandCam(){}
}