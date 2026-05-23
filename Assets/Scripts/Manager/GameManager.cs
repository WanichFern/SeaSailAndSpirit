using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI Panels")]
    public GameObject hudUI;        // กล่องที่ใส่ปุ่ม Pause และ UI ตอนเล่นปกติ
    public GameObject pauseMenuUI; // กล่องที่ใส่ปุ่ม Resume (หน้าจอเมนู)

    private bool isPaused = false;

    void Awake()
    {
        // ทำ Singleton เพื่อให้เรียกใช้ GameManager.Instance ได้จากทุกที่
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // เริ่มเกมมาให้แน่ใจว่าเมนูหยุดถูกปิด และ UI ปกติถูกเปิด
        ResumeGame();
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f; // หยุดเวลาในเกมทั้งหมด (ฟิสิกส์, อนิเมชั่นที่อิง deltaTime)

        hudUI.SetActive(false);      // ซ่อน UI ปกติ
        pauseMenuUI.SetActive(true); // แสดงหน้าเมนูหยุด
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f; // ให้เวลาเดินตามปกติ

        hudUI.SetActive(true);       // แสดง UI ปกติ
        pauseMenuUI.SetActive(false); // ซ่อนหน้าเมนูหยุด
    }

    // ฟังก์ชันเสริมสำหรับเช็คสถานะจากสคริปต์อื่น
    public bool IsGamePaused() => isPaused;
}