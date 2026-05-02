using UnityEngine;
using System.Collections;

public class SpellCaster : MonoBehaviour
{
    [Header("References")]
    public ElementList elementList;
    public GameObject fireConePrefab;
    public GameObject waterCirclePrefab;
    public GameObject steamConePrefab;
    public GameObject shockwavePrefab;
    public GameObject electricBlastPrefab;
    public GameObject icePrefab;
    public Transform spellOrigin;
    public bool isCasting = false;

    [Header("Base Damage")]
    public float baseDamage = 10f;

    [Header("Damage Multipliers")]
    [Range(0f, 2f)] public float fireDamageMultiplier = 1f;
    [Range(0f, 2f)] public float waterDamageMultiplier = 1f;
    [Range(0f, 2f)] public float earthDamageMultiplier = 1f;
    [Range(0f, 2f)] public float electricDamageMultiplier = 1f;
    [Range(0f, 2f)] public float frostDamageMultiplier = 1f;
    [Range(0f, 2f)] public float poisonDamageMultiplier = 1f;
    [Range(0f, 2f)] public float magnetDamageMultiplier = 1f;
    [Range(0f, 2f)] public float steamDamageMultiplier = 1f;
    [Range(0f, 2f)] public float iceDamageMultiplier = 1f;
    [Range(0f, 2f)] public float plasmaDamageMultiplier = 1f;

    [Header("Spell Effects")]
    [Range(0f, 5f)]  public float electricStunDuration = 1.5f;

    [Range(0f, 1f)]  public float frostSlowPercent = 0.4f;

    [Range(0f, 50f)] public float steamShoveForce = 10f;

    [Range(0f, 1f)]  public float poisonSlowPercent = 0.2f;

    [Range(0f, 50f)] public float magnetPullForce = 20f;

    [Range(1, 20)]   public int   iceProjectileCount = 5;
    
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
            int frost = elementList.CountOf(ElementList.Element.Frost);
            int steam = elementList.CountOf(ElementList.Element.Steam);
            int ice = elementList.CountOf(ElementList.Element.Ice);
            int poison = elementList.CountOf(ElementList.Element.Poison);
            int magnet = elementList.CountOf(ElementList.Element.Magnet);
            int plasma = elementList.CountOf(ElementList.Element.Plasma);

            if (fire > 0) CastFireCone(fire);
            if (water > 0) CastWaterCircle(water);
            if (earth > 0) CastEarthShockwave(earth);
            if (elec > 0) CastElectricBlast(elec);
            if (frost > 0) CastFrost(frost);
            if (steam > 0) CastSteamCone(steam);
            if (ice > 0) CastIce(ice);
            if (poison > 0) CastPoison(poison);
            if (magnet > 0) CastMagnet(magnet);
            if (plasma > 0) CastPlasma(plasma);
            
