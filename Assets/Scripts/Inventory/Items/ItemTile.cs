using UnityEngine;
using UnityEngine.UIElements;
using System;

[UxmlElement]
public partial class ItemTile : VisualElement
{
    // Properties

    public Label DebugLabel { get; set; }

    /// <summary>
    /// The item that this tile belongs to
    /// </summary>
    public Item ParentItem { get; private set; }

    /// <summary>
    /// The index of this tile relative to the item
    /// (X = Column, Y = Row)
    /// </summary>
    public Vector2Int Index { get; private set; }

    /// <summary>
    /// The grid position of this item
    /// (X = Column, Y = Row)
    /// </summary>
    public Vector2Int Position { get; private set; }

    /// <summary>
    /// The slot containing this tile on the grid
    /// </summary>
    public Slot GridSlot { get { return InventoryController.Instance.GetSlot(Position.x, Position.y); } }

    public event Action<Vector2, Item> OnStartDrag = delegate { };

    public ItemTile()
    {
        RegisterCallback<MouseEnterEvent>(OnHover);
        RegisterCallback<MouseLeaveEvent>(OnHoverExit);
        RegisterCallback<PointerDownEvent>(OnPointerDown);

        OnStartDrag += InventoryController.Instance.OnPointerDown;

        DebugLabel = new Label("");
        DebugLabel.AddToClassList("debug-text");

        Add(DebugLabel);
    }

    #region Events

    /// <summary>
    /// Handles transformations when the user hovers over this item tile
    /// </summary>
    /// <param name="evt">Mouse enter event</param>
    private void OnHover(MouseEnterEvent evt)
    {
        if (ParentItem.IsHovering)
        {
            return;
        }

        ParentItem.SetScale(new Vector2(1.05f, 1.05f));
        ParentItem.IsHovering = true;
    }

    /// <summary>
    /// Handles transformations when the user's mouse leaves this item over this item tile
    /// </summary>
    /// <param name="evt">Mouse leave event</param>
    private void OnHoverExit(MouseLeaveEvent evt)
    {
        ParentItem.ResetScale();
        ParentItem.IsHovering = false;
    }

    /// <summary>
    /// Invokes the method to start dragging this item
    /// </summary>
    private void OnPointerDown(PointerDownEvent evt)
    {
        if (evt.button != 0 || !ParentItem.IsHovering) return;

        ParentItem.ResetTileColors();
        OnStartDrag.Invoke(evt.position, ParentItem);
        evt.StopPropagation();
    }
    #endregion

    public void SetIndex(int x, int y)
    {
        Index = new Vector2Int(x, y);
    }

    public void SetGridSlot(int x, int y)
    {
        Position = new Vector2Int(x, y);
        GridSlot.SetTile(this);
    }

    public void ClearGridSlot()
    {
        GridSlot.ClearTile();
    }

    public void SetParent(Item parent)
    {
        name = parent.name;
        ParentItem = parent;
    }

    public void SetColor()
    {
        string slotColor = GridSlot.Color;
        AddToClassList($"item-{slotColor}");
    }

    public void RemoveColor()
    {
        string slotColor = GridSlot.Color;
        RemoveFromClassList($"item-{slotColor}");
    }
}