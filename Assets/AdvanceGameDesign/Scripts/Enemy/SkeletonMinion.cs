using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SkeletonMinion : MonoBehaviour
{
    public Transform player;
    private Rigidbody rb;

    public float health = 50f;
    public float maxHealth = 50f;
    public Image healthFill;

    public float speed = 2.5f;
    public float attackRange = 1.5f;

    public float damage = 8f;

    public bool isAlive = false;

    private Animator animator;
    private Collider col;


    void Start()
    {
        animator = GetComponent<Animator>();
        col = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }

        SetDeadState();

    }

    void Update()
    {
        if (!isAlive || player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance > attackRange)
        {
            MoveTowardPlayer();
        }
        else
        {
            Attack();
        }
    }

    void MoveTowardPlayer()
    {
        Vector3 direction = player.position - transform.position;
        direction.y = 0;
        direction.Normalize();

        if (rb != null)
            {
                rb.MovePosition(rb.position + direction * speed * Time.deltaTime);
            }
        else
            {
                transform.position += direction * speed * Time.deltaTime;
            }

        transform.LookAt(transform.position + direction);

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
        if (!isAlive || player == null) return;

        if (Vector3.Distance(transform.position, player.position) <= attackRange)
        {
            player.GetComponent<PlayerHealth>()?.TakeDamage(damage);
        }
    }

    public void TakeDamage(float amount)
    {
        if (!isAlive) return;

        Debug.Log("Skeleton taking damage: " + amount + ". Health: " + health);

        health -= amount;

        Vector2 current = healthFill.rectTransform.sizeDelta;

        if (health < 0) health = 0;

        Vector2 newSize = new Vector2(health, current.y);

        healthFill.rectTransform.sizeDelta = newSize;

        if (health <= 0)
        {
            Die();
        }
    }

    public void Revive()
    {
        StartCoroutine(ReviveRoutine());
    }

    IEnumerator ReviveRoutine()
    {
        health = maxHealth;

        Vector2 current = healthFill.rectTransform.sizeDelta;

        Vector2 newSize = new Vector2(health, current.y);

        healthFill.rectTransform.sizeDelta = newSize;

        animator.SetTrigger("Revive");

        yield return new WaitForSeconds(5.5f);

        isAlive = true;

        if (col != null)
        col.enabled = true;

        animator.SetBool("isWalking", false);
        animator.SetBool("isAttacking", false);
    }

    void Die()
    {
        isAlive = false;

        animator.SetTrigger("Die");

        if (col != null)
            col.enabled = false;
    }

    void SetDeadState()
    {
        isAlive = false;

        if (col != null)
            col.enabled = false;
    }
}