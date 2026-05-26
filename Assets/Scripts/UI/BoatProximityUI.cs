using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BoatProximityUI : MonoBehaviour
{
    public static BoatProximityUI Instance;

    [Header("UI References")]
    public GameObject boatButtonPanel;
    public TextMeshProUGUI boatButtonText;

    [Header("Settings")]
    public float proximityRange = 3f;

    private Transform player;
    private BoatController nearbyBoat;
    private bool isShowing = false;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        boatButtonPanel.SetActive(false);
    }

    void Update()
    {
        if (player == null) return;

        CheckBoatProximity();
    }

    void CheckBoatProximity()
    {
        // Find the boat in the scene
        BoatController boat = BoatController.Instance;
        if (boat == null) return;

        float distance = Vector3.Distance(
            player.position,
            boat.transform.position);

        bool shouldShow = distance <= proximityRange;

        if (shouldShow && !isShowing)
        {
            ShowButton(boat);
        }
        else if (!shouldShow && isShowing)
        {
            HideButton();
        }
    }

    void ShowButton(BoatController boat)
    {
        nearbyBoat = boat;
        isShowing = true;
        boatButtonPanel.SetActive(true);

        // Change button text based on where player is
        // If player is far right (near island), show "Sail Home"
        // If player is near home, show "Set Sail"
        if (player.position.x > 50f)
            boatButtonText.text = "Sail Home";
        else
            boatButtonText.text = "Set Sail";
    }

    void HideButton()
    {
        nearbyBoat = null;
        isShowing = false;
        boatButtonPanel.SetActive(false);
    }

    // This is what the button calls OnClick
    public void OnBoatButtonPressed()
    {
        if (nearbyBoat != null)
        {
            nearbyBoat.TryBoard();
            HideButton();
        }
    }

    // Call this from BoatController when player boards
    // so the button hides immediately
    public void ForceHide()
    {
        HideButton();
    }
}