using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements;

[UxmlElement]
public partial class Grid : VisualElement
{
    // Fields

    
    private Slot[][] _grid;

    // Properties

    public int Width;

    public int Height;

    public VisualElement SlotLayer;
    public List<Slot> SlotList;

    public VisualElement ItemLayer;

    public Grid()
    {
        
    }

    /// <summary>
    /// Finds the grid slot at the given index
    /// </summary>
    /// <param name="x">Row</param>
    /// <param name="y">Column</param>
    /// <returns>The grid slot at (x, y)</returns>
    public Slot GetSlot(int x, int y)
    {
        return _grid[x][y];
    }

    public void Setup(int width, int height)
    {
        Width = width;
        Height = height;

        SlotLayer = this.Q("SlotLayer");
        ItemLayer = this.Q("ItemLayer");

        ItemLayer.style.width = SlotLayer.resolvedStyle.width;
        ItemLayer.style.height = SlotLayer.resolvedStyle.height;

        ItemLayer.style.left = SlotLayer.resolvedStyle.left;
        ItemLayer.style.top = SlotLayer.resolvedStyle.top;

        SlotList = SlotLayer.Query<Slot>().ToList();

        int row = 0;
        int col = 0;

        // Initializing grid from slots
        _grid = new Slot[Height][];
        foreach (Slot slot in SlotList)
        {
            if (_grid[row] == null)
            {
                _grid[row] = new Slot[Width];
            }

            _grid[row][col] = slot;
            slot.GridIndex = new Vector2Int(row, col);

            col++;
            if (col == Width) // at end of row
            {
                row++;
                col = 0;
            }

            slot.DebugLabel.visible = GameObject.Find("UIDoc")
                .GetComponent<InventoryController>().ShowDebug;
            
        }
    }

    public bool TryGetDimensions()
    {
        return worldBound.width > 0;
    }

}
