using UnityEngine;

public class IcicleProjectile : MonoBehaviour
{
    public float speed = 15f;

    private Vector3 direction;
    private float damage;
    private bool initialized = false;

    public void Init(Vector3 dir, float dmg)
    {
        direction = dir;
        damage = dmg;
        initialized = true;

        // Destroy if it never hits anything
        Destroy(gameObject, 2f);
    }

    void Update()
    {
        if (!initialized) return;
        transform.position += direction * speed * Time.deltaTime;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Enemy")) return;

        other.GetComponent<EnemyHealth>()?.TakeDamage(damage);
        other.GetComponent<SkeletonMinion>()?.TakeDamage(damage);
        other.GetComponent<BossHealth>()?.TakeDamage(damage);
        Debug.Log($"Icicle hit: {other.name}");
        Destroy(gameObject);
    }
}