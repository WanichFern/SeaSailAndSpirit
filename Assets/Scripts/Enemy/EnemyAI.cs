using UnityEngine;

public class EnemyAI : MonoBehaviour, IDamageable
{
    public EnemyStatsSO enemyStats;

    [Header("Animation Setup")]
    public Animator animator;          
    public SpriteRenderer spriteRenderer;

    private Transform player;
    private float currentHP;
    private float lastAttackTime;

    private bool isMovingThisFrame;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        if (enemyStats != null) currentHP = enemyStats.maxHP;

        if (animator == null) animator = GetComponentInChildren<Animator>();
        if (spriteRenderer == null) spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    void Update()
    {
        if (player == null || enemyStats == null) return;

        float distance = Vector3.Distance(transform.position, player.position);
        isMovingThisFrame = false;

        if (distance <= enemyStats.attackRange)
        {
            Attack();
        }
        else if (distance <= enemyStats.chaseRange)
        {
            ChasePlayer();
        }

        UpdateAnimationState();
    }

    void ChasePlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0;
        transform.position += direction * enemyStats.walkSpeed * Time.deltaTime;

        isMovingThisFrame = true;

        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = (direction.x < 0);
        }
    }

    void Attack()
    {
        if (spriteRenderer != null)
        {
            Vector3 direction = (player.position - transform.position).normalized;
            spriteRenderer.flipX = (direction.x < 0);
        }

        if (Time.time >= lastAttackTime + enemyStats.attackCooldown)
        {
            if (animator != null)
            {
                animator.SetTrigger("Attack");
            }

            PlayerStats pStats = player.GetComponent<PlayerStats>();
            if (pStats != null)
            {
                pStats.TakeDamage(enemyStats.damage);
                lastAttackTime = Time.time;
            }
        }
    }

    void UpdateAnimationState()
    {
        if (animator == null) return;

        int stateValue = isMovingThisFrame ? 1 : 0;
        animator.SetInteger("State", stateValue);
    }

    public void TakeDamage(float damage)
    {
        currentHP -= damage;
        if (currentHP <= 0) Die();
    }

    void Die()
    {
        DropLoot();
        Destroy(gameObject);
    }

    void DropLoot()
    {
        if (enemyStats.dropItemPrefab != null)
        {
            float randomRoll = Random.Range(0f, 100f);
            if (randomRoll <= enemyStats.dropChance)
            {
                Instantiate(enemyStats.dropItemPrefab, transform.position, Quaternion.identity);
                Debug.Log($"{enemyStats.enemyName} dropped an item!");
            }
        }
    }
}