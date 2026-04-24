using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public enum ItemState
{
    InAccessioning,
    InInventory,
    InDonationBox,
}

[UxmlElement]
public partial class Item : VisualElement
{
    // Fields
    private PlaceableItemSO _itemSO;
    private float _width;
    private float _height;

    // Properties

    public ItemState CurrentState;

    /// <summary>
    /// The rotation of this item before it was clicked
    /// </summary>
    public int StoredRotation { get; set; }

    /// <summary>
    /// This item's current rotation (0-360)
    /// </summary>
    public int Rotation { get; set; }

    /// <summary>
    /// This item's current shape
    /// </summary>
    public int[][] Shape { get; set; }

    /// <summary>
    /// The width of this item in tiles
    /// </summary>
    public int WidthInTiles { get; set; }

    /// <summary>
    /// The height of this item in tiles
    /// </summary>
    public int HeightInTiles { get; set; }

    /// <summary>
    /// Contains information about this item
    /// </summary>
    public PlaceableItemSO SO { get { return _itemSO; } }

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

    public bool IsHovering;

    public Item()
    {
        pickingMode = PickingMode.Ignore;
        CurrentState = ItemState.InAccessioning;
    }

    /// <summary>
    /// Builds out a list of sliced tiles based on the item's shape
    /// </summary>
    private void GenerateItemTiles()
    {
        // How big each individual tile should be
        float tileWidth = GameObject.Find("UIDoc").GetComponent<InventoryController>().ItemTileSize.x;
        float tileHeight = GameObject.Find("UIDoc").GetComponent<InventoryController>().ItemTileSize.y;

        Tiles = new List<ItemTile>();
        for (int row = 0; row < _itemSO.BaseHeight; row++)
        {
            for (int col = 0; col < _itemSO.BaseWidth; col++)
            {
                // Empty parts of the shape don't get "made"
                if (_itemSO.BaseShape[row][col] == 0)
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

                Add(tile);
                Tiles.Add(tile);

                tile.DebugLabel.text = $"({tile.Index.x}, {tile.Index.y})";
                tile.DebugLabel.visible = GameObject.Find("UIDoc").GetComponent<InventoryController>().ShowDebug;
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
    /// Rotates the item in the specified direction
    /// </summary>
    /// <param name="dir">The direction to rotate (positive = clockwise)</param>
    public void Rotate(int dir)
    {
        Rotation = (Rotation + 90 * dir) % 360; // clamp it to 360 deg

        Rotate rot = new Rotate(new Angle(Rotation, AngleUnit.Degree));
        style.rotate = rot;

        foreach (ItemTile tile in Tiles)
        {
            if (dir >= 0) // clockwise
            {
                tile.SetIndex(tile.Index.y, HeightInTiles - 1 - tile.Index.x);
            }
            else // counter-clockwise
            {
                tile.SetIndex(WidthInTiles - 1 - tile.Index.y, tile.Index.x);
            }

            // Keep debug label orientation
            tile.DebugLabel.style.rotate = new Rotate(new Angle(360.0f - rot.angle.value, AngleUnit.Degree));
            tile.DebugLabel.text = $"({tile.Index.x}, {tile.Index.y})";
        }

        _itemSO.RotateItemShape(this, dir);
        float oldWidth = _width;
        _width = _height;
        _height = oldWidth;

    }

    // Rotates the item to match a specific angle
    public void RotateTo(int deg)
    {
        while (Rotation != deg)
        {
            Rotate(1);
        }
    }

    /// <summary>
    /// Resets the rotation of the item to its value before the user started rotating it.
    /// </summary>
    public void RevertRotation()
    {
        int correctionCount = (Rotation - StoredRotation) / 90;

        for (int i = 0; i < Mathf.Abs(correctionCount); i++)
        {
            if (correctionCount < 0)
            {
                Rotate(1);
            }
            else
            {
                Rotate(-1);
            }
        }
    }

    /// <summary>
    /// Generates a random item inside of the accessioning box
    /// </summary>
    /// <param name="box">The element where this item will spawn</param>
    public void Spawn(Accessioning box)
    {
        AddToClassList("item");

        // Generate random item from current item pool
        if (GameManager.Instance.ItemPool.Length > 0)
        {
            _itemSO = GameManager.Instance.ItemPool[Random.Range(0, GameManager.Instance.ItemPool.Length)];           
            name = _itemSO.Name;

            WidthInTiles = _itemSO.BaseWidth;
            HeightInTiles = _itemSO.BaseHeight;

            _itemSO.GenerateShape();
            Shape = _itemSO.BaseShape;

            GenerateItemTiles();
        }

        // Parent container should be as big as the item is (totally)
        _width = _itemSO.BaseWidth * GameObject.Find("UIDoc").GetComponent<InventoryController>().ItemTileSize.x;
        _height = _itemSO.BaseHeight * GameObject.Find("UIDoc").GetComponent<InventoryController>().ItemTileSize.y;

        Vector2 pos = box.GetRandomPoint(_width, _height);

        style.left = pos.x;
        style.top = pos.y;
        style.width = _width;
        style.height = _height;

        style.backgroundImage = _itemSO.Sprite.texture;
        style.opacity = 100;

    }

    /// <summary>
    /// Places the dragged item into a slot
    /// </summary>
    /// <param name="startSlot">The slot that the user places the item in</param>
    public void PlaceInSlot(InventoryController inv, VisualElement dest, Slot startSlot)
    {
        RemoveFromHierarchy(); // Remove from accessioning box 
        RemoveFromClassList("item");
        AddToClassList("item-slotted");
        // ------------------------------

        float rowOffset = 0;
        float colOffset = 0;
        if (Pivot != null)
        {
            rowOffset = Pivot.Index.x * inv.SlotSize.y;
            colOffset = Pivot.Index.y * inv.SlotSize.x;
        }

        float drawOffset = 0;
        if (Rotation % 180 != 0)
        {
            drawOffset = (WidthInTiles - HeightInTiles) * inv.SlotSize.x / 2;
        }

        style.left = startSlot.resolvedStyle.left - colOffset + drawOffset;
        style.top = startSlot.resolvedStyle.top - rowOffset - drawOffset;

        foreach (ItemTile tile in Tiles)
        {
            int gridRow = startSlot.GridIndex.x + tile.Index.x;
            int gridCol = startSlot.GridIndex.y + tile.Index.y;

            if (Pivot != null)
            {
                gridRow -= Pivot.Index.x;
                gridCol -= Pivot.Index.y;
            }

            tile.SetGridSlot(gridRow, gridCol);
            
            // Make sure players can see what color items they have in the box
            if (CurrentState != ItemState.InDonationBox)
            {
                ResetTileColors();
                tile.SetColor();
            }
            
        }

        RootGridIndex = GetRootGridPosition();

        dest.Add(this);
    }

    public void ReturnToAccessioning(Accessioning box, Vector2 mouse)
    {
        RemoveFromHierarchy(); // Remove from inventory
        RemoveFromClassList("item-slotted");
        AddToClassList("item");
        ResetTileColors();
        box.Add(this);

        Vector2 mousePos = UIHelpers.SetItemPivotToMouse(Pivot, mouse);
        Vector2 itemPos = mousePos - new Vector2(box.worldBound.x, box.worldBound.y);
        style.left = itemPos.x;
        style.top = itemPos.y;

        schedule.Execute(() =>
        {
            Debug.Log("the item's max is: " + worldBound.max.x);
            Debug.Log("the box's max is: " + box.worldBound.max.x);

            // Scooch back into box if placed outside
            if (worldBound.min.x < box.worldBound.min.x)
            {
                style.left = itemPos.x + Mathf.Abs(worldBound.min.x - box.worldBound.min.x);
            }

            if (worldBound.min.y < box.worldBound.min.y)
            {
                style.top = itemPos.y + Mathf.Abs(worldBound.min.y - box.worldBound.min.y);
            }

            if (worldBound.max.x > box.worldBound.max.x)
            {
                style.left = itemPos.x - Mathf.Abs(worldBound.max.x - box.worldBound.max.x);
            }

            if (worldBound.max.y > box.worldBound.max.y)
            {
                style.top = itemPos.y - Mathf.Abs(worldBound.max.y - box.worldBound.max.y);
            }
        });
    }

    public void SetScale(Vector2 scale)
    {
        style.scale = new StyleScale(scale);
    }

    public void ResetScale()
    {
        style.scale = new StyleScale(Vector2.one);
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

    public void SetState(ItemState newState)
    {
        if (CurrentState == newState)
        {
            return;
        }  

        if (CurrentState == ItemState.InAccessioning && newState == ItemState.InInventory) // moving from accessioning to inventory
        {
            AccessioningController.Instance.TakeDonation();
        }
        else if (CurrentState == ItemState.InInventory && newState == ItemState.InAccessioning) // moving from inventory to accessioning
        {
            AccessioningController.Instance.ReturnDonation();
        }

        CurrentState = newState;
    }
}
