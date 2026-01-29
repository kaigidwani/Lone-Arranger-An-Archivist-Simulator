using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class Slot : VisualElement
{
    // Fields
    

    // Properties
    public Item ItemRef;

    public bool IsFree { get { return ItemRef == null; } }

    public Slot()
    {

    }

    public void SetItem(Item item)
    {
        ItemRef = item;
        //Add(item);

        item.style.position = Position.Absolute;
        item.style.left = 0;
        item.style.top = 0;
    }

    public void Clear()
    {
        ItemRef = null;
    }


}
