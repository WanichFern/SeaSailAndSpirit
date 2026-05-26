using UnityEngine;

public class BoatDock : MonoBehaviour
{
    public Transform boatSpawnPoint;
    public GameObject boatPrefab;

    private BoatController currentBoat;

    void Start()
    {
        SpawnBoat();
    }

    void SpawnBoat()
    {
        if (currentBoat != null) return;
        GameObject boatObj = Instantiate(
            boatPrefab,
            boatSpawnPoint.position,
            boatSpawnPoint.rotation);
        currentBoat = boatObj.GetComponent<BoatController>();
        currentBoat.dockSpawnPoint = boatSpawnPoint;
    }

    public void Interact()
    {
        if (currentBoat != null)
            currentBoat.TryBoard();
    }
}