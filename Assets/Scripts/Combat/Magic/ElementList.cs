using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;



public class ElementList : MonoBehaviour
{
    public SpellCaster spellCaster;
    public enum Element { Fire, Water, Earth, Electric, Steam, lava, ice, plasma }

    public List<Element> elements = new List<Element>();
    public UnityEvent onListChanged;

    public Dictionary<KeyCode, Element> elementKeys = new Dictionary<KeyCode, Element>
    {
        {KeyCode.Q, Element.Fire},
        {KeyCode.W, Element.Water},
        {KeyCode.E, Element.Earth},
        {KeyCode.R, Element.Electric}
        // Follow format to add more elements
    };

    private Dictionary<(Element, Element), Element> recipes = new()
    {
        { (Element.Fire, Element.Water), Element.Steam },
        { (Element.Fire, Element.Earth), Element.lava },
        { (Element.Water, Element.Earth), Element.ice },
        { (Element.Electric, Element.Fire), Element.plasma }
    };

    void Update()
    {
        foreach (var pair in elementKeys)
        {
            if (Input.GetKeyDown(pair.Key))
            {
                if (spellCaster.isCasting) return;

                ProcessElementAddition(pair.Value);
            }
        }
    }

    void ProcessElementAddition(Element newElement)
    {
        bool combined = false;

        for (int i = elements.Count - 1; i > 0; i--)
        {
            Element existing = elements[i];
            
            if (recipes.TryGetValue((existing, newElement), out Element result) ||
                recipes.TryGetValue((newElement, existing), out result))
            {
                elements.RemoveAt(i);
                elements.Add(result);
                combined = true;
                break; 
            }
        }

        if (!combined && elements.Count < 6)
        {
            elements.Add(newElement);
        }

        onListChanged.Invoke();
    }


    public void ClearList()
    {
        elements.Clear();
        onListChanged.Invoke();
    }
    // This Count is used to check the total number of elements in the list, used to check if the list is full or empty
    public int CountOf(Element e)
    {
        int count = 0;
        foreach (var pair in elements)
        {
            if (pair == e)
            {
                count++;
            }
        }
        return count;
    }
}