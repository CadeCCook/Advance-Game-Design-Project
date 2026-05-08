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

    private Vector3 knockbackVelocity = Vector3.zero;
    public float knockbackDecay = 5f;

    private Coroutine slowCoroutine;
    private bool isSlowed = false;
    private float originalSpeed;

    private Coroutine stunCoroutine;
    public bool isStunned { get; private set; }

    void Start()
    {
        animator = GetComponent<Animator>();
        col = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();

        if (rb != null)
            rb.isKinematic = true;

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
        if (!isAlive || player == null || isStunned) return;

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

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance > attackRange)
            MoveTowardPlayer();
        else
            Attack();
    }

    void MoveTowardPlayer()
    {
        Vector3 direction = player.position - transform.position;
        direction.y = 0;
        direction.Normalize();

        transform.position += direction * speed * Time.deltaTime;
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
            player.GetComponent<PlayerHealth>()?.TakeDamage(damage);
    }

    public void ApplyKnockback(Vector3 force)
    {
        knockbackVelocity += force;
    }

    public void ApplyStun(float duration)
    {
        if (stunCoroutine != null)
            StopCoroutine(stunCoroutine);
        stunCoroutine = StartCoroutine(StunCoroutine(duration));
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

    public void TakeDamage(float amount)
    {
        if (!isAlive) return;

        Debug.Log("Skeleton taking damage: " + amount + ". Health: " + health);

        health -= amount;
        if (health < 0) health = 0;

        Vector2 current = healthFill.rectTransform.sizeDelta;
        Vector2 newSize = new Vector2(health * 2, current.y);
        healthFill.rectTransform.sizeDelta = newSize;

        if (health <= 0)
            Die();
    }

    public void Revive()
    {
        StartCoroutine(ReviveRoutine());
    }

    IEnumerator ReviveRoutine()
    {
        health = maxHealth;

        Vector2 current = healthFill.rectTransform.sizeDelta;
        Vector2 newSize = new Vector2(health * 2, current.y);
        healthFill.rectTransform.sizeDelta = newSize;

        animator.SetTrigger("Revive");

        yield return new WaitForSeconds(5.5f);

        if (rb != null)
            rb.isKinematic = true;

        knockbackVelocity = Vector3.zero;

        isAlive = true;

        if (col != null)
            col.enabled = true;

        animator.SetBool("isWalking", false);
        animator.SetBool("isAttacking", false);
    }

    void Die()
    {
        isAlive = false;
        knockbackVelocity = Vector3.zero;

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        animator.SetBool("isWalking", false);
        animator.SetBool("isAttacking", false);
        animator.SetTrigger("Die");

        if (col != null)
            col.enabled = false;
    }

    void SetDeadState()
    {
        Die();
        health = 0;
        isAlive = false;

        if (col != null)
            col.enabled = false;
    }
}