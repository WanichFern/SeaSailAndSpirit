using UnityEngine;
using Unity.Services.LevelPlay;
using System;
using System.Collections;

public class RewardedAdController : MonoBehaviour
{
    public static RewardedAdController Instance;

    [SerializeField] private string rewardedAdUnitId = "YOUR_REWARDED_AD_UNIT_ID";

    private LevelPlayRewardedAd rewardedAd;
    private bool isReady;
    private bool rewardWasGranted = false;
    private Action pendingRewardAction;
    private Action pendingFailAction;

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

    public bool TryShowRewarded(Action onRewardGranted,
        Action onFailed = null)
    {
        if (!isReady)
        {
            Debug.LogWarning("[Rewarded] Ad not ready.");
            onFailed?.Invoke();
            return false;
        }

        rewardWasGranted = false;
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
        rewardWasGranted = false;
    }

    private void OnAdRewarded(LevelPlayAdInfo adInfo,
        LevelPlayReward reward)
    {
        Debug.Log("[Rewarded] Reward granted!");
        rewardWasGranted = true;

        // Invoke reward immediately when we get it
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

        // Wait one frame before checking reward
        StartCoroutine(DelayedCloseCheck());
    }

    private IEnumerator DelayedCloseCheck()
    {
        // Wait for end of frame so OnAdRewarded
        // has a chance to fire first if it's coming
        yield return new WaitForEndOfFrame();

        if (!rewardWasGranted)
        {
            Debug.Log("[Rewarded] Closed without reward.");
            pendingFailAction?.Invoke();
        }

        pendingRewardAction = null;
        pendingFailAction = null;
        rewardWasGranted = false;

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