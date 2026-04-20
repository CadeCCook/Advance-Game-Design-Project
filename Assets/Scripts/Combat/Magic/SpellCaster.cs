using UnityEngine;
using System.Collections;

public class SpellCaster : MonoBehaviour
{
    [Header("References")]
    public ElementList elementList;
    public GameObject fireConePrefab;
    public GameObject waterCirclePrefab;
    public Transform spellOrigin;
    public bool isCasting = false;

    [Header("Fire Cone Scaling")]
    public float baseDuration = 2f;
    public float durationPerElement = 1f;


    [Header("Water Circle Scaling")]
    public float baseWaterScale = 1f;
    public float scalePerElement = 0.5f;
    

    private Camera mainCam;

    void Start()
    {
        mainCam = Camera.main;
        Debug.Log("waterCirclePrefab assigned: " + (waterCirclePrefab != null));
    Debug.Log("fireConePrefab assigned: " + (fireConePrefab != null));
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (elementList.elements.Count == 0)
            {
                Debug.Log("No elements in list — nothing to cast.");
                return;
            }

            int fire = elementList.CountOf(ElementList.Element.Fire);
            int water = elementList.CountOf(ElementList.Element.Water);
            int earth = elementList.CountOf(ElementList.Element.Earth);
            int elec = elementList.CountOf(ElementList.Element.Electric);
            int steam = elementList.CountOf(ElementList.Element.Steam);
            int lava = elementList.CountOf(ElementList.Element.lava);
            int ice = elementList.CountOf(ElementList.Element.ice);
            int plasma = elementList.CountOf(ElementList.Element.plasma);

            if (fire > 0) CastFireCone(fire);
            if (water > 0) CastWaterCircle(water);
            if (earth > 0) CastEarthShockwave(earth);
            if (elec > 0) CastElectricBlast(elec);
            if (steam > 0) CastSteamBlast(steam);
            if (lava > 0) CastLava(lava);
            if (ice > 0) CastIce(ice);
            if (plasma > 0) CastPlasma(plasma);
            
            elementList.ClearList();
        }
    }

    void CastFireCone(int fireCount)
    {
        isCasting = true;
        Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

        if (!groundPlane.Raycast(ray, out float enter)) return;

        Vector3 mouseWorldPos = ray.GetPoint(enter);
        Vector3 direction = (mouseWorldPos - spellOrigin.position);
        direction.y = 0;
        direction.Normalize();

        Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up);

        GameObject cone = Instantiate(fireConePrefab, spellOrigin.position, rotation);

        float duration = baseDuration + (fireCount - 1) * durationPerElement;

        StartCoroutine(DamageOverTime(duration, direction));    // Start the function to damage enemies

        ParticleSystem ps = cone.GetComponent<ParticleSystem>();
        if (ps != null)
        {
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            var main = ps.main;
            main.duration = duration;
            main.startLifetime = duration * 0.8f;
            ps.Play();
        }

        StartCoroutine(TrackCursor(cone, duration));

        Destroy(cone, duration + 1f);
        isCasting = false;
    }
    void CastWaterCircle(int waterCount)
    {
        isCasting = true;

        Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

        if (!groundPlane.Raycast(ray, out float enter)) return;

        Vector3 mouseWorldPos = ray.GetPoint(enter);

        // Spawn at mouse position (circle AoE instead of cone)
        GameObject water = Instantiate(waterCirclePrefab, mouseWorldPos, Quaternion.identity);

        // Scale instead of duration
        float scale = baseWaterScale + (waterCount - 1) * scalePerElement;
        water.transform.localScale = new Vector3(scale, scale, scale);

        // Destroy after short time
        Destroy(water, 2f);

        isCasting = false;
    }

    void CastElectricBlast(int count)
    {
        Debug.Log($"Casting Electric Blast with power: {count}");
    }

    void CastEarthShockwave(int count) 
    {
        Debug.Log($"Casting Earth Shockwave with power: {count}");
    }

    void CastSteamBlast(int count) 
    {
    Debug.Log($"Casting Steam Blast with power: {count}");
    }

    void CastLava(int count) { /* Logic */ }
    void CastIce(int count) { /* Logic */ }
    void CastPlasma(int count) { /* Logic */ }

    private IEnumerator TrackCursor(GameObject cone, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration && cone != null)
        {
            Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
            Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

            if (groundPlane.Raycast(ray, out float enter))
            {
                Vector3 mouseWorldPos = ray.GetPoint(enter);
                Vector3 direction = (mouseWorldPos - spellOrigin.position);
                direction.y = 0;

                if (direction != Vector3.zero)
                {
                    cone.transform.rotation = Quaternion.LookRotation(direction.normalized, Vector3.up);
                    cone.transform.position = spellOrigin.position;
                }
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        if (cone != null)
        {
            ParticleSystem ps = cone.GetComponent<ParticleSystem>();
            if (ps != null)
                ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }

        isCasting = false;
    }

    IEnumerator DamageOverTime(float duration, Vector3 direction) // Damage enemies in cone of fire
    { 
        float timer = 0f; 
        while (timer < duration) 
        { 
            DealConeDamage(spellOrigin.position, direction, 5f, 60f); 
            timer += 0.1f; 
            yield return new WaitForSeconds(0.1f); 
        } 
    }
    void DealConeDamage(Vector3 origin, Vector3 forward, float range, float angle)
    {
        Collider[] hits = Physics.OverlapSphere(origin, range);

        foreach (Collider hit in hits)
        {
            if (!hit.CompareTag("Enemy")) continue;

            Vector3 dirToTarget = (hit.transform.position - origin).normalized;

            float dot = Vector3.Dot(forward, dirToTarget);
            float currentAngle = Mathf.Acos(dot) * Mathf.Rad2Deg;

            if (currentAngle <= angle * 0.5f)
            {
                Debug.Log("Hit enemy: " + hit.name + " at angle " + currentAngle);

                hit.GetComponent<EnemyHealth>()?.TakeDamage(10*.1f);
            }
        }
    }
}
