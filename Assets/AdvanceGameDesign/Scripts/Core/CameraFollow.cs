using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;

    [Header("Follow")]
    [SerializeField] private Vector3 offset = new Vector3(0f, 8f, -10f);
    [SerializeField] private float smooth = 10f;

    [Header("Optional")]
    [SerializeField] private bool lookAtTarget = true;

    private void LateUpdate()
    {
        if (!target) return;

        Vector3 desired = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, desired, smooth * Time.deltaTime);

        if (lookAtTarget)
            transform.LookAt(target.position);
    }

    // Handy: right-click component -> Recenter Offset
    [ContextMenu("Recenter Offset From Current Position")]
    private void RecenterOffset()
    {
        if (!target) return;
        offset = transform.position - target.position;
    }
}