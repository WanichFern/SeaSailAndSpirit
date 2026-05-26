using UnityEngine;

public enum BoatState
{
    Docked,
    Sailing,
    AtIsland
}

public class BoatController : MonoBehaviour
{
    public static BoatController Instance;

    [Header("Sailing Settings")]
    public float sailSpeed = 8f;
    public float playerMoveSpeed = 5f;
    public float verticalClamp = 5f;   // max up/down from center
    public float centerZ = 0f;         // the Z center of the sea lane

    [Header("References")]
    public Transform playerSeatPoint;  // where player sits on boat
    public LandingMarker landingMarker;
    public SpriteRenderer boatVisualRenderer;

    [Header("Dock")]
    public Transform dockSpawnPoint;   // assigned by BoatDock

    private BoatState state = BoatState.Docked;
    private PlayerMovement playerMovement;
    private PlayerStats playerStats;
    private Rigidbody rb;
    private bool playerOnBoard = false;
    private SpriteRenderer boatSprite;

    public bool IsPlayerOnBoard => playerOnBoard;

    void Awake()
    {
        Instance = this;
        rb = GetComponent<Rigidbody>();
    }

    void UpdateBoatFacing()
    {
        if (boatSprite != null)
            boatSprite.flipX = sailDirection < 0;
    }

    // Called when player interacts with the boat at dock
    public void TryBoard()
    {
        if (state == BoatState.Docked && !playerOnBoard)
        {
            BoardBoat();
        }
        else if (state == BoatState.AtIsland && playerOnBoard)
        {
            ExitBoat();
        }
    }

    void BoardBoat()
    {
        GameObject playerObj =
            GameObject.FindGameObjectWithTag("Player");
        playerMovement = playerObj.GetComponent<PlayerMovement>();
        playerStats = playerObj.GetComponent<PlayerStats>();

        if (playerObj.transform.position.x > 50f)
            sailDirection = -1f;
        else
        {
            sailDirection = 1f;
            IslandGenerator.Instance.GenerateNewIsland();
        }

        playerObj.transform.position = playerSeatPoint.position
            + Vector3.up * 3f;
        playerObj.transform.SetParent(transform);

        // Pass boat reference so player locks to seat
        playerMovement.SetBoatMode(true, verticalClamp, centerZ, this);
        playerOnBoard = true;
        state = BoatState.Sailing;

        BoatProximityUI.Instance?.ForceHide();
        UpdateBoatFacing();
    }

    void ExitBoat()
    {
        GameObject playerObj =
            GameObject.FindGameObjectWithTag("Player");

        // Detach player from boat
        playerObj.transform.SetParent(null);
        playerMovement.SetBoatMode(false, 0, 0);

        playerOnBoard = false;
        state = BoatState.Docked; // boat stays, player is off

        landingMarker?.Hide();

        ZoneTrigger.DropIslandCam();

        Debug.Log("Exited boat on island!");
    }

    private float sailDirection = 1f; // +1 = right, -1 = left

    void FixedUpdate()
    {
        if (!playerOnBoard) return;

        Vector3 pos = rb.position;

        if (state == BoatState.Sailing)
        {
            // Auto-sail horizontally
            pos.x += sailDirection * sailSpeed * Time.fixedDeltaTime;

            // Player input controls vertical (Z axis)
            float verticalInput = GetPlayerVerticalInput();
            pos.z += verticalInput * playerMoveSpeed * Time.fixedDeltaTime;

            // Clamp Z in sea zone
            pos.z = Mathf.Clamp(pos.z,
                centerZ - verticalClamp,
                centerZ + verticalClamp);
        }
        else if (state == BoatState.AtIsland)
        {
            // Free movement around island — player steers with joystick
            float verticalInput = GetPlayerVerticalInput();
            float horizontalInput = GetPlayerHorizontalInput();

            pos.x += horizontalInput * playerMoveSpeed * Time.fixedDeltaTime;
            pos.z += verticalInput * playerMoveSpeed * Time.fixedDeltaTime;

            // Clamp around island area
            pos.x = Mathf.Clamp(pos.x, 120f, 180f);
            pos.z = Mathf.Clamp(pos.z, -30f, 30f);
        }

        rb.MovePosition(pos);
    }

    float GetPlayerVerticalInput()
    {
        PlayerMovement pm = GameObject
            .FindGameObjectWithTag("Player")
            ?.GetComponent<PlayerMovement>();
        return pm != null ? pm.GetInputVector().y : 0f;
    }

    float GetPlayerHorizontalInput()
    {
        PlayerMovement pm = GameObject
            .FindGameObjectWithTag("Player")
            ?.GetComponent<PlayerMovement>();
        return pm != null ? pm.GetInputVector().x : 0f;
    }

    public void EnterIslandZone()
    {
        state = BoatState.AtIsland;
        landingMarker?.Show();
        Debug.Log("Entered island zone!");
    }

    // Called by ZoneTrigger when exiting sea zone going back home
    public void EnterHomeZone()
    {
        if (playerOnBoard)
        {
            // Auto-dock when reaching home
            ReturnToDock();
        }
    }


    void ReturnToDock()
    {
        GameObject playerObj =
            GameObject.FindGameObjectWithTag("Player");

        playerObj.transform.SetParent(null);
        playerMovement.SetBoatMode(false, 0, 0);

        // Transfer items to chest
        InventoryManager.Instance.TransferToChest();

        playerOnBoard = false;
        state = BoatState.Docked;

        // Move boat back to dock
        transform.position = dockSpawnPoint.position;

        Debug.Log("Returned home! Items transferred to chest.");
    }

    // Called when player dies on the island
    public void RespawnAtDock()
    {
        if (playerOnBoard)
        {
            GameObject playerObj =
                GameObject.FindGameObjectWithTag("Player");
            playerObj.transform.SetParent(null);
            playerMovement.SetBoatMode(false, 0, 0);
            playerOnBoard = false;
        }

        state = BoatState.Docked;
        transform.position = dockSpawnPoint.position;
        Debug.Log("Boat respawned at dock.");
    }
}