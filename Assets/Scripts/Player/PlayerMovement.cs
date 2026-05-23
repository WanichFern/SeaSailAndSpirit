using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody rb;
    private Vector2 inputVector = Vector2.zero;
    private PlayerInputActions controls;
    private PlayerStats pStats;

    public float FacingDirection { get; private set; } = 1f;

    // 1. ฟังก์ชันนี้จะถูกเรียกตอนสร้าง Object
    void Awake()
    {
        InitInput();
        controls = new PlayerInputActions();
        // เชื่อมต่อกับ Script PlayerStats ที่อยู่ในตัวละครเดียวกัน
        pStats = GetComponent<PlayerStats>();
    }

    // 2. ฟังก์ชันเช็คความปลอดภัย (ถ้ายังไม่มี controls ให้สร้างใหม่)
    void InitInput()
    {
        if (controls == null)
        {
            controls = new PlayerInputActions();
        }
    }

    // 3. เมื่อเปิดใช้งาน Component
    void OnEnable()
    {
        InitInput(); // เช็คอีกรอบเพื่อความชัวร์
        controls.Enable();
    }

    // 4. เมื่อปิดการใช้งาน ใช้เครื่องหมาย ?. (แปลว่า ถ้ามีค่าถึงจะสั่ง Disable ป้องกัน Null)
    void OnDisable()
    {
        controls?.Disable();
    }

    void Update()
    {
        inputVector = controls.Player.Move.ReadValue<Vector2>();

        // ถ้ามี input ทางซ้าย/ขวา → อัปเดตทิศ
        if (Mathf.Abs(inputVector.x) > 0.1f)
        {
            FacingDirection = Mathf.Sign(inputVector.x);
        }
    }

    // แก้ไขแค่ฟังก์ชัน FixedUpdate ใน PlayerMovement.cs
    void FixedUpdate()
    {
        Vector3 movement = new Vector3(inputVector.x, 0, inputVector.y);

        if (movement.magnitude > 0.1f)
        {
            float speed = pStats.totalWalkSpeed;
            rb.MovePosition(rb.position + movement.normalized * speed * Time.fixedDeltaTime);
        }
    }
}