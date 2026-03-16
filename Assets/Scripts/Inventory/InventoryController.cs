using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;


public class InventoryController : MonoBehaviour
{
    // Fields

    private VisualElement _root;
    private VisualElement _itemContainer;
    private float _itemScale = 1;

    private List<Slot> _slotList;

    private static GhostIcon _ghostIcon;

    private static bool _isDragging;
    private static Item _draggedItem;

    // Properties

    public static InventoryController Instance;
    public int Width = 6;
    public int Height = 6;

    public PlaceableItemSO[] ItemPool;

    [HideInInspector] public bool ShowDebug;

    /// <summary>
    /// The list of slots represented as a 2D array
    /// (X = Row, Y = Column)
    /// </summary>
    public Slot[][] Grid { get; private set; }

    /// <summary>
    /// Final width and height of each individual slot
    /// </summary>
    public Vector2 SlotSize
    {
        get {
            
            return new Vector2(
                GetSlot(0, 0).resolvedStyle.width,
                GetSlot(0, 0).resolvedStyle.height

            );
        }
    }

    public Vector2 ItemTileSize
    {
        get { return SlotSize * _itemScale; }
    }

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
        _itemContainer = _root.Q("ItemLayer");
        _ghostIcon = _root.Q<GhostIcon>();

        _slotList = _root.Query<Slot>().ToList();

        int row = 0;
        int col = 0;
        
        // Initializing grid from slots
        Grid = new Slot[Height][];
        foreach (Slot slot in _slotList)
        {
            if (Grid[row] == null)
            {
                Grid[row] = new Slot[Width];
            }

            Grid[row][col] = slot;
            slot.GridIndex = new Vector2Int(row, col);
            
            col++;
            if (col == Width) // at end of row
            {
                row++;
                col = 0;
            }
        }

        // Making sure the slot and itewm layers are the same size
        GetSlot(0, 0).RegisterCallbackOnce<GeometryChangedEvent>((evt) =>
        {
            VisualElement slotLayer = _root.Q("SlotLayer");

            _itemContainer.style.width = slotLayer.resolvedStyle.width;
            _itemContainer.style.height = slotLayer.resolvedStyle.height;

            _itemContainer.style.left = slotLayer.resolvedStyle.left;
            _itemContainer.style.top = slotLayer.resolvedStyle.top;
        });
        
        _root.RegisterCallback<PointerMoveEvent>(OnPointerMove);
        _root.RegisterCallback<PointerUpEvent>(OnPointerUp);

