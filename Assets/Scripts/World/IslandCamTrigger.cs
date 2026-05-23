using UnityEngine;
using Unity.Cinemachine; // Namespace สำหรับ Cinemachine รุ่นใหม่

public class IslandCamTrigger : MonoBehaviour
{
    public CinemachineCamera landingCam; // ลากกล้อง Top-view มาใส่

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            landingCam.Priority = 11; // กล้องจะ Zoom Out ทันที
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            landingCam.Priority = 1; // กลับไปใช้กล้องเดินปกติ
        }
    }
}