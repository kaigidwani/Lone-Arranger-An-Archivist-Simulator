using System;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.Progress;
using Random = UnityEngine.Random;

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

    public void Spawn(Accessioning acc, ItemInfo type)
    {
        _itemInfo = type;

        schedule.Execute(() =>
        {

            float x = Random.Range(acc.Position.x, acc.Bounds.x);
            float y = Random.Range(acc.Position.y, acc.Bounds.y);

            Debug.Log("position: " + new Vector2(x, y));

            // USS properties
            style.left = x;
            style.top = y;
            style.opacity = 100;
        });

        OnStartDrag += InventoryController.Instance.OnPointerDown;

    }

    void OnPointerDown(PointerDownEvent evt)
    {
        if (evt.button != 0) return;

        OnStartDrag.Invoke(evt.position, this);
        evt.StopPropagation();
    }
}
