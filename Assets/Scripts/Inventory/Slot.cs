using System;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class Slot : VisualElement
{
    // Fields

    private string _color;

    // Properties

    public Item ItemRef;

    public bool IsFree { get { return ItemRef == null; } }

    public Slot()
    {
        RegisterCallback<GeometryChangedEvent>(evt =>
        {
            _color = GetColor();
        });

        
        RegisterCallback<MouseEnterEvent>(OnHover);
        RegisterCallback<MouseLeaveEvent>(OnHoverExit);
    }

    void OnHover(MouseEnterEvent evt)
    {
        AddToClassList("hover");
    }

    void OnHoverExit(MouseLeaveEvent evt)
    {
        RemoveFromClassList("hover");
    }

    /// <summary>
    /// Adds a reference to an item to this inventory slot
    /// </summary>
    /// <param name="item">An item that should respond to this slot</param>
    public void SetItem(Item item)
    {
        ItemRef = item;
    }

    /// <summary>
    /// Finds the color of this slot via its classlist
    /// </summary>
    /// <returns>A string containing the color of this slot</returns>
    public string GetColor()
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
    public void ClearItem()
    {
        ItemRef = null;
    }
}
