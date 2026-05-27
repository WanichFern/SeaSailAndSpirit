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

        // Use regular deltaTime — no timeScale pausing
        timer -= Time.deltaTime;
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

        // Don't pause time — just disable player input instead
        DisablePlayerInput();
    }

    void DisablePlayerInput()
    {
        // Disable movement and interaction while dead
        PlayerMovement pm = GameObject
            .FindGameObjectWithTag("Player")
            ?.GetComponent<PlayerMovement>();
        if (pm != null) pm.enabled = false;

        PlayerInteraction pi = GameObject
            .FindGameObjectWithTag("Player")
            ?.GetComponent<PlayerInteraction>();
        if (pi != null) pi.enabled = false;
    }

    void EnablePlayerInput()
    {
        PlayerMovement pm = GameObject
            .FindGameObjectWithTag("Player")
            ?.GetComponent<PlayerMovement>();
        if (pm != null) pm.enabled = true;

        PlayerInteraction pi = GameObject
            .FindGameObjectWithTag("Player")
            ?.GetComponent<PlayerInteraction>();
        if (pi != null) pi.enabled = true;
    }

    // Called by "Watch Ad" button
    public void OnWatchAdPressed()
    {
        isShowing = false;
        revivePanel.SetActive(false);

        bool didShow = RewardedAdController.Instance.TryShowRewarded(
            onRewardGranted: () =>
            {
                EnablePlayerInput();
                onRevive?.Invoke();
            },
            onFailed: () =>
            {
                EnablePlayerInput();
                onDecline?.Invoke();
            }
        );

        if (!didShow)
        {
            EnablePlayerInput();
            onDecline?.Invoke();
        }
    }

    // Called by "Give Up" button
    public void OnDeclinePressed()
    {
        isShowing = false;
        revivePanel.SetActive(false);
        EnablePlayerInput();
        onDecline?.Invoke();
    }
}