using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ElementList : MonoBehaviour
{
    public SpellCaster spellCaster;
    public enum Element { Fire }

    public List<Element> elements = new List<Element>();
    public UnityEvent onListChanged;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (spellCaster.isCasting)
            {
                Debug.Log("Can't add elements while casting!");
                return;
            }

            if (elements.Count >= 6)
            {
                Debug.Log("Element list is full! (max 6)");
                return;
            }

            elements.Add(Element.Fire);
            onListChanged.Invoke();
            Debug.Log($"Added Fire. Total elements: {elements.Count}");
        }
    }

    public void ClearList()
    {
        elements.Clear();
        onListChanged.Invoke();
    }

    public int CountOf(Element e)
    {
        int count = 0;
        foreach (var el in elements)
            if (el == e) count++;
        return count;
    }
}