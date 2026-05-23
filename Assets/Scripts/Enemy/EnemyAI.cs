using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public EnemyStatsSO enemyStats; // ลาก SO ของศัตรูแต่ละตัวมาใส่ที่นี่

    private Transform player;
    private float currentHP;
    private float lastAttackTime;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        if (enemyStats != null) currentHP = enemyStats.maxHP;
    }

    void Update()
    {
        if (player == null || enemyStats == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= enemyStats.attackRange)
        {
            Attack();
        }
        else if (distance <= enemyStats.chaseRange)
        {
            ChasePlayer();
        }
    }

    void ChasePlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0;
        transform.position += direction * enemyStats.walkSpeed * Time.deltaTime;
    }

    void Attack()
    {
        if (Time.time >= lastAttackTime + enemyStats.attackCooldown)
        {
            PlayerStats pStats = player.GetComponent<PlayerStats>();
            if (pStats != null)
            {
                pStats.TakeDamage(enemyStats.damage);
                lastAttackTime = Time.time;
            }
        }
    }

    public void TakeDamage(float damage)
    {
        currentHP -= damage;
        if (currentHP <= 0) Die();
    }

    void Die()
    {
        // ระบบดรอปไอเทมก่อนทำลายตัวเอง
        DropLoot();
        Destroy(gameObject);
    }

    void DropLoot()
    {
        if (enemyStats.dropItemPrefab != null)
        {
            // สุ่มโอกาสดรอป (Algorithm: Probability Check)
            float randomRoll = Random.Range(0f, 100f);
            if (randomRoll <= enemyStats.dropChance)
            {
                Instantiate(enemyStats.dropItemPrefab, transform.position, Quaternion.identity);
                Debug.Log($"{enemyStats.enemyName} dropped an item!");
            }
        }
    }
}