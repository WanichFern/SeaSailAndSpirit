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
        if (isPaused) return;

        isPaused = true;
        Time.timeScale = 0f;
        hudUI.SetActive(false);
        pauseMenuUI.SetActive(true);

        SaveManager.Instance?.LoadGame();

        // Show banner on pause
        BannerAdController.Instance?.LoadAndShowBanner();
    }

    public void ResumeGame()
    {
        if (!isPaused) return;

        isPaused = false;
        Time.timeScale = 1f;
        hudUI.SetActive(true);
        pauseMenuUI.SetActive(false);

        // Hide banner when resuming
        BannerAdController.Instance?.HideBanner();
    }
}