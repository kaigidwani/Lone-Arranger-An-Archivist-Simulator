using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using static UnityEngine.Rendering.DebugUI.Table;

[UxmlElement]
public partial class Item : VisualElement
{
    // Fields

    private PlaceableItemSO _itemSO;

    // Properties

    /// <summary>
    /// Contains information about this item
    /// </summary>
    public PlaceableItemSO Type { get { return _itemSO; } }

    /// <summary>
    /// List of individual tiles that make up this item
    /// </summary>
    public List<ItemTile> Tiles { get; private set; }

    /// <summary>
    /// The item tile the user is currently dragging
    /// </summary>
    public ItemTile Pivot { get; private set; }

    /// <summary>
    /// The upper-left-most grid index of this item
    /// (X = Row, Y = Column)
    /// </summary>
    public Vector2Int RootGridIndex { get; private set; }

    public bool IsPlaced;
    public bool IsHovering;

    public Item()
    {

    }

    /// <summary>
    /// Builds out a randomly generated item in multiple, sliced tiles
    /// </summary>
    private void ConstructItem()
    {
        // How big each individual tile should be
        float tileWidth = InventoryController.Instance.SlotSize.x;
        float tileHeight = InventoryController.Instance.SlotSize.y;

        // Parent container should be as big as the item is (totally)
        style.width = _itemSO.Dimensions.y * tileWidth;
        style.height = _itemSO.Dimensions.x * tileHeight;

        Tiles = new List<ItemTile>();
        for (int row = 0; row < _itemSO.Dimensions.x; row++)
        {
            for (int col = 0; col < _itemSO.Dimensions.y; col++)
            {
                // Empty parts of the shape don't get "made"
                if (_itemSO.Shape[row][col] == 0)
                {
                    continue;
                }

                ItemTile tile = new ItemTile();
                tile.SetParent(this);
                tile.SetIndex(row, col);

                tile.AddToClassList("item-tile");

                tile.style.width = tileWidth;
                tile.style.height = tileHeight;
                tile.style.left = col * tileWidth;
                tile.style.top = row * tileHeight;
                tile.style.backgroundImage = _itemSO.Sprite.texture; // They all use different parts of the same image

                // Makes each item based on a constant size
                tile.style.backgroundSize = new BackgroundSize(
                    new Length(_itemSO.Dimensions.y * 100, LengthUnit.Percent),
                    new Length(_itemSO.Dimensions.x * 100, LengthUnit.Percent)
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

    /// <summary>
    /// Finds the upper-left-most grid position of this item
    /// </summary>
    /// <returns>The index of the upper-left-most grid position</returns>
    private Vector2Int GetRootGridPosition()
    {
        int minX = int.MaxValue;
        int minY = int.MaxValue;  

        foreach (ItemTile tile in Tiles)
        {
            if (tile.Position.x < minX || (tile.Position.x == minX && tile.Position.y < minY))
            {
                minX = tile.Position.x;
                minY = tile.Position.y;
                
            }
        }

        return new Vector2Int(minX, minY);
    }

    /// <summary>
    /// Generates a random item inside of the accessioning box
    /// </summary>
    /// <param name="box">The element where this item will spawn</param>
    public void Spawn(Accessioning box)
    {
        AddToClassList("item");

        // Generate random item from current item pool
        if (InventoryController.Instance.ItemPool.Length > 0)
        {
            _itemSO = InventoryController.Instance.ItemPool[Random.Range(0, InventoryController.Instance.ItemPool.Length)];
            name = _itemSO.Name;

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

        float rowOffset = Pivot.Index.x * InventoryController.Instance.SlotSize.y;
        float colOffset = Pivot.Index.y * InventoryController.Instance.SlotSize.x;

        style.left = startSlot.resolvedStyle.left - colOffset;
        style.top = startSlot.resolvedStyle.top - rowOffset;

        foreach (ItemTile tile in Tiles)
        {
            int gridRow = startSlot.GridIndex.x + (tile.Index.x - Pivot.Index.x);
            int gridCol = startSlot.GridIndex.y + (tile.Index.y - Pivot.Index.y);

            tile.SetGridSlot(gridRow, gridCol);
            Debug.Log($"{name}-- row: {gridRow}, col: {gridCol}");
        }

        RootGridIndex = GetRootGridPosition();

        SetTileColors();
        IsPlaced = true;
    }

    public void SetScale(Vector2 scale)
    {
        style.scale = new StyleScale(scale);
    }

    public void ResetScale()
    {
        style.scale = new StyleScale(Vector2.one);
    }

    public void SetTileColors()
    {
        foreach (ItemTile tile in Tiles)
        {
            tile.SetColor();
        }
    }

    public void ResetTileColors()
    {
        foreach (ItemTile tile in Tiles)
        {
            tile.RemoveColor();
        }
    }

    public void SetPivot(ItemTile tile)
    {
        tile.AddToClassList("pivot");
        Pivot = tile;

    }

    public void ResetPivot()
    {
        Pivot.RemoveFromClassList("pivot");
        Pivot = null;
    }
}
