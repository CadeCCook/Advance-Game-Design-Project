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
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            int elementCount = elementList.elements.Count;
            int fireCount = elementList.CountOf(ElementList.Element.Fire);
            int waterCount = elementList.CountOf(ElementList.Element.Water);


            if (elementCount == 0)
            {
                Debug.Log("No elements in list — nothing to cast.");
                return;
            }
            if (fireCount > 0)
            {
                CastFireCone(fireCount);
                elementList.ClearList();
            }
            if (waterCount > 0) 
            {
                CastWaterCircle(waterCount);
                elementList.ClearList();
            }
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
}
