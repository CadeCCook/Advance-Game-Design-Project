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
    public GameObject poisonCloudPrefab;
    public GameObject magnetPulsePrefab;
    public GameObject plasmaBurstPrefab;
    public GameObject frostPrefab;
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

    [Range(1, 20)]   public int   iceProjectileCountMult = 1;

    [Range(0, 20)]   public int   iceProjectileCountAdd = 0;

    [Range(0f, 10f)] public float poisonDuration = 5f;
    
    [Header("Fire Cone Scaling")]
    public float baseDuration = 2f;
    public float durationPerElement = 1f;
    

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

    void ScaleParticleSystems(GameObject obj, float scale)
    {
        foreach (ParticleSystem ps in obj.GetComponentsInChildren<ParticleSystem>())
        {
            var main = ps.main;
            main.startSizeMultiplier *= scale;
        }
    }

    void CastFireCone(int count)
    {
        isCasting = true;
        Vector3 mouseWorldPos = GetMouseWorldPos();
        Vector3 direction = (mouseWorldPos - spellOrigin.position);
        direction.y = 0;
        direction.Normalize();

        Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up);
        GameObject cone = Instantiate(fireConePrefab, spellOrigin.position, rotation);
        float duration = baseDuration + (count - 1) * durationPerElement;

        StartCoroutine(DamageOverTime(duration, cone.transform, GetDamage(fireDamageMultiplier * count/6)));

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

        Debug.Log($"Casting Fire Cone | Power: {count} | Damage: {GetDamage(fireDamageMultiplier * count)}");
        isCasting = false;
    }
    void CastWaterCircle(int count)
    {
        isCasting = true;
        Vector3 mouseWorldPos = GetMouseWorldPos();

        GameObject water = Instantiate(waterCirclePrefab, mouseWorldPos, Quaternion.identity);
        float  scale = ApplySpellScale(water, count);

        float radius = 3f * scale;
        DealAreaEffect(mouseWorldPos, radius, GetDamage(waterDamageMultiplier * count), steamShoveForce);

        Destroy(water, 2f);

        Debug.Log($"Casting Water Circle | Power: {count} | Damage: {GetDamage(waterDamageMultiplier * count)}");

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
        DealAreaEffect(mouseWorldPos, 5f, GetDamage(earthDamageMultiplier * count), 1f * count);
        
        
        Destroy(shockwave, 3.5f);
        Debug.Log($"Casting Earth Shockwave | Power: {count} | Damage: {GetDamage(earthDamageMultiplier * count)}");

        isCasting = false;
    }

    void CastElectricBlast(int count)
    {
        isCasting = true;
        Vector3 mouseWorldPos = GetMouseWorldPos();

        GameObject electricBlast = Instantiate(electricBlastPrefab, mouseWorldPos, Quaternion.identity);
        DealAreaEffect(mouseWorldPos, 2f, GetDamage(electricDamageMultiplier * count), 0f);
        StartCoroutine(DealStun(mouseWorldPos, 2f, electricStunDuration * count));
        
        Destroy(electricBlast, 1f);
        Debug.Log($"Casting Electric Blast | Power: {count} | Damage: {GetDamage(electricDamageMultiplier * count)} | Stun: {electricStunDuration * count} seconds");
        isCasting = false;
    }

    void CastFrost(int count)
    {
        isCasting = true;
        Vector3 mouseWorldPos = GetMouseWorldPos();

        GameObject frost = Instantiate(frostPrefab, mouseWorldPos, Quaternion.identity);
        DealAreaEffect(mouseWorldPos, 3f, GetDamage(frostDamageMultiplier * count), 0f);
        StartCoroutine(DealSlow(mouseWorldPos, 3f, frostSlowPercent, 5f));
        
        Destroy(frost, 4f);
        Debug.Log($"Casting Frost | Power: {count} | Damage: {GetDamage(frostDamageMultiplier)}");
        isCasting = false;
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
        ScaleParticleSystems(cone, 1f);

        StartCoroutine(ShoveOverTime(duration, cone.transform, GetDamage(steamDamageMultiplier * count * 2)));

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
        int iceProjectileCount = count * iceProjectileCountMult + iceProjectileCountAdd;

        for (int i = 0; i < iceProjectileCount; i++)
        {
            // Spread each projectile evenly across a 30 degree arc
            float spreadAngle = iceProjectileCount > 1 
                ? Mathf.Lerp(-15f, 15f, (float)i / (iceProjectileCount - 1)) 
                : 0f;
            Vector3 spreadDirection = Quaternion.Euler(0, spreadAngle, 0) * direction;

            Quaternion spawnRotation = Quaternion.LookRotation(spreadDirection) * Quaternion.Euler(0, -90f, 0);
            GameObject icicle = Instantiate(icePrefab, spellOrigin.position, spawnRotation);
            icicle.GetComponent<IcicleProjectile>()?.Init(spreadDirection, GetDamage(iceDamageMultiplier));
        }

        Debug.Log($"Casting Ice | Power: {count} | Projectiles: {iceProjectileCount} | Damage: {GetDamage(iceDamageMultiplier)}");
        isCasting = false;
    }

    void CastPoison(int count)
    {
        isCasting = true;
        Vector3 mouseWorldPos = GetMouseWorldPos();

        GameObject pool = Instantiate(poisonCloudPrefab, mouseWorldPos, Quaternion.identity); // swap for correct prefab later
        float scale = ApplySpellScale(pool, count);
        ScaleParticleSystems(poisonCloudPrefab, scale);

        float radius = 3f * scale;
        StartCoroutine(PoisonPoolOverTime(mouseWorldPos, radius, GetDamage(poisonDamageMultiplier * count), poisonDuration));
        StartCoroutine(DealSlow(mouseWorldPos, radius, poisonSlowPercent, poisonDuration));

        Destroy(pool, poisonDuration);
        isCasting = false;

        Debug.Log($"Casting Poison | Power: {count} | Damage: {GetDamage(poisonDamageMultiplier)}");
    }

    void CastMagnet(int count)
    {
        isCasting = true;
        Vector3 mouseWorldPos = GetMouseWorldPos();

        GameObject magnet = Instantiate(magnetPulsePrefab, mouseWorldPos, Quaternion.identity);
        float scale = ApplySpellScale(magnet, count);
        

        float radius = 1f * scale;
        DealAreaEffect(mouseWorldPos, radius, GetDamage(magnetDamageMultiplier * count), 0f);
        StartCoroutine(MagnetPullOverTime(mouseWorldPos, radius, 2f));

        Destroy(magnet, 2f);
        isCasting = false;

        Debug.Log($"Casting Magnet | Power: {count} | Damage: {GetDamage(magnetDamageMultiplier)}");
    }

    void CastPlasma(int count)
    {
        isCasting = true;
        Vector3 mouseWorldPos = GetMouseWorldPos();

        GameObject plasma = Instantiate(plasmaBurstPrefab, mouseWorldPos, Quaternion.identity); // swap for correct prefab later
        float scale = ApplySpellScale(plasma, count);

        float radius = 3f * scale;
        DealAreaEffect(mouseWorldPos, radius, GetDamage(plasmaDamageMultiplier * count), 0f);

        Destroy(plasma, 2f);
        isCasting = false;

        Debug.Log($"Casting Plasma | Power: {count} | Damage: {GetDamage(plasmaDamageMultiplier)}");
    }

    IEnumerator ShoveOverTime(float duration, Transform coneTransform, float damage)
    {
        float timer = 0f;
        while (timer < duration)
        {
            ShoveCone(spellOrigin.position, coneTransform.forward, 5f, 60f, damage);
            timer += 0.1f;
            yield return new WaitForSeconds(0.1f);
        }
    }

    void ShoveCone(Vector3 origin, Vector3 forward, float range, float angle, float damage)
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

                EnemyHealth enemyHealth = hit.GetComponent<EnemyHealth>();
                if (enemyHealth != null)
                    {
                        enemyHealth.TakeDamage(damage * 0.1f);
                    }

                SkeletonMinion skeletonMinion = hit.GetComponent<SkeletonMinion>();
                if (skeletonMinion != null)
                    {
                        skeletonMinion.TakeDamage(damage * 0.1f);
                        skeletonMinion.ApplyKnockback(forward * steamShoveForce);
                    }

                BossHealth bossHealth = hit.GetComponent<BossHealth>();
                if (bossHealth != null)
                    {
                        bossHealth.TakeDamage(damage * 0.1f);
                    }
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

    IEnumerator DamageOverTime(float duration, Transform coneTransform, float damage)
    {
        float timer = 0f;
        while (timer < duration)
        {
            DealConeDamage(spellOrigin.position, coneTransform.forward, 5f, 60f, damage);
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
                EnemyHealth enemyHealth = hit.GetComponent<EnemyHealth>();
                    if (enemyHealth != null)
                        {
                            enemyHealth.TakeDamage(damage);
                        }

                SkeletonMinion skeletonMinion = hit.GetComponentInParent<SkeletonMinion>();
                    if (skeletonMinion != null)
                    {
                        skeletonMinion.TakeDamage(damage);
                    }

                BossHealth bossHealth = hit.GetComponent<BossHealth>();
                    if (bossHealth != null)
                        {
                            bossHealth.TakeDamage(damage);
                        }
            }
        }
    }

    void DealAreaEffect(Vector3 center, float radius, float damage, float pushForce)
    {
        Collider[] hits = Physics.OverlapSphere(center, radius);

        foreach (Collider hit in hits)
        {
            if (!hit.CompareTag("Enemy")) continue;

            EnemyHealth enemyHealth = hit.GetComponent<EnemyHealth>();
                if (enemyHealth != null)
                    {
                        Debug.Log("***Goblin hit***");
                        enemyHealth.TakeDamage(damage);
                    }

            SkeletonMinion skeletonMinion = hit.GetComponentInParent<SkeletonMinion>();
                if (skeletonMinion != null)
                    {
                        Debug.Log("***Skeleton hit***");
                        skeletonMinion.TakeDamage(damage);
                    }

            BossHealth bossHealth = hit.GetComponent<BossHealth>();
                if (bossHealth != null)
                    {
                        bossHealth.TakeDamage(damage);
                    }

            if (pushForce > 0f)
            {
                Vector3 pushDirection = (hit.transform.position - center);
                pushDirection.y = 0;
                if (pushDirection != Vector3.zero) pushDirection.Normalize();

                GoblinAI ai = hit.GetComponent<GoblinAI>();
                if (ai != null)
                {
                    ai.DetectPlayer();
                    ai.ApplyKnockback(pushDirection * pushForce);
                }
            }

            Debug.Log($"Area effect hit enemy: {hit.name}");
        }
    }

    IEnumerator PoisonPoolOverTime(Vector3 center, float radius, float damage, float duration)
    {
        float timer = 0f;
        while (timer < duration)
        {
            Collider[] hits = Physics.OverlapSphere(center, radius);
            foreach (Collider hit in hits)
            {
                if (!hit.CompareTag("Enemy")) continue;
                EnemyHealth enemyHealth = hit.GetComponent<EnemyHealth>();
                if (enemyHealth != null)
                    {
                        enemyHealth.TakeDamage(damage * 0.1f);
                    }

                SkeletonMinion skeletonMinion = hit.GetComponentInParent<SkeletonMinion>();
                if (skeletonMinion != null)
                {
                    skeletonMinion.TakeDamage(damage * 0.1f);
                }

                BossHealth bossHealth = hit.GetComponent<BossHealth>();
                if (bossHealth != null)
                    {
                        bossHealth.TakeDamage(damage * 0.1f);
                    }
                Debug.Log($"Poison ticking on enemy: {hit.name}");
            }
            timer += 0.5f;
            yield return new WaitForSeconds(0.5f);
        }
    }

    IEnumerator MagnetPullOverTime(Vector3 center, float radius, float duration)
    {
        float timer = 0f;
        while (timer < duration)
        {
            Collider[] hits = Physics.OverlapSphere(center, radius);
            foreach (Collider hit in hits)
            {
                if (!hit.CompareTag("Enemy")) continue;

                GoblinAI ai = hit.GetComponent<GoblinAI>();
                if (ai != null)
                {
                    Vector3 pullDirection = (center - hit.transform.position);
                    pullDirection.y = 0;
                    if (pullDirection != Vector3.zero) pullDirection.Normalize();
                    ai.DetectPlayer();
                    ai.ApplyKnockback(pullDirection * magnetPullForce);
                }

                SkeletonMinion skeleton = hit.GetComponent<SkeletonMinion>();
                if (skeleton != null)
                {
                    Vector3 pullDirection = (center - hit.transform.position);
                    pullDirection.y = 0;
                    if (pullDirection != Vector3.zero) pullDirection.Normalize();
                    skeleton.ApplyKnockback(pullDirection * magnetPullForce);
                }
                
            }
            timer += 0.1f;
            yield return new WaitForSeconds(0.1f);
        }
    }

    IEnumerator DealStun(Vector3 center, float radius, float duration)
    {
        float maxDuration = 3f;
        float timer = 0f;
        
        if (duration > maxDuration)
        {
            duration = maxDuration;
        }
        while (timer < duration)
        {
            Collider[] hits = Physics.OverlapSphere(center, radius);
            foreach (Collider hit in hits)
            {
                if (!hit.CompareTag("Enemy")) continue;
                GoblinAI ai = hit.GetComponent<GoblinAI>();
                if (ai != null)
                {
                    float remainingTime = duration - timer;
                    ai.ApplyStun(remainingTime);  // Only apply the time left
                }

                SkeletonMinion skeleton = hit.GetComponent<SkeletonMinion>();
                if (skeleton != null)
                {
                    float remainingTime = duration - timer;
                    skeleton.ApplyStun(remainingTime);
                }
                Debug.Log($"Stunned enemy: {hit.name}");
            }
            timer += 0.5f;
            yield return new WaitForSeconds(0.5f);
        }
    }

    IEnumerator DealSlow(Vector3 center, float radius, float slowPercent, float duration)
    {
        Collider[] hits = Physics.OverlapSphere(center, radius);
        foreach (Collider hit in hits)
        {
            if (!hit.CompareTag("Enemy")) continue;
            hit.GetComponent<GoblinAI>()?.ApplySlow(slowPercent, duration);
            hit.GetComponent<SkeletonMinion>()?.ApplySlow(slowPercent, duration);
        }
        yield return null;
    }

    float ApplySpellScale(GameObject spell, int count)
    {
        float scale = 1f + (count - 1) * 0.5f;
        spell.transform.localScale = new Vector3(scale, scale, scale);
        return scale;
    }

}
