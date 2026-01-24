using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class AgentAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator;
    private NavMeshAgent agent;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        if (!animator)
            animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        if (!animator) return;
        animator.SetFloat("Speed", agent.velocity.magnitude);
    }
}
