using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;



public class ElementList : MonoBehaviour
{
    public SpellCaster spellCaster;
    public enum Element { Fire, Water }

    public List<Element> elements = new List<Element>();
    public UnityEvent onListChanged;

    // Dictionary to map keys to elements for easy removal and addition
    public Dictionary<KeyCode, Element> elementKeys = new Dictionary<KeyCode, Element>
    {
        {KeyCode.Q, Element.Fire},
        {KeyCode.W, Element.Water}
        // Follow format to add more elements
    };

    void Update()
    {
        foreach (var pair in elementKeys)
        {
            if (Input.GetKeyDown(pair.Key))
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

            elements.Add(pair.Value);
            Debug.Log($"Added {pair.Value}. Total elements: {elements.Count}");
            onListChanged.Invoke();
            return;
            }
        }
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