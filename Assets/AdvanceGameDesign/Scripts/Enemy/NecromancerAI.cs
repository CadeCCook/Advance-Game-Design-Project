using System.Collections;
using UnityEngine;

public class NecromancerAI : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public GameObject warningCirclePrefab;
    public GameObject lightningPrefab;

    [Header("Movement")]
    public float moveSpeed = 2f;
    public float stopDistance = 8f;

    private Animator animator;
    private bool playerDetected = false;

    [Header("Skeleton Summon")]
    public SkeletonMinion[] skeletons;
    public float reviveDelay = 6f;

    private bool skeletonsActivated = false;
    private bool[] skeletonReviving;

    [Header("Lightning Attack")]
    public float attackRange = 15f;
    public float attackCooldown = 4f;
    public float warningTime = 1f;
    public float attackRadius = 1.5f;
    public float damage = 20f;

    private bool isAttacking;

    void Start()
    {
        animator = GetComponent<Animator>();
        skeletonReviving = new bool[skeletons.Length];
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

            if (playerObj != null)
                player = playerObj.transform;
        }

        StartCoroutine(AttackLoop());
    }

    IEnumerator AttackLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(attackCooldown);

            if (player == null || isAttacking)
            continue;

            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (distanceToPlayer <= attackRange)
                {
                    playerDetected = true;
                    StartCoroutine(CastLightningAttack());
                }
        }
    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        Vector3 lookDirection = player.position - transform.position;
        lookDirection.y = 0;

        if (lookDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
                transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                5f * Time.deltaTime
                );
            }

        if (playerDetected && distanceToPlayer > stopDistance)
            {
                MoveTowardPlayer();
            }
        else
            {
                if (animator != null)
                animator.SetBool("isWalking", false);
            }

        if (!skeletonsActivated && distanceToPlayer <= attackRange)
            {
                skeletonsActivated = true;
                ReviveAllSkeletons();
            }

        if (skeletonsActivated)
            {
                for (int i = 0; i < skeletons.Length; i++)
                    {
                        if (skeletons[i] != null && !skeletons[i].isAlive && !skeletonReviving[i])
                            {
                                StartCoroutine(ReviveSkeletonAfterDelay(skeletons[i], i));
                            }
                    }
            }
    }

    void MoveTowardPlayer()
    {
        Vector3 direction = player.position - transform.position;
        direction.y = 0;
        direction.Normalize();

        transform.position += direction * moveSpeed * Time.deltaTime;

        if (animator != null)
        animator.SetBool("isWalking", true);
    }

    void ReviveAllSkeletons()
    {
        for (int i = 0; i < skeletons.Length; i++)
            {
                SkeletonMinion skeleton = skeletons[i];

                if (skeleton != null && !skeleton.isAlive)
                    {
                        skeletonReviving[i] = true;

                        skeleton.Revive();

                        StartCoroutine(ResetReviveFlag(i));
                    }
            }
    }

    IEnumerator ResetReviveFlag(int index)
    {
        yield return new WaitForSeconds(6f);
        skeletonReviving[index] = false;
    }

    IEnumerator ReviveSkeletonAfterDelay(SkeletonMinion skeleton, int index)
    {
        skeletonReviving[index] = true;

        yield return new WaitForSeconds(reviveDelay);

        if (skeleton != null && !skeleton.isAlive)
            {
                skeleton.Revive();
            }

        skeletonReviving[index] = false;
    }

    IEnumerator CastLightningAttack()
    {
        isAttacking = true;

        Vector3 targetPosition = player.position;
        targetPosition.y = player.position.y + 0.001f;

        GameObject warning = Instantiate(warningCirclePrefab, targetPosition, Quaternion.identity);
        warning.transform.localScale = new Vector3(attackRadius * 2f, 0.01f, attackRadius * 2f);

        yield return new WaitForSeconds(warningTime);

        if (warning != null)
            Destroy(warning);

        GameObject lightning = Instantiate(lightningPrefab, targetPosition, Quaternion.identity);
        Destroy(lightning, 2f);

        Collider[] hits = Physics.OverlapSphere(targetPosition, attackRadius);

        foreach (Collider hit in hits)
        {
            if (!hit.CompareTag("Player")) continue;

            PlayerHealth playerHealth = hit.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
        }

        isAttacking = false;
    }
}
