using UnityEngine;

public class GoblinAI : MonoBehaviour
{
    public Transform player;
    public float speed = 3f;
    public float attackRange = 2f;

    public bool Activated = true; // future stealth system hook

    private Animator animator;
    public EnemyHealth health;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (!Activated) return;
        if (health.getIsDead()) return;

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
        Vector3 direction = (player.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;

        transform.LookAt(player);

        animator.SetBool("isWalking", true);
        animator.SetBool("isAttacking", false);
    }

    void Attack()
    {
        animator.SetBool("isWalking", false);
        animator.SetBool("isAttacking", true);
    }
}