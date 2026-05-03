using System;
using UnityEngine;

public class GoblinAI : MonoBehaviour
{
    public Transform player;
    public float speed = 3f;
    public float attackRange = 2f;

    public bool Activated = false; // detection system

    private Animator animator;
    public EnemyHealth health;
    private Enemy_Cluster_Scr cluster;

    private Vector3 knockbackVelocity = Vector3.zero;
    public float knockbackDecay = 5f;

    private float damageAmount = 5;

    private Boolean playerInRange;

    private float aggroRange = 10;

    void Start()
    {
        animator = GetComponent<Animator>();
        health = GetComponent<EnemyHealth>();

        Debug.Log(transform.parent);

        cluster = transform.parent.GetComponent<Enemy_Cluster_Scr>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    public void ApplyKnockback(Vector3 force)
    {
        knockbackVelocity += force;
    }

    void Update()
    {
        if (health.getIsDead()) return;

        if (!Activated) 
        {
            if (health.health < health.maxHealth)
            {
                cluster.detect_Player();
                Debug.Log("Detected player due to damage.");
                return;
            }

            Vector3 diff = player.position - transform.position;

            if (diff.sqrMagnitude <= aggroRange * aggroRange)
            {
                cluster.detect_Player();
                Debug.Log("Detected player due to distance.");
                return;
            }

        }
        
        else
        {
            float distance = Vector3.Distance(transform.position, player.position);

            if (knockbackVelocity.magnitude > 0.05f)
            {
                transform.position += knockbackVelocity * Time.deltaTime;
                knockbackVelocity = Vector3.Lerp(knockbackVelocity, Vector3.zero, knockbackDecay * Time.deltaTime);
                return;
            }
            else
            {
                knockbackVelocity = Vector3.zero;
            }

            if (distance > attackRange)
            {
                playerInRange = false;
                MoveTowardPlayer();
            }
            else
            {
                playerInRange = true;
                Attack();
            }
        }
    }

    void MoveTowardPlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;

        transform.LookAt(transform.position - (player.position - transform.position));

        animator.SetBool("isWalking", true);
        animator.SetBool("isAttacking", false);
    }

    void Attack()
    {
        animator.SetBool("isWalking", false);
        animator.SetBool("isAttacking", true);
    }

    public void DealDamage()
    {
        if (playerInRange)
        {
            Debug.Log("Attacking player...");
            player.GetComponent<PlayerHealth>()?.TakeDamage(damageAmount);
        }
    }

    public void DetectPlayer()
    {
        Activated = true;
    }
}