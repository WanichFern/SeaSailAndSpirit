using UnityEngine;
using Unity.Services.LevelPlay;
using System;

public class RewardedAdController : MonoBehaviour
{
    public static RewardedAdController Instance;

    [SerializeField] private string rewardedAdUnitId = "YOUR_REWARDED_AD_UNIT_ID";

    private LevelPlayRewardedAd rewardedAd;
    private bool isReady;
    private Action pendingRewardAction;
    private Action pendingFailAction; // ← added for close-without-reward

    void Awake() => Instance = this;

    public void InitializeRewarded()
    {
        rewardedAd = new LevelPlayRewardedAd(rewardedAdUnitId);

        rewardedAd.OnAdLoaded += OnAdLoaded;
        rewardedAd.OnAdLoadFailed += OnAdLoadFailed;
        rewardedAd.OnAdDisplayed += OnAdDisplayed;
        rewardedAd.OnAdDisplayFailed += OnAdDisplayFailed;
        rewardedAd.OnAdRewarded += OnAdRewarded;
        rewardedAd.OnAdClicked += OnAdClicked;
        rewardedAd.OnAdClosed += OnAdClosed;
    }

    public void LoadRewarded()
    {
        isReady = false;
        rewardedAd?.LoadAd();
    }

    // onRewardGranted = player watched full ad
    // onFailed = player closed early or ad not ready
    public bool TryShowRewarded(Action onRewardGranted,
        Action onFailed = null)
    {
        if (!isReady)
        {
            Debug.LogWarning("[Rewarded] Ad not ready.");
            onFailed?.Invoke();
            return false;
        }

        pendingRewardAction = onRewardGranted;
        pendingFailAction = onFailed;
        rewardedAd.ShowAd();
        return true;
    }

    public bool IsReady() => isReady;

    private void OnAdLoaded(LevelPlayAdInfo adInfo)
    {
        isReady = true;
        Debug.Log("[Rewarded] Ad loaded.");
    }

    private void OnAdLoadFailed(LevelPlayAdError error)
    {
        isReady = false;
        Debug.LogError("[Rewarded] Load failed: " + error);
    }

    private void OnAdDisplayed(LevelPlayAdInfo adInfo)
    {
        Debug.Log("[Rewarded] Ad displayed.");
    }

    private void OnAdDisplayFailed(LevelPlayAdInfo adInfo,
        LevelPlayAdError error)
    {
        Debug.LogError("[Rewarded] Display failed: " + error);
        pendingFailAction?.Invoke();
        pendingRewardAction = null;
        pendingFailAction = null;
    }

    private void OnAdRewarded(LevelPlayAdInfo adInfo,
        LevelPlayReward reward)
    {
        Debug.Log("[Rewarded] Reward granted!");
        pendingRewardAction?.Invoke();
        pendingRewardAction = null;
        pendingFailAction = null;
    }

    private void OnAdClicked(LevelPlayAdInfo adInfo)
    {
        Debug.Log("[Rewarded] Ad clicked.");
    }

    private void OnAdClosed(LevelPlayAdInfo adInfo)
    {
        Debug.Log("[Rewarded] Ad closed.");
        isReady = false;

        // If reward wasn't given, call fail
        if (pendingFailAction != null)
        {
            pendingFailAction.Invoke();
            pendingRewardAction = null;
            pendingFailAction = null;
        }

        LoadRewarded();
    }

    void OnDestroy()
    {
        if (rewardedAd == null) return;
        rewardedAd.OnAdLoaded -= OnAdLoaded;
        rewardedAd.OnAdLoadFailed -= OnAdLoadFailed;
        rewardedAd.OnAdDisplayed -= OnAdDisplayed;
        rewardedAd.OnAdDisplayFailed -= OnAdDisplayFailed;
        rewardedAd.OnAdRewarded -= OnAdRewarded;
        rewardedAd.OnAdClicked -= OnAdClicked;
        rewardedAd.OnAdClosed -= OnAdClosed;
    }
}