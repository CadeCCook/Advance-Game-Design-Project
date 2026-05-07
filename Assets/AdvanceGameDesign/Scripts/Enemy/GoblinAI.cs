using System;
using UnityEngine;
using System.Collections;

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

    private Coroutine stunCoroutine;
    public bool isStunned {get; private set;}
    private Coroutine slowCoroutine;

    public bool isSlowed { get; private set; }

    private float originalSpeed;

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

    public void ApplyStun(float duration)
    {
        if (stunCoroutine != null)
        {
            StopCoroutine(stunCoroutine);
        }
        stunCoroutine = StartCoroutine(StunCoroutine(duration));
    }

    void Update()
    {
        if (health.getIsDead()) return;
        if (isStunned) return;

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

    private IEnumerator StunCoroutine(float duration)
    {
        isStunned = true;
        animator.SetBool("isWalking", false);
        animator.SetBool("isAttacking", false);

        yield return new WaitForSeconds(duration);

        isStunned = false;
        stunCoroutine = null;
    }

    public void ApplySlow(float slowPercent, float duration)
    {
        if (slowCoroutine != null)
            StopCoroutine(slowCoroutine);
        slowCoroutine = StartCoroutine(SlowCoroutine(slowPercent, duration));
    }

    private IEnumerator SlowCoroutine(float slowPercent, float duration)
    {
        if (!isSlowed)
            originalSpeed = speed;

        isSlowed = true;
        speed = originalSpeed * (1f - slowPercent);

        yield return new WaitForSeconds(duration);

        speed = originalSpeed;
        isSlowed = false;
        slowCoroutine = null;
    }
}