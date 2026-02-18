using System;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using static UnityEditor.PlayerSettings;
using static UnityEditor.Progress;
using Random = UnityEngine.Random;
using UnityEngine.Tilemaps;
using UnityEngine.WSA;

[UxmlElement]
public partial class Item : VisualElement
{
    // Fields

    private Slot _currentSlot;
    private bool _isHovering;

    // Properties
    public ItemTile Pivot { get; private set; }

    public bool IsPlaced;

    public string Name;
    public Sprite BaseSprite;
    public Vector2 Dimensions;
    public int[][] Shape;
    public List<ItemTile> Tiles;
    public event Action<Vector2, Item> OnStartDrag = delegate { };

    public Item()
    {
        _currentSlot = null;
        RegisterCallback<PointerDownEvent>(OnPointerDown);
        RegisterCallback<MouseEnterEvent>(OnHover);
        RegisterCallback<MouseLeaveEvent>(OnHoverExit);
    }

    /// <summary>
    /// Invokes the method to start dragging this item
    /// </summary>
    void OnPointerDown(PointerDownEvent evt)
    {
        if (evt.button != 0 || !_isHovering) return;

        ResetTileColors();
        OnStartDrag.Invoke(evt.position, this);
        evt.StopPropagation();
    }

    /// <summary>
    /// Handles transformations when the user hovers over this item
    /// </summary>
    /// <param name="evt">Mouse over event</param>
    void OnHover(MouseEnterEvent evt)
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

    /// <summary>
    /// Generates a random item inside of the accessioning box
    /// </summary>
    /// <param name="box">The element where this item will spawn</param>
    public void Spawn(Accessioning box)
    {
        AddToClassList("item");

        // Generate random item from current item pool
        PlaceableItemSO type = null;
        if (InventoryController.Instance.ItemPool.Length > 0)
        {
            //sDebug.Log("items in rotation: " + InventoryController.Instance.ItemPool.Length);
            type = InventoryController.Instance.ItemPool[Random.Range(0, InventoryController.Instance.ItemPool.Length)];

            Name = type.Name;
            BaseSprite = type.Sprite;
            Dimensions = type.Dimensions;
            Shape = type.Shape;

            ConstructItem();
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
    /// Builds out the item in multiple, sliced tiles
    /// </summary>
    /// <param name="type">The type of item to build</param>
    void ConstructItem()
    {
        // How big each individual tile should be
        float tileWidth = InventoryController.Instance.SlotSize.x ;
        float tileHeight = InventoryController.Instance.SlotSize.y ;

        // Parent container should be as big as the item is (totally)
        style.width = Dimensions.x * tileWidth;
        style.height = Dimensions.y * tileHeight;

        Tiles = new List<ItemTile>();
        for (int row = 0; row < Dimensions.y; row++)
        {
            for (int col = 0; col < Dimensions.x; col++)
            {
                // Empty parts of the shape don't get "made"
                if (Shape[row][col] == 0)
                {
                    continue;
                }
                
                ItemTile tile = new ItemTile();
                tile.SetParent(this);
                tile.SetIndex(col, row);
                

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
                Tiles.Add(tile);
            }
        }
    }

    void SetScale(Vector2 scale)
    {
        style.scale = new StyleScale(scale);
    }


    /// <summary>
    /// Places the dragged item into a slot
    /// </summary>
    /// <param name="startSlot">The slot that the user places the item in</param>
    public void Place(VisualElement dest, Slot startSlot)
    {
        RemoveFromHierarchy(); // Remove from accessioning box
        dest.Add(this);
        
        RemoveFromClassList("item");
        AddToClassList("item-slotted");

        float pivotOffsetX = Pivot.Index.x * InventoryController.Instance.SlotSize.x;
        float pivotOffsetY = Pivot.Index.y * InventoryController.Instance.SlotSize.y;

        style.left = startSlot.resolvedStyle.left - pivotOffsetX;
        style.top = startSlot.resolvedStyle.top - pivotOffsetY;

        Vector2Int start = InventoryController.Instance.GetSlotIndex(startSlot);
        Vector2Int pivot = Pivot.Index;

        foreach (ItemTile tile in Tiles)
        {
            Vector2Int t = tile.Index;

            int gridRow = start.y + (t.y - pivot.y);
            int gridCol = start.x + (t.x - pivot.x);

            Slot slot = InventoryController.Instance.GetSlot(gridRow, gridCol);
            slot.SetItem(this);

            tile.SetGridSlot(gridRow, gridCol);
        }

        //SendToBack();
        SetTileColors();
        IsPlaced = true;
    }

    public void SetPivot(ItemTile tile)
    {
        tile.AddToClassList("pivot");
        Pivot = tile;
        
    }

    public void ClearPivot()
    {
        Pivot.RemoveFromClassList("pivot");
        Pivot = null;
    }

    public void SetTileColors()
    {
        foreach (ItemTile tile in Tiles)
        {
            tile.SetColor();
        }
    }

    private void ResetTileColors()
    {
        foreach (ItemTile tile in Tiles)
        {
            tile.RemoveColor();
        }
    }
}
