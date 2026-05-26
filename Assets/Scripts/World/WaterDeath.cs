using UnityEngine;

public class WaterDeath : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerStats stats = other.GetComponent<PlayerStats>();
            if (stats != null)
            {
                Debug.Log("Player fell in water!");
                stats.TakeDamage(999f); // instant kill
            }
        }
    }
}