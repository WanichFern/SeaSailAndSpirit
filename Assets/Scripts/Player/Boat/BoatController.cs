using UnityEngine;
using System.Collections;

public enum BoatState { Docked, SailingToIsland, SailingHome }

public class BoatController : MonoBehaviour
{
    public static BoatController Instance;

    [Header("Sailing Settings")]
    public float sailSpeed = 8f;
    public float playerMoveSpeed = 5f;
    public float verticalClamp = 5f;
    public float centerZ = 0f;

    [Header("Island Settings")]
    public float islandXPosition = 150f;
    public float homeXPosition = 0f;

    [Header("References")]
    public Transform playerSeatPoint;
    public Transform dockSpawnPoint;
    public Transform dockLanding;
    public ScreenFader fader;

    private BoatState state = BoatState.Docked;
    private Rigidbody rb;
    private PlayerMovement playerMovement;
    private bool playerOnBoard = false;
    private SpriteRenderer boatSprite;

    private bool headingToIsland = true;

    public bool IsHeadingToIsland =>
    state == BoatState.SailingToIsland;

    void Awake()
    {
        Instance = this;
        rb = GetComponent<Rigidbody>();
        boatSprite = GetComponentInChildren<SpriteRenderer>();
    }

    public void BoardBoat()
    {
        if (playerOnBoard) return;

        GameObject playerObj =
            GameObject.FindGameObjectWithTag("Player");
        playerMovement = playerObj.GetComponent<PlayerMovement>();

        float distToIsland = Mathf.Abs(
            transform.position.x - islandXPosition);
        float distToHome = Mathf.Abs(
            transform.position.x - homeXPosition);

        if (distToIsland < distToHome)
        {
            headingToIsland = false;
            state = BoatState.SailingHome;
            Debug.Log("Sailing home");
        }
        else
        {
            headingToIsland = true;
            state = BoatState.SailingToIsland;
            IslandGenerator.Instance.GenerateNewIsland();
            Debug.Log("Sailing to island");
        }

        playerObj.transform.position =
            playerSeatPoint.position + Vector3.up * 0.5f;
        playerObj.transform.SetParent(transform);
        playerMovement.SetBoatMode(true, verticalClamp, centerZ, this);
        playerOnBoard = true;

        UpdateBoatFacing();
        BoatProximityUI.Instance?.ForceHide();
    }

    void FixedUpdate()
    {
        if (!playerOnBoard) return;

        Vector3 pos = rb.position;
        float vertInput = playerMovement?.GetInputVector().y ?? 0f;

        if (state == BoatState.SailingToIsland)
        {
            pos.x += sailSpeed * Time.fixedDeltaTime;
        }
        else if (state == BoatState.SailingHome)
        {
            pos.x -= sailSpeed * Time.fixedDeltaTime;
        }

        pos.z += vertInput * playerMoveSpeed * Time.fixedDeltaTime;
        pos.z = Mathf.Clamp(pos.z,
            centerZ - verticalClamp,
            centerZ + verticalClamp);

        rb.MovePosition(pos);
    }

    public void OnReachedIsland()
    {
        if (state != BoatState.SailingToIsland) return;
        StartCoroutine(ArriveAtIsland());
    }

    public void OnReachedHome()
    {
        if (state != BoatState.SailingHome) return;
        StartCoroutine(ArriveAtHome());
    }

    IEnumerator ArriveAtIsland()
    {
        // Stop sailing while coroutine runs
        state = BoatState.Docked;

        yield return fader.Fade(true);

        GameObject playerObj =
            GameObject.FindGameObjectWithTag("Player");

        playerObj.transform.SetParent(null);
        playerMovement.SetBoatMode(false, 0, 0, null);
        playerOnBoard = false;

        // Spawn player at island center
        Vector3 islandCenter = IslandGenerator.Instance
            .GetIslandCenter();
        playerObj.transform.position = islandCenter;

        // Move boat to random edge of island
        Vector3 edgePos = IslandGenerator.Instance
            .GetRandomEdgePosition();
        transform.position = edgePos;

        yield return fader.Fade(false);

        Debug.Log("Arrived at island!");
    }

    IEnumerator ArriveAtHome()
    {
        // Stop sailing while coroutine runs
        state = BoatState.Docked;

        yield return fader.Fade(true);

        GameObject playerObj =
            GameObject.FindGameObjectWithTag("Player");

        playerObj.transform.SetParent(null);
        playerMovement.SetBoatMode(false, 0, 0, null);
        playerOnBoard = false;

        // Spawn player at dock landing
        playerObj.transform.position = dockLanding.position;

        PlayerStats stats = playerObj.GetComponent<PlayerStats>();
        if (stats != null)
        {
            stats.currentHP = stats.totalMaxHP;
            Debug.Log("Player healed to full HP on return home.");
        }

        // Move boat back to dock
        transform.position = dockSpawnPoint.position;

        yield return fader.Fade(false);

        Debug.Log("Returned home!");
    }

    public void RespawnAtDock()
    {
        if (playerOnBoard)
        {
            GameObject playerObj =
                GameObject.FindGameObjectWithTag("Player");
            playerObj.transform.SetParent(null);
            playerMovement?.SetBoatMode(false, 0, 0, null);
            playerOnBoard = false;
        }

        transform.position = dockSpawnPoint.position;
        state = BoatState.Docked;
        Debug.Log("Boat respawned at dock.");
    }

    void UpdateBoatFacing()
    {
        if (boatSprite != null)
            boatSprite.flipX = !headingToIsland;
    }
}