        ShowDebug = false;
        SetDebug();
    }

    #region Events

    /// <summary>
    /// Toggles visibility of ghost icon on click
    /// </summary>
    /// <param name="pos">Where the user clicked</param>
    /// <param name="item">The item the user clicked on</param>
    public void OnPointerDown(Vector2 pos, Item item)
    {
        _isDragging = true;
        _draggedItem = item;

        if (_draggedItem.IsPlaced)
        {
            foreach (ItemTile tile in _draggedItem.Tiles)
            {
                tile.ClearGridSlot();
            }
        }

        if (!ShowDebug)
        {
            Cursor.visible = false;
        }

        _draggedItem.style.visibility = Visibility.Hidden;

        foreach (ItemTile tile in _draggedItem.Tiles)
        {
            Rect r = tile.worldBound;
            if (r.Contains(pos))
            {                
                _draggedItem.SetPivot(tile);

            }
        }

        _ghostIcon.SetVisual(_draggedItem);        
    }

    /// <summary>
    /// Handles dragging across the screen
    /// </summary>
    public void OnPointerMove(PointerMoveEvent evt)
    {
        if (!_isDragging) return;

        _ghostIcon.UpdatePosition(_draggedItem.Pivot);
    }

    /// <summary>
    /// Handles dropping items on the screen
    /// </summary>
    public void OnPointerUp(PointerUpEvent evt)
    {
        if (!_isDragging) return;

        _isDragging = false;

        Cursor.visible = true;
        
        _draggedItem.style.visibility = Visibility.Visible;

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

        if (CanPlace(hoveredSlot))
        {
            _draggedItem.Place(_itemContainer, hoveredSlot);    
        }
        else
        {
            _draggedItem.RevertRotation();

            if (_draggedItem.IsPlaced)
            {
                _draggedItem.Place(_itemContainer, _draggedItem.Pivot.GridSlot);
            }
        }

        _ghostIcon.ResetVisual();
        _draggedItem.ResetPivot();
        ReorderItems();
    }

    public void OnRotateCW(InputAction.CallbackContext ctx)
    {
        if (!_isDragging || ctx.phase != InputActionPhase.Performed)
        {
            return;
        }   

        int dir = 1;
        _draggedItem.Rotate(dir);
        _ghostIcon.Rotate(dir, _draggedItem.Pivot);
    }

    public void OnRotateCCW(InputAction.CallbackContext ctx)
    {
        if (!_isDragging || ctx.phase != InputActionPhase.Performed)
        {
            return;
        }

        int dir = -1;
        _draggedItem.Rotate(dir);
        _ghostIcon.Rotate(dir, _draggedItem.Pivot);
    }

    #endregion

    /// <summary>
    /// Determines whether the slot the user places the item is valid
    /// </summary>
    /// <param name="startSlot">The slot the user places the item in</param>
    /// <returns>Whether the item can be placed in the given slot</returns>
    private bool CanPlace(Slot startSlot)
    {
        if (startSlot == null)
        {
            return false;
        }

        foreach (ItemTile tile in _draggedItem.Tiles)
        {
            int gridRow = startSlot.GridIndex.x + (tile.Index.x - _draggedItem.Pivot.Index.x);
            int gridCol = startSlot.GridIndex.y + (tile.Index.y - _draggedItem.Pivot.Index.y);

            // Bounds logic
            if (gridRow < 0 || gridRow >= Height ||
                gridCol < 0 || gridCol >= Width)
            {
                return false;
            }

            // Occupancy logic
            if (!GetSlot(gridRow, gridCol).IsFree)
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// All placed items are reordered by their grid position
    /// </summary>
    private void ReorderItems()
    {
        List<Item> items = _itemContainer.Children().OfType<Item>()
            .OrderBy(x => x.RootGridIndex.x)
            .OrderBy(y => y.RootGridIndex.y).ToList();

        // Avoids z-indexing issues (being unable to click on certain items)
        for (int i = 0; i < items.Count; i++)
        {
            items[i].BringToFront();
        }

        foreach (Item item in items)
        {
            _itemContainer.Add(item);
        }
    }

    /// <summary>
    /// Finds the grid slot at the given index
    /// </summary>
    /// <param name="x">Row</param>
    /// <param name="y">Column</param>
    /// <returns>The grid slot at (x, y)</returns>
    public Slot GetSlot(int x, int y)
    {
        return Grid[x][y];
    }

    #region Debug
    public void SetDebug()
    {
        if (ShowDebug)
        {
            Debug.Log("Debug on");
        }
        else
        {
            Debug.Log("Debug off");
        }
        

        List<Item> items = _root.Query<Item>().ToList();
        for (int i = 0; i < items.Count; i++)
        {
            foreach (ItemTile tile in items[i].Tiles)
            {
                tile.DebugLabel.visible = ShowDebug;
            }
        }

        for (int i = 0; i < _slotList.Count; i++)
        {
            _slotList[i].DebugLabel.visible = ShowDebug;
        }

        _ghostIcon.DebugLabel.visible = ShowDebug;
    }

    public void OnToggleDebug(InputAction.CallbackContext ctx)
    {
        if (ctx.phase == InputActionPhase.Performed)
        {
            ShowDebug = !ShowDebug;

            SetDebug();
        }
    }

    #endregion
}
