using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ElementHUD : MonoBehaviour
{
    [Header("References")]
    public ElementList elementList;

    [Header("Circle Settings")]
    public float circleSize = 30f;
    public float padding = 10f;
    public float bottomMargin = 20f;

    private Dictionary<ElementList.Element, Color> elementColors = new()
    {
        //main elements
        { ElementList.Element.Fire, new Color(1f, 0.35f, 0f) },
        { ElementList.Element.Water, new Color(0f, 0.5f, 1f) },
        { ElementList.Element.Earth, new Color(0.39f, 0.19f, 0.01f) },
        { ElementList.Element.Electric, new Color(0.36f, 0.07f, 0.97f) },
        { ElementList.Element.Frost, new Color(0.75f, 0.92f, 1f) },
        //combos
        { ElementList.Element.Steam, new Color(0.8f, 0.8f, 0.8f) }, 
        { ElementList.Element.Poison, new Color(0.4f, 0.9f, 0.1f) },
        { ElementList.Element.Ice, new Color(0.6f, 0.8f, 1f) },
        { ElementList.Element.Magnet, new Color(0.72f, 0.2f, 0.18f) },
        { ElementList.Element.Plasma, new Color(1f, 0f, 0.5f) }
    };

    private List<Image> circles = new List<Image>();

    void Start()
    {
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas != null)
        {
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        }

        elementList.onListChanged.AddListener(RefreshHUD);
    }

    void RefreshHUD()
    {
        foreach (var c in circles)
            Destroy(c.gameObject);
        circles.Clear();

        for (int i = 0; i < elementList.elements.Count; i++)
        {
            var el = elementList.elements[i];

            GameObject obj = new GameObject($"Circle_{i}");
            obj.transform.SetParent(transform, false);

            Image img = obj.AddComponent<Image>();
            img.sprite = MakeCircleSprite();
            img.color = elementColors.ContainsKey(el) ? elementColors[el] : Color.white;

            RectTransform rt = obj.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(circleSize, circleSize);

            rt.anchorMin = new Vector2(0, 0);
            rt.anchorMax = new Vector2(0, 0);
            rt.pivot = new Vector2(0, 0);

            rt.anchoredPosition = new Vector2(
                padding + i * (circleSize + padding),
                bottomMargin
            );

            circles.Add(img);
        }
    }

    Sprite MakeCircleSprite()
    {
        int res = 64;
        Texture2D tex = new Texture2D(res, res);
        float center = res / 2f;
        float radius = res / 2f - 1;

        for (int x = 0; x < res; x++)
        for (int y = 0; y < res; y++)
        {
            float dist = Vector2.Distance(new Vector2(x, y), new Vector2(center, center));
            tex.SetPixel(x, y, dist <= radius ? Color.white : Color.clear);
        }

        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, res, res), new Vector2(0.5f, 0.5f));
    }
}