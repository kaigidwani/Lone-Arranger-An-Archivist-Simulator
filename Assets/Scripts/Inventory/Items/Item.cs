using System;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

[UxmlElement]
public partial class Item : VisualElement
{
    // Fields

    private ItemInfo _itemInfo;

    // Properties

    public Sprite BaseSprite;
    public Vector2 Dimensions;
    public event Action<Vector2, Item> OnStartDrag = delegate { };

    public Item()
    {
        AddToClassList("item");
        RegisterCallback<PointerDownEvent>(OnPointerDown);
    }

    /// <summary>
    /// Generates a random item inside of the accessioning box
    /// </summary>
    /// <param name="box">The element where this item will spawn</param>
    public void Spawn(Accessioning box)
    {

        // Generate random item from current item pool
        ItemInfo type = null;
        if (InventoryController.Instance.ItemPool.Length > 0)
        {
            type = InventoryController.Instance.ItemPool[Random.Range(0, InventoryController.Instance.ItemPool.Length - 1)];

            _itemInfo = type;
            style.backgroundImage = type.Sprite.texture;

            BaseSprite = _itemInfo.Sprite;
            Dimensions = _itemInfo.Dimensions;
        }

        schedule.Execute(() =>
        {

            float x = Random.Range(box.Min.x, box.Max.x - resolvedStyle.width);
            float y = Random.Range(box.Min.y, box.Max.y - resolvedStyle.height);

            // USS properties
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
