using UnityEngine;
using Unity.Services.LevelPlay;

public class LevelPlayInitializer : MonoBehaviour
{
    public static LevelPlayInitializer Instance;

    [SerializeField] private string appKey = "267db7255";

    [Header("Ad Controllers")]
    [SerializeField] private RewardedAdController rewardedAdController;
    [SerializeField] private BannerAdController bannerAdController;

    public static bool IsInitialized { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        Debug.Log("[LevelPlay] Initializing SDK...");

        LevelPlay.OnInitSuccess += OnInitSuccess;
        LevelPlay.OnInitFailed += OnInitFailed;

        LevelPlay.Init(appKey);
    }

    void OnInitSuccess(LevelPlayConfiguration configuration)
    {
        IsInitialized = true;
        Debug.Log("[LevelPlay] SDK initialized successfully.");

        // Initialize ad units after SDK is ready
        rewardedAdController.InitializeRewarded();
        rewardedAdController.LoadRewarded();

        bannerAdController.InitializeBanner();
        // Don't load banner yet — only load when pause screen opens
    }

    void OnInitFailed(LevelPlayInitError error)
    {
        IsInitialized = false;
        Debug.LogError("[LevelPlay] SDK initialization failed: " + error);
    }

    void OnDestroy()
    {
        LevelPlay.OnInitSuccess -= OnInitSuccess;
        LevelPlay.OnInitFailed -= OnInitFailed;
    }
}