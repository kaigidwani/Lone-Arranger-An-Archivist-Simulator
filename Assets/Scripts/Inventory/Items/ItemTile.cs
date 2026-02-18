using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

[UxmlElement]
public partial class ItemTile : VisualElement
{
    public Item ParentItem { get; private set; }
    public Vector2Int Index { get; private set; }

    public Vector2Int Position { get; private set; }

    public Slot GridSlot
    {
        get
        {
            return InventoryController.Instance.Grid[Position.x][Position.y];
        }
    }

    public ItemTile()
    {

    }

    public void SetIndex(int x, int y)
    {
        this.Index = new Vector2Int(x, y);
    }
    public void SetGridSlot(int x, int y)
    {
        InventoryController.Instance.Grid[x][y].SetItem(ParentItem);
        this.Position = new Vector2Int(x, y);
    }

    public void ClearGridSlot()
    {
        GridSlot.ClearItem();
    }


    public void SetParent(Item parent)
    {
        ParentItem = parent;
    }

    public void SetColor()
    {
        string slotColor = GridSlot.GetColor();
        AddToClassList($"item-{slotColor}");
    }

    public void RemoveColor()
    {
        string slotColor = GridSlot.GetColor();
        RemoveFromClassList($"item-{slotColor}");
    }
}