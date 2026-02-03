using System;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.Progress;
using Random = UnityEngine.Random;

[UxmlElement]
public partial class Item : VisualElement
{
    // Fields

    private ItemInfo _itemInfo;
    private Slot _currentSlot;

    // Properties

    public Sprite BaseSprite;
    public Vector2 Dimensions;
    public Vector2 ItemSize;
    public event Action<Vector2, Item> OnStartDrag = delegate { };

    /// <summary>
    /// 
    /// </summary>
    public Slot CurrentSlot { 
        get { return _currentSlot; } 
        set
        {
            _currentSlot = value;
            _currentSlot.Add(this);
            _currentSlot.SetItem(this);
        }
    }

    public Item()
    {
        _currentSlot = null;
        RegisterCallback<PointerDownEvent>(OnPointerDown);
    }

    /// <summary>
    /// Generates a random item inside of the accessioning box
    /// </summary>
    /// <param name="box">The element where this item will spawn</param>
    public void Spawn(Accessioning box)
    {
        AddToClassList("item");

        // Generate random item from current item pool
        ItemInfo type = null;
        if (InventoryController.Instance.ItemPool.Length > 0)
        {
            type = InventoryController.Instance.ItemPool[Random.Range(0, InventoryController.Instance.ItemPool.Length - 1)];

            _itemInfo = type;
            style.backgroundImage = type.Sprite.texture;

            BaseSprite = _itemInfo.Sprite;
            //Dimensions = _itemInfo.Dimensions;
        }

        schedule.Execute(() =>
        {
            // USS properties
            ItemSize.x = InventoryController.Instance.Slots[0].resolvedStyle.width * 0.8f;
            ItemSize.y = InventoryController.Instance.Slots[0].resolvedStyle.height * 0.8f;
            style.width = ItemSize.x;
            style.height = ItemSize.y;

            float x = Random.Range(box.Min.x, box.Max.x - ItemSize.x);
            float y = Random.Range(box.Min.y, box.Max.y - ItemSize.y);

            style.left = x;
            style.top = y;
            style.opacity = 100;
        });

        OnStartDrag += InventoryController.Instance.OnPointerDown;

    }

    /// <summary>
    /// Invokes the method to start dragging this item
    /// </summary>
    void OnPointerDown(PointerDownEvent evt)
    {
        if (evt.button != 0) return;

        OnStartDrag.Invoke(evt.position, this);
        evt.StopPropagation();
    }
}
