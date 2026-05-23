using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class SeaSpiritUIControl : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [Header("UI")]
    public TextMeshProUGUI modeText;
    public GameObject extensionPanel;
    public Image chargingBlueArea;

    [Header("Lunge Settings")]
    public float minLungeDistance = 2f;
    public float maxLungeDistance = 10f;
    public float lungeSpeed = 15f;

    [Header("Input Settings")]
    public float dragThreshold = 20f;

    private Vector2 startDragPosition;
    private Vector2 currentDragPosition;

    private bool isDragging = false;

    private SeaSpiritAI ai;

    void Start()
    {
        ai = Object.FindAnyObjectByType<SeaSpiritAI>();

        if (extensionPanel != null)
            extensionPanel.SetActive(false);

        if (chargingBlueArea != null)
            chargingBlueArea.gameObject.SetActive(false);
    }

    // 👉 กดลง
    public void OnPointerDown(PointerEventData eventData)
    {
        startDragPosition = eventData.position;
        isDragging = false;
    }

    // 👉 ลาก
    public void OnDrag(PointerEventData eventData)
    {
        currentDragPosition = eventData.position;

        float dragDistance = Vector2.Distance(currentDragPosition, startDragPosition);

        if (dragDistance > dragThreshold)
        {
            if (!isDragging)
                EnterAimingState();

            isDragging = true;

            Vector2 dir = (currentDragPosition - startDragPosition).normalized;

            // 🎯 ปรับขนาด UI
            UpdateAimVisual(dragDistance);

            // เก็บ direction
            targetDirection = dir;
        }
    }

    // 👉 ปล่อย
    public void OnPointerUp(PointerEventData eventData)
    {
        if (isDragging)
        {
            TriggerLunge();
        }
        else
        {
            ToggleExtensionPanel();
        }

        ResetAim();
    }

    public void SetMode(SpiritMode mode)
    {
        if (ai == null) return;

        ai.currentMode = mode;
        UpdateMainButtonText();

        if (extensionPanel != null)
            extensionPanel.SetActive(false);
    }

    public void SetMode_Normal()
    {
        SetMode(SpiritMode.Normal);
    }

    public void SetMode_Collect()
    {
        SetMode(SpiritMode.Collect);
    }

    public void SetMode_Fight()
    {
        SetMode(SpiritMode.Fight);
    }

    void UpdateMainButtonText()
    {
        if (modeText != null && ai != null)
        {
            modeText.text = ai.currentMode.ToString().ToLower();
        }
    }

    // ----------------------

    private Vector2 targetDirection;

    void EnterAimingState()
    {
        if (chargingBlueArea != null)
        {
            chargingBlueArea.gameObject.SetActive(true);
        }
    }

    void UpdateAimVisual(float dragDistance)
    {
        if (chargingBlueArea == null) return;

        RectTransform rect = chargingBlueArea.rectTransform;

        float size = Mathf.Clamp(dragDistance * 2f, 100f, 400f);
        rect.sizeDelta = Vector2.Lerp(rect.sizeDelta, new Vector2(size, size), Time.deltaTime * 15f);
    }

    void TriggerLunge()
    {
        if (ai == null || targetDirection == Vector2.zero) return;

        float dragDistance = Vector2.Distance(currentDragPosition, startDragPosition);

        float t = Mathf.Clamp01(dragDistance / 350f);

        float distance = Mathf.Lerp(minLungeDistance, maxLungeDistance, t);

        Vector3 worldDir = new Vector3(targetDirection.x, 0, targetDirection.y);

        ai.StartLunge(worldDir, distance, lungeSpeed);
    }

    void ToggleExtensionPanel()
    {
        if (extensionPanel != null)
            extensionPanel.SetActive(!extensionPanel.activeSelf);
    }

    void ResetAim()
    {
        isDragging = false;

        if (chargingBlueArea != null)
            chargingBlueArea.gameObject.SetActive(false);
    }
}