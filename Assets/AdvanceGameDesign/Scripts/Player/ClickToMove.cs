using UnityEngine;
using UnityEngine.AI;

public class ClickToMove : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private NavMeshAgent agent;

    [Header("Click Settings")]
    [SerializeField] private LayerMask groundMask = ~0; // everything by default

    private void Reset()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Awake()
    {
        if (!mainCamera) mainCamera = Camera.main;
        if (!agent) agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (!mainCamera || !agent) return;

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, 500f, groundMask, QueryTriggerInteraction.Ignore))
            {
                agent.SetDestination(hit.point);
            }
        }
    }
}
