using System;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.Progress;
using Random = UnityEngine.Random;

[UxmlElement]
public partial class Item : VisualElement
{
    // Fields

    private Slot _currentSlot;

    // Properties

    public Sprite BaseSprite;
    public Vector2 Dimensions;
    public event Action<Vector2, Item> OnStartDrag = delegate { };

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
            Debug.Log("items in rotation: " + InventoryController.Instance.ItemPool.Length);
            type = InventoryController.Instance.ItemPool[Random.Range(0, InventoryController.Instance.ItemPool.Length)];

            BaseSprite = type.Sprite;
            Dimensions = type.Dimensions;

            ConstructItem(type);
        }

        schedule.Execute(() =>
        {
            float x = Random.Range(box.Min.x, box.Max.x - resolvedStyle.width);
            float y = Random.Range(box.Min.y, box.Max.y - resolvedStyle.height);

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

    /// <summary>
    /// Builds out the item in multiple, sliced tiles
    /// </summary>
    /// <param name="type">The type of item to build</param>
    void ConstructItem(ItemInfo type)
    {
        // How big each individual tile should be
        float tileWidth = InventoryController.Instance.SlotSize.x * 0.8f;
        float tileHeight = InventoryController.Instance.SlotSize.y * 0.8f;

        // Parent container should be as big as the item is (totally)
        // *** Note: need to account for hovering and clicking on empty tiles
        style.width = Dimensions.x * tileWidth;
        style.height = Dimensions.y * tileHeight;

        for (int row = 0; row < Dimensions.y; row++)
        {
            for (int col = 0; col < Dimensions.x; col++)
            {
                // Empty parts of the shape don't get "made"
                if (type.Shape[row][col] == 0)
                {
                    continue;
                }

                VisualElement tile = new VisualElement();
                tile.AddToClassList("item-tile");

                tile.style.width = tileWidth;
                tile.style.height = tileHeight;
                tile.style.left = col * tileWidth;
                tile.style.top = row * tileHeight;
                tile.style.backgroundImage = BaseSprite.texture; // They all use different parts of the same image

                // Makes each item based on a constant size
                tile.style.backgroundSize = new BackgroundSize(
                    new Length(Dimensions.x * 100, LengthUnit.Percent),
                    new Length(Dimensions.y * 100, LengthUnit.Percent)
                );

                // Offset contents of tile to slice the image
                tile.style.backgroundPositionX = new BackgroundPosition(
                    BackgroundPositionKeyword.Left, -col * tileWidth
                );

                tile.style.backgroundPositionY = new BackgroundPosition(
                    BackgroundPositionKeyword.Top, -row * tileHeight
                );

                Add(tile);
            }
        }
    }
}
