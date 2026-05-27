using UnityEngine;
using UnityEngine.UI;

public class PlayerHPUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private Slider hpSlider;

    void Start()
    {
        if (playerStats == null) playerStats = Object.FindAnyObjectByType<PlayerStats>();
        if (hpSlider == null) hpSlider = GetComponent<Slider>();

        UpdateHPBar();
    }

    void Update()
    {
        UpdateHPBar();
    }

    private void UpdateHPBar()
    {
        if (playerStats == null || hpSlider == null) return;

        if (playerStats.totalMaxHP <= 0) return;

        // Calculate health percentage between 0.0 and 1.0
        float hpPercentage = playerStats.currentHP / playerStats.totalMaxHP;

        // Update slider value
        hpSlider.value = Mathf.Clamp01(hpPercentage);
    }
}