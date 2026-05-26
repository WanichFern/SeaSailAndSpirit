using UnityEngine;
using Unity.Services.LevelPlay;

public class BannerAdController : MonoBehaviour
{
    public static BannerAdController Instance;

    [SerializeField] private string bannerAdUnitId = "YOUR_BANNER_AD_UNIT_ID";
    private LevelPlayBannerAd bannerAd;
    private bool isInitialized = false;

    void Awake() => Instance = this;

    public void InitializeBanner()
    {
        var configBuilder = new LevelPlayBannerAd.Config.Builder();
        configBuilder.SetSize(LevelPlayAdSize.CreateAdaptiveAdSize());
        configBuilder.SetPosition(LevelPlayBannerPosition.BottomCenter);
        configBuilder.SetDisplayOnLoad(true);
        configBuilder.SetRespectSafeArea(true);

        var bannerConfig = configBuilder.Build();
        bannerAd = new LevelPlayBannerAd(bannerAdUnitId, bannerConfig);

        bannerAd.OnAdLoaded += BannerOnAdLoaded;
        bannerAd.OnAdLoadFailed += BannerOnAdLoadFailed;
        bannerAd.OnAdDisplayed += BannerOnAdDisplayed;
        bannerAd.OnAdDisplayFailed += BannerOnAdDisplayFailed;
        bannerAd.OnAdClicked += BannerOnAdClicked;

        isInitialized = true;
        Debug.Log("[Banner] Initialized.");
    }

    public void LoadAndShowBanner()
    {
        if (!isInitialized)
        {
            Debug.LogWarning("[Banner] Not initialized yet.");
            return;
        }
        bannerAd.LoadAd(); // loads and auto-shows (DisplayOnLoad = true)
    }

    public void ShowBanner()
    {
        bannerAd?.ShowAd();
    }

    public void HideBanner()
    {
        bannerAd?.HideAd();
    }

    public void DestroyBanner()
    {
        if (bannerAd == null) return;
        bannerAd.OnAdLoaded -= BannerOnAdLoaded;
        bannerAd.OnAdLoadFailed -= BannerOnAdLoadFailed;
        bannerAd.OnAdDisplayed -= BannerOnAdDisplayed;
        bannerAd.OnAdDisplayFailed -= BannerOnAdDisplayFailed;
        bannerAd.OnAdClicked -= BannerOnAdClicked;
        bannerAd.DestroyAd();
    }

    private void BannerOnAdLoaded(LevelPlayAdInfo adInfo)
    {
        Debug.Log("[Banner] Loaded.");
    }

    private void BannerOnAdLoadFailed(LevelPlayAdError error)
    {
        Debug.LogError("[Banner] Load failed: " + error);
    }

    private void BannerOnAdDisplayed(LevelPlayAdInfo adInfo)
    {
        Debug.Log("[Banner] Displayed.");
    }

    private void BannerOnAdDisplayFailed(LevelPlayAdInfo adInfo,
        LevelPlayAdError error)
    {
        Debug.LogError("[Banner] Display failed: " + error);
    }

    private void BannerOnAdClicked(LevelPlayAdInfo adInfo)
    {
        Debug.Log("[Banner] Clicked.");
    }

    void OnDestroy() => DestroyBanner();
}