using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

[UxmlElement]
public partial class ItemTile : VisualElement
{
    public Vector2Int Index { get; private set; }

    public ItemTile()
    {

    }

    public void SetGridIndex(int x, int y)
    {
        this.Index = new Vector2Int(x, y);
    }

    public void SetColor()
    {
        string slotColor = InventoryController.Instance.Grid[Index.x][Index.y].GetColor();
        AddToClassList($"item-{slotColor}");
    }

    public void RemoveColor()
    {
        string slotColor = InventoryController.Instance.Grid[Index.x][Index.y].GetColor();
        RemoveFromClassList($"item-{slotColor}");
    }
}