using UnityEngine;
using System.Collections;

public class SeaSpiritAI : MonoBehaviour
{
    [Header("Settings")]
    public SpiritMode currentMode = SpiritMode.Normal;
    public float searchRadius = 7f;
    public float followSpeed = 4f;
    public float smoothTime = 0.12f;
    public float attackCooldown = 1.5f;
    public float collectionCooldown = 0.8f;
    private float lastLungeTime;
    public float lungeCooldown = 0.5f;
    public float maxFollowDistance = 10f;
    public Vector3 baseOffset = new Vector3(1.5f, 1.5f, 0);

    [Header("References")]
    public Transform player;

    private SpriteRenderer spriteRenderer;
    private GameObject currentTarget;

    private float lastActionTime;
    private float lastDirectionX = 1f;

    private Vector3 currentVelocity = Vector3.zero;
    private bool isLunging = false;

    void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    void Update()
    {
        if (isLunging) return;

        HandleAutoModeFallback();
        UpdateDirectionAndFlip();

        switch (currentMode)
        {
            case SpiritMode.Normal:
                FollowPlayer();
                break;

            case SpiritMode.Collect:
                HandleCollection();
                break;

            case SpiritMode.Fight:
                HandleCombat();
                break;
        }
    }

    // -----------------------------
    // 🧠 CORE LOGIC
    // -----------------------------

    void HandleAutoModeFallback()
    {
        if (currentMode == SpiritMode.Collect && InventoryManager.Instance != null)
        {
            if (InventoryManager.Instance.IsFull())
            {
                currentMode = SpiritMode.Normal;
                currentTarget = null;
            }
        }
    }

    void UpdateDirectionAndFlip()
    {
        if (player == null) return;

        PlayerMovement pm = player.GetComponent<PlayerMovement>();
        if (pm == null) return;

        lastDirectionX = pm.FacingDirection;

        if (spriteRenderer != null)
            spriteRenderer.flipX = (lastDirectionX < 0);
    }

    // -----------------------------
    // 🟢 NORMAL
    // -----------------------------

    void FollowPlayer()
    {
        if (player == null) return;

        float playerDirection = player.GetComponent<PlayerMovement>().FacingDirection;
        float targetOffsetX = (playerDirection > 0) ? -baseOffset.x : baseOffset.x;

        Vector3 targetPos = player.position + new Vector3(targetOffsetX, baseOffset.y, baseOffset.z);
        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref currentVelocity, smoothTime);
    }

    // -----------------------------
    // 🟢 COLLECT
    // -----------------------------

    void HandleCollection()
    {
        if (currentTarget == null || !currentTarget.CompareTag("Item"))
        {
            currentTarget = FindNearestTarget("Item");
        }

        if (currentTarget == null)
        {
            FollowPlayer();
            return;
        }

        MoveToTarget(currentTarget.transform.position);

        if (Vector3.Distance(transform.position, currentTarget.transform.position) < 0.5f)
        {
            if (Time.time > lastActionTime + collectionCooldown)
            {
                CollectItem();
            }
        }
    }

    void CollectItem()
    {
        var item = currentTarget.GetComponent<CollectibleItem>();

        if (item != null && InventoryManager.Instance != null)
        {
            InventoryManager.Instance.AddItem(item.itemName, 1);
        }

        Destroy(currentTarget);
        currentTarget = null;
        lastActionTime = Time.time;
    }

    // -----------------------------
    // 🔴 COMBAT
    // -----------------------------

    void HandleCombat()
    {
        if (currentTarget == null || !currentTarget.CompareTag("Enemy"))
        {
            currentTarget = FindNearestTarget("Enemy");
        }

        if (currentTarget == null)
        {
            FollowPlayer();
            return;
        }

        MoveToTarget(currentTarget.transform.position);

        if (Vector3.Distance(transform.position, currentTarget.transform.position) < 1.2f)
        {
            if (Time.time > lastActionTime + attackCooldown)
            {
                AttackTarget();
            }
        }
    }

    void AttackTarget()
    {
        var enemy = currentTarget.GetComponent<EnemyAI>();

        if (enemy != null)
        {
            enemy.TakeDamage(10);
        }

        lastActionTime = Time.time;
    }
    // -----------------------------
    // 🔍 SEARCH
    // -----------------------------

    GameObject FindNearestTarget(string tag)
    {
        Collider[] hits = Physics.OverlapSphere(player.position, searchRadius);

        GameObject nearest = null;
        float minDist = Mathf.Infinity;

        foreach (var hit in hits)
        {
            if (hit.CompareTag(tag))
            {
                float dist = Vector3.Distance(player.position, hit.transform.position);

                if (dist < minDist)
                {
                    minDist = dist;
                    nearest = hit.gameObject;
                }
            }
        }

        return nearest;
    }

    void MoveToTarget(Vector3 targetPos)
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPos,
            followSpeed * Time.deltaTime
        );
    }

    // -----------------------------
    // ⚡ LUNGE (เชื่อมกับ UI ใหม่)
    // -----------------------------

    public void StartLunge(Vector3 direction, float distance, float speed)
    {
        if (isLunging) return;

        if (Time.time < lastLungeTime + lungeCooldown)
            return;

        lastLungeTime = Time.time;

        distance = Mathf.Clamp(distance, 0f, 6f);
        StartCoroutine(LungeRoutine(direction, distance, speed));
    }

    void OnTriggerEnter(Collider other)
    {
        if (isLunging)
        {
            // 🟢 เก็บของ
            if (currentMode == SpiritMode.Collect && other.CompareTag("Item"))
            {
                if (InventoryManager.Instance != null && !InventoryManager.Instance.IsFull())
                {
                    var item = other.GetComponent<CollectibleItem>();
                    if (item != null)
                    {
                        InventoryManager.Instance.AddItem(item.itemName, 1);
                        Destroy(other.gameObject);
                    }
                }
            }

            // 🔴 ตีศัตรู
            if (currentMode == SpiritMode.Fight && other.CompareTag("Enemy"))
            {
                var enemy = other.GetComponent<EnemyAI>();
                if (enemy != null)
                {
                    enemy.TakeDamage(10);
                }
            }
        }
    }

    IEnumerator LungeRoutine(Vector3 direction, float distance, float speed)
    {
        isLunging = true;

        Vector3 startPos = transform.position;
        Vector3 targetPos = startPos + direction * distance;
        Vector3 relativeTarget = targetPos - player.position;

        if (relativeTarget.magnitude > maxFollowDistance)
        {
            relativeTarget = relativeTarget.normalized * maxFollowDistance;
            targetPos = player.position + relativeTarget;
        }
        // -----------------------------------------

        float duration = distance / speed;
        float timer = 0f;

        while (timer < duration)
        {
            float t = timer / duration;
            t = 1 - Mathf.Pow(1 - t, 3); // ease out

            transform.position = Vector3.Lerp(startPos, targetPos, t);

            timer += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPos;
        yield return new WaitForSeconds(lungeCooldown);
        isLunging = false;
    }
}