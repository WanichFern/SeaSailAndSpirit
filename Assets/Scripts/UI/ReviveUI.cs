using UnityEngine;
using TMPro;

public class ReviveUI : MonoBehaviour
{
    public static ReviveUI Instance;

    [Header("UI")]
    public GameObject revivePanel;
    public TextMeshProUGUI timerText;

    private float reviveTimeLimit = 5f;
    private float timer = 0f;
    private bool isShowing = false;

    private System.Action onRevive;
    private System.Action onDecline;

    void Awake()
    {
        Instance = this;
        revivePanel.SetActive(false);
    }

    void Update()
    {
        if (!isShowing) return;

        timer -= Time.unscaledDeltaTime;
        timerText.text = Mathf.CeilToInt(timer).ToString();

        if (timer <= 0)
            OnDeclinePressed();
    }

    public void Show(System.Action reviveAction,
        System.Action declineAction)
    {
        onRevive = reviveAction;
        onDecline = declineAction;
        timer = reviveTimeLimit;
        isShowing = true;
        revivePanel.SetActive(true);
        Time.timeScale = 0f;
    }

    // Called by "Watch Ad" button
    public void OnWatchAdPressed()
    {
        Time.timeScale = 1f;
        isShowing = false;
        revivePanel.SetActive(false);

        bool didShow = RewardedAdController.Instance.TryShowRewarded(
            onRewardGranted: () =>
            {
                // Full ad watched — revive at half HP
                onRevive?.Invoke();
            },
            onFailed: () =>
            {
                // Closed early — proper death
                onDecline?.Invoke();
            }
        );

        // Ad not ready at all — just die
        if (!didShow) onDecline?.Invoke();
    }

    // Called by "Give Up" button
    public void OnDeclinePressed()
    {
        Time.timeScale = 1f;
        isShowing = false;
        revivePanel.SetActive(false);
        onDecline?.Invoke();
    }
}