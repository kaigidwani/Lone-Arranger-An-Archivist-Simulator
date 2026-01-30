using System;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class Slot : VisualElement
{
    // Properties

    public Item ItemRef;

    public string Color;

    public bool IsFree { get { return ItemRef == null; } }

    public Slot()
    {
        
    }

    /// <summary>
    /// Adds a reference to an item to this inventory slot
    /// </summary>
    /// <param name="item">An item that should respond to this slot</param>
    public void SetItem(Item item)
    {
        ItemRef = item;

        item.style.position = Position.Absolute;

        Color = GetSlotColor();
        if (!string.IsNullOrEmpty(Color))
        {
            item.AddToClassList($"item-{Color}");
        }
    }

    /// <summary>
    /// Finds the color of this slot via its classlist
    /// </summary>
    /// <returns>A string containing the color of this slot</returns>
    private string GetSlotColor()
    {
        foreach (string className in GetClasses())
        {
            if (className.StartsWith("inventory-slot-"))
            {
                return className.Replace("inventory-slot-", "");
            }
        }

        return "";

    }

    /// <summary>
    /// Removes this slot's item reference
    /// </summary>
    private void Clear()
    {
        ItemRef = null;
    }
}
