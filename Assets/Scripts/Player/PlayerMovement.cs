using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody rb;
    public Vector2 inputVector = Vector2.zero;
    private PlayerInputActions controls;
    private PlayerStats pStats;
    private BoatController currentBoat;
    public Vector2 GetInputVector() => inputVector;

    public float FacingDirection { get; private set; } = 1f;

    // Boat mode fields
    private bool inBoatMode = false;
    private float boatVerticalClamp = 5f;
    private float boatCenterZ = 0f;

    void Awake()
    {
        InitInput();
        pStats = GetComponent<PlayerStats>();
    }

    void InitInput()
    {
        if (controls == null)
            controls = new PlayerInputActions();
    }

    void OnEnable()
    {
        InitInput();
        controls.Enable();
    }

    void OnDisable()
    {
        controls?.Disable();
    }

    void Update()
    {
        inputVector = controls.Player.Move.ReadValue<Vector2>();

        if (Mathf.Abs(inputVector.x) > 0.1f)
            FacingDirection = Mathf.Sign(inputVector.x);
    }

    public void SetBoatMode(bool enabled, float clamp,
    float centerZ, BoatController boat = null)
    {
        inBoatMode = enabled;
        boatVerticalClamp = clamp;
        boatCenterZ = centerZ;
        currentBoat = boat;

        if (rb != null)
        {
            if (enabled)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.isKinematic = true;
            }
            else
            {
                rb.isKinematic = false;
                currentBoat = null;
            }
        }
    }

    void FixedUpdate()
    {
        if (inBoatMode)
        {
            // Player is locked to seat — boat moves instead
            // Lock player to seat point exactly
            if (currentBoat != null)
            {
                transform.localPosition = Vector3.zero;
            }
            // Actual boat movement is handled in BoatController
        }
        else
        {
            Vector3 movement = new Vector3(inputVector.x, 0, inputVector.y);
            if (movement.magnitude > 0.1f)
            {
                float speed = pStats.totalWalkSpeed;
                rb.MovePosition(rb.position +
                    movement.normalized * speed * Time.fixedDeltaTime);
            }
        }
    }
}