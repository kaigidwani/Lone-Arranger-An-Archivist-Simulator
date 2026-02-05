using System;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using static UnityEditor.PlayerSettings;
using static UnityEditor.Progress;
using Random = UnityEngine.Random;

[UxmlElement]
public partial class Item : VisualElement
{
    // Fields

    private Slot _currentSlot;
    private bool _isHovering;

    // Properties

    public Sprite BaseSprite;
    public Vector2 Dimensions;
    public event Action<Vector2, Item> OnStartDrag = delegate { };

    public VisualElement PivotElement
    {
        get
        {
            return this.Q("item-pivot");
        }
    }

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
        RegisterCallback<MouseOverEvent>(OnHover);
        RegisterCallback<MouseLeaveEvent>(OnHoverExit);
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
        if (evt.button != 0 || !_isHovering) return;

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
        float tileWidth = InventoryController.Instance.SlotSize.x ;
        float tileHeight = InventoryController.Instance.SlotSize.y ;

        // Parent container should be as big as the item is (totally)
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
                if (type.Shape[row][col] == 2)
                {
                    tile.AddToClassList("item-pivot");
                }

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

    /// <summary>
    /// Handles transformations when the user hovers over this item
    /// </summary>
    /// <param name="evt">Mouse over event</param>
    void OnHover(MouseOverEvent evt)
    {
        foreach (VisualElement elem in Children())
        {
            Rect r = elem.worldBound;

            // Item tiles that are empty should not change the scale or be draggable
            if (r.Contains(evt.mousePosition))
            {
                SetScale(new Vector2(1.05f, 1.05f));
                _isHovering = true;

                return;
            }
        }

        SetScale(new Vector2(1, 1));
        _isHovering = false;
    }

    /// <summary>
    /// Handles transformations when the user's mouse leaves this item over this item
    /// </summary>
    /// <param name="evt">Mouse leave event</param>
    void OnHoverExit(MouseLeaveEvent evt)
    {
        SetScale(new Vector2(1, 1));
        _isHovering = false;
    }

    void SetScale(Vector2 scale)
    {
        style.scale = new StyleScale(scale);
    }
}
