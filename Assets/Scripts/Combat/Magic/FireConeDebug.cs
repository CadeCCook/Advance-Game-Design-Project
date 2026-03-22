using UnityEngine;

public class FireConeDebug : MonoBehaviour
{
    public Transform spellOrigin;
    public float range = 5f;
    public float angle = 60f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) // press Space to test
        {
            DealConeDamage();
        }
    }

    public void DealConeDamage()
    {
        // Draw the detection sphere
        Debug.DrawRay(spellOrigin.position, spellOrigin.forward * range, Color.red, 1f);

        Collider[] hits = Physics.OverlapSphere(spellOrigin.position, range);

        foreach (Collider hit in hits)
        {
            if (!hit.CompareTag("Enemy")) continue;

            Vector3 dirToTarget = (hit.transform.position - spellOrigin.position);
            dirToTarget.y = 0; // ignore vertical for cone angle
            dirToTarget.Normalize();

            float dot = Vector3.Dot(spellOrigin.forward, dirToTarget);
            float currentAngle = Mathf.Acos(dot) * Mathf.Rad2Deg;

            // Draw lines to visualize which objects are considered
            Debug.DrawLine(spellOrigin.position, hit.transform.position, Color.yellow, 1f);

            if (currentAngle <= angle * 0.5f)
            {
                Debug.Log("Hit enemy: " + hit.name + " at angle " + currentAngle);
                hit.GetComponent<EnemyHealth>()?.TakeDamage(10);
            }
        }
    }

    void OnDrawGizmos()
    {
        if (spellOrigin == null) return;

        Gizmos.color = Color.cyan;
        Vector3 forward = spellOrigin.forward;
        Vector3 rightEdge = Quaternion.Euler(0, angle * 0.5f, 0) * forward;
        Vector3 leftEdge = Quaternion.Euler(0, -angle * 0.5f, 0) * forward;

        Gizmos.DrawLine(spellOrigin.position, spellOrigin.position + forward * range);
        Gizmos.DrawLine(spellOrigin.position, spellOrigin.position + rightEdge * range);
        Gizmos.DrawLine(spellOrigin.position, spellOrigin.position + leftEdge * range);
    }
}