            elementList.ClearList();
        }
    }

    float GetDamage(float multiplier) => baseDamage * multiplier;

    Vector3 GetMouseWorldPos()
    {
        Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        return groundPlane.Raycast(ray, out float enter) ? ray.GetPoint(enter) : Vector3.zero;
    }

    void CastFireCone(int fireCount)
    {
        isCasting = true;
        Vector3 mouseWorldPos = GetMouseWorldPos();
        Vector3 direction = (mouseWorldPos - spellOrigin.position);
        direction.y = 0;
        direction.Normalize();

        Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up);
        GameObject cone = Instantiate(fireConePrefab, spellOrigin.position, rotation);
        float duration = baseDuration + (fireCount - 1) * durationPerElement;

        StartCoroutine(DamageOverTime(duration, direction, GetDamage(fireDamageMultiplier)));

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

        Debug.Log($"Casting Fire Cone with power: {fireCount}");
        isCasting = false;
    }
    void CastWaterCircle(int waterCount)
    {
        isCasting = true;
        Vector3 mouseWorldPos = GetMouseWorldPos();

        GameObject water = Instantiate(waterCirclePrefab, mouseWorldPos, Quaternion.identity);
        float scale = baseWaterScale + (waterCount - 1) * scalePerElement;
        water.transform.localScale = new Vector3(scale, scale, scale);

        float radius = 3f * scale;
        DealWaterCircleEffect(mouseWorldPos, radius, GetDamage(waterDamageMultiplier), steamShoveForce);

        Destroy(water, 2f);

        Debug.Log($"Casting Water Circle with power: {waterCount}");

        isCasting = false;
    }

    void CastEarthShockwave(int count) 
    {
        isCasting = true;
        Vector3 mouseWorldPos = GetMouseWorldPos();
        Vector3 direction = (mouseWorldPos - spellOrigin.position);
        direction.y = 0;
        direction.Normalize();

        GameObject shockwave = Instantiate(shockwavePrefab, spellOrigin.position, Quaternion.LookRotation(direction));
        
        Destroy(shockwave, 3.5f);
        Debug.Log($"Casting Earth Shockwave with power: {count}");

        isCasting = false;
    }

    void CastElectricBlast(int count)
    {
        isCasting = true;
        Vector3 mouseWorldPos = GetMouseWorldPos();

        GameObject electricBlast = Instantiate(electricBlastPrefab, mouseWorldPos, Quaternion.identity);

        Destroy(electricBlast, 1f);
        Debug.Log($"Casting Electric Blast with power: {count}");
        isCasting = false;
    }

    void CastFrost(int count)
    {
        Debug.Log($"Casting Frost | Power: {count} | Damage: {GetDamage(frostDamageMultiplier)}");
    }

    void CastSteamCone(int count) 
    {
        isCasting = true;
        Vector3 mouseWorldPos = GetMouseWorldPos();
        Vector3 direction = (mouseWorldPos - spellOrigin.position);
        direction.y = 0;
        direction.Normalize();

        Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up);
        GameObject cone = Instantiate(steamConePrefab, spellOrigin.position, rotation);
        float duration = baseDuration + (count - 1) * durationPerElement;

        StartCoroutine(ShoveOverTime(duration, direction));

        ParticleSystem ps = cone.GetComponent<ParticleSystem>();
        if (ps != null)
        {
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            var main = ps.main;
            main.duration = duration;
            main.startLifetime = duration * 0.5f;
            ps.Play();
        }

        StartCoroutine(TrackCursor(cone, duration));
        Destroy(cone, duration + 1f);
        isCasting = false;
    }

    void CastIce(int count)
    {
        isCasting = true;
        Vector3 mouseWorldPos = GetMouseWorldPos();
        Vector3 direction = (mouseWorldPos - spellOrigin.position);
        direction.y = 0;
        direction.Normalize();

        GameObject ice = Instantiate(icePrefab, spellOrigin.position, Quaternion.LookRotation(direction));
        
        Destroy(ice, 3f);
        Debug.Log($"Casting Ice | Power: {count} | Damage: {GetDamage(iceDamageMultiplier)}");
        isCasting = false;
    }

    void CastPoison(int count)
    {
        Debug.Log($"Casting Poison | Power: {count} | Damage: {GetDamage(poisonDamageMultiplier)}");
    }

    void CastMagnet(int count)
    {
        Debug.Log($"Casting Magnet | Power: {count} | Damage: {GetDamage(magnetDamageMultiplier)}");
    }

    void CastPlasma(int count)
    {
        Debug.Log($"Casting Plasma | Power: {count} | Damage: {GetDamage(plasmaDamageMultiplier)}");
    }

    IEnumerator ShoveOverTime(float duration, Vector3 direction)
    {
        float timer = 0f;
        while (timer < duration)
        {
            ShoveCone(spellOrigin.position, direction, 5f, 60f);
            timer += 0.1f;
            yield return new WaitForSeconds(0.1f);
        }
    }

    void ShoveCone(Vector3 origin, Vector3 forward, float range, float angle)
    {
        Collider[] hits = Physics.OverlapSphere(origin, range);

        foreach (Collider hit in hits)
        {
            if (!hit.CompareTag("Enemy")) continue;

            Vector3 dirToTarget = (hit.transform.position - origin).normalized;
            float dot          = Vector3.Dot(forward, dirToTarget);
            float currentAngle = Mathf.Acos(dot) * Mathf.Rad2Deg;

            if (currentAngle <= angle * 0.5f)
            {
                GoblinAI ai = hit.GetComponent<GoblinAI>();
                if (ai != null)
                    ai.ApplyKnockback(forward * steamShoveForce);

                hit.GetComponent<EnemyHealth>()?.TakeDamage(GetDamage(steamDamageMultiplier) * 0.1f);
                Debug.Log("Steam shoved enemy: " + hit.name);
            }
        }
    }

    private IEnumerator TrackCursor(GameObject cone, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration && cone != null)
        {
            Vector3 mouseWorldPos = GetMouseWorldPos();
            Vector3 direction = (mouseWorldPos - spellOrigin.position);
            direction.y = 0;

            if (direction != Vector3.zero)
            {
                cone.transform.rotation = Quaternion.LookRotation(direction.normalized, Vector3.up);
                cone.transform.position = spellOrigin.position;
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

    IEnumerator DamageOverTime(float duration, Vector3 direction, float damage)
    {
        float timer = 0f;
        while (timer < duration)
        {
            DealConeDamage(spellOrigin.position, direction, 5f, 60f, damage);
            timer += 0.1f;
            yield return new WaitForSeconds(0.1f);
        }
    }

    void DealConeDamage(Vector3 origin, Vector3 forward, float range, float angle, float damage)
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
                hit.GetComponent<EnemyHealth>()?.TakeDamage(damage * 0.1f);
            }
        }
    }

    void DealWaterCircleEffect(Vector3 center, float radius, float damage, float pushForce)
{
    Collider[] hits = Physics.OverlapSphere(center, radius);

    foreach (Collider hit in hits)
    {
        if (!hit.CompareTag("Enemy")) continue;

        EnemyHealth enemyHealth = hit.GetComponent<EnemyHealth>();
        if (enemyHealth != null)
        {
            enemyHealth.TakeDamage(damage);
        }

        Vector3 pushDirection = hit.transform.position - center;
        pushDirection.y = 0;

        if (pushDirection != Vector3.zero)
        {
            pushDirection.Normalize();
        }

        GoblinAI goblinAI = hit.GetComponent<GoblinAI>();
        if (goblinAI != null)
        {
            goblinAI.DetectPlayer();
            goblinAI.ApplyKnockback(pushDirection * pushForce);
        }

        Debug.Log("Water circle hit enemy: " + hit.name);
    }
}
}
