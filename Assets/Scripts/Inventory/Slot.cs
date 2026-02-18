using System;
using System.Drawing;
using Unity.VisualScripting;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.Progress;

[UxmlElement]
public partial class Slot : VisualElement
{
    // Fields

    private string _color;
    private Label _debugLabel;

    // Properties

    public Item ItemRef;

    public bool IsFree { get { return ItemRef == null; } }

    public Slot()
    {
        RegisterCallback<GeometryChangedEvent>(evt =>
        {
            _color = GetColor();
        });

        _debugLabel = new Label();
        _debugLabel.text = "Empty";
        _debugLabel.AddToClassList("debug-text");

        Add(_debugLabel);
    }

    /// <summary>
    /// Adds a reference to an item to this inventory slot
    /// </summary>
    /// <param name="item">An item that should respond to this slot</param>
    public void SetItem(Item item)
    {
        Debug.Log(item.Name);
        ItemRef = item;
        _debugLabel.text = item.Name;
        
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
        _debugLabel.text = "Empty";
    }
}
