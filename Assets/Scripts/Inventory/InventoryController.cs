using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using static UnityEditor.Progress;
using Cursor = UnityEngine.Cursor;


public class InventoryController : MonoBehaviour
{
    // Fields
    private VisualElement _root;
    List<Slot> _slotList;

    private static GhostIcon _ghostIcon;

    private static bool _isDragging;
    private static Item _draggedItem;

    // Properties
    
    public int Width = 6;
    public int Height = 6;

    public static InventoryController Instance;

    public Slot[][] Grid;

    public Vector2 SlotSize
    {
        get {
            
            return new Vector2(
                Grid[0][0].resolvedStyle.width,
                Grid[0][0].resolvedStyle.height

            );
        }
    }

    public List<Item> Items;
    public PlaceableItemSO[] ItemPool;

    public event Action<Item> OnDrop;

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            Instance = this;
        }
    }

    void Start()
    {
        _root = GetComponent<UIDocument>().rootVisualElement;
        _root.RegisterCallback<PointerMoveEvent>(OnPointerMove);
        _root.RegisterCallback<PointerUpEvent>(OnPointerUp);

        Items = new List<Item>();

        int row = 0;
        int col = 0;
        _slotList = _root.Query<Slot>().ToList();
        Grid = new Slot[Height][];
        foreach (Slot slot in _slotList)
        {
            if (Grid[row] == null)
            {
                Grid[row] = new Slot[Width];
            }

            //Debug.Log($"row: {row}, col: {col}");
            Grid[row][col] = slot;
            
            col++;
            if (col == Width)
            {
                row++;
                col = 0;
            }
        }

        _ghostIcon = _root.Q<GhostIcon>();
    }

    

    /// <summary>
    /// Toggles visibility of ghost icon on click
    /// </summary>
    /// <param name="pos">Where the user clicked</param>
    /// <param name="item">The item the user clicked on</param>
    public void OnPointerDown(Vector2 pos, Item item)
    {
        _isDragging = true;
        _draggedItem = item;

        if (_draggedItem.CurrentSlot != null)
        {
            _draggedItem.CurrentSlot.ClearItem();
        }

        Cursor.visible = false;

        _draggedItem.style.visibility = Visibility.Hidden;

        foreach (ItemTile tile in _draggedItem.Tiles)
        {
            Rect r = tile.worldBound;
            if (r.Contains(pos))
            {
                if (_draggedItem.Pivot != null)
                {
                    _draggedItem.ResetPivot();
                }
                
                _draggedItem.SetPivot(tile);

            }
        }

        _ghostIcon.SetIcon(item);
        _ghostIcon.SetToMousePosition();
    }

    /// <summary>
    /// Handles dragging across the screen
    /// </summary>
    void OnPointerMove(PointerMoveEvent evt)
    {
        if (!_isDragging) return;

        _ghostIcon.SetToMousePosition();
    }

    /// <summary>
    /// Handles dropping items on the screen
    /// </summary>
    void OnPointerUp(PointerUpEvent evt)
    {
        if (!_isDragging) return;

        _isDragging = false;

        Cursor.visible = true;

        if (_draggedItem.Pivot != null)
        {
            _draggedItem.ResetPivot();
        }
        
        _draggedItem.style.visibility = Visibility.Visible;

        OnDrop?.Invoke(_draggedItem);
    
        // Find slot under mouse
        Vector2 mousePos = evt.position;

        Slot hoveredSlot = null;

        foreach (Slot s in _slotList) 
        {
            Rect r = s.worldBound;
            if (r.Contains(mousePos))
            {
                hoveredSlot = s;
            }
        }
        
        if (hoveredSlot != null && CanPlace(hoveredSlot))
        {
            //Debug.Log("placed");
            _draggedItem.Place(hoveredSlot);
        }
        else if (_draggedItem.CurrentSlot != null)
        {
            
            _draggedItem.Place(_draggedItem.CurrentSlot);
        }
        else
        {
            //Debug.Log("Error, could not place");
        }

        _ghostIcon.RefreshVisual();
    } 

    /// <summary>
    /// Determines whether the slot the user places the item is valid
    /// </summary>
    /// <param name="slot">The slot the user places the item in</param>
    /// <returns>Whether the item can be placed in the given slot</returns>
    private bool CanPlace(Slot startSlot)
    {
        Vector2Int start = GetSlotIndex(startSlot);

        Debug.Log("item pivot: " + _draggedItem.Pivot);
        Vector2Int pivot = _draggedItem.Pivot.Index;

        foreach (ItemTile tile in _draggedItem.Tiles)
        {
            Vector2Int t = tile.Index;

            int gridRow = start.y + (t.y - pivot.y);
            int gridCol = start.x + (t.x - pivot.x);

            // Bounds logic
            if (gridRow < 0 || gridRow >= Height ||
                gridCol < 0 || gridCol >= Width)
            {
                return false;
            }

            // Occupancy logic
            if (!Grid[gridRow][gridCol].IsFree)
            {
                return false;
            }
        }

        return true;
    }

    public Vector2Int GetSlotIndex(Slot slot)
    {
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                if (Grid[y][x] == slot)
                {
                    return new Vector2Int(x, y);
                }
            }
        }

        return Vector2Int.zero;
    }

    
}
