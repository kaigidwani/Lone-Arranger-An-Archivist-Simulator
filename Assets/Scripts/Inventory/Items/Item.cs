using System;
using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class Item : VisualElement
{
    // Fields
    //[UxmlAttribute]
    private ItemInfo _itemInfo;

    // Properties
    public Sprite BaseSprite;
    public Vector2 Dimensions;
    public event Action<Vector2, Item> OnStartDrag = delegate { };

    public Item()
    {
        AddToClassList("item");
    }

    void OnPointerDown(PointerDownEvent evt)
    {
        if (evt.button != 0) return;

        OnStartDrag.Invoke(evt.position, this);
        evt.StopPropagation();
    }
}
