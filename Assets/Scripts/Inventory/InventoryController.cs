using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;


public class InventoryController : MonoBehaviour
{
    // Fields
    private Vector2 _mousePos;

    private VisualElement _root;
    private VisualElement _itemLayer;
    private float _itemScale = 1;

    private List<Slot> _slotList;

    private static GhostIcon _ghostIcon;
    private static Accessioning _accBox;

    private static bool _isOverBox;
    private static bool _isDragging;
    private static Item _draggedItem;

    // Properties

    public int Width = 6;
    public int Height = 6;

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

    private void OnEnable()
    {
        _root = GetComponent<UIDocument>().rootVisualElement;
        _itemLayer = _root.Q("ItemLayer");
        _ghostIcon = _root.Q<GhostIcon>();

        _slotList = _root.Query<Slot>().ToList();
    }

    private void OnDisable()
    {
        
    }

    public Vector2 ItemTileSize
    {
        get { return SlotSize * _itemScale; }
    }

    private void Awake()
    {
    }

    void Start()
    {
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

            _itemLayer.style.width = slotLayer.resolvedStyle.width;
            _itemLayer.style.height = slotLayer.resolvedStyle.height;

            _itemLayer.style.left = slotLayer.resolvedStyle.left;
            _itemLayer.style.top = slotLayer.resolvedStyle.top;

            // Adding items to inventory if already had some

            Debug.Log($"There should be {GameManager.Instance.StoredItems.Count} items in inv");
            if (GameManager.Instance.StoredItems.Count > 0)
            {
                foreach (Item item in GameManager.Instance.StoredItems)
                {
                    item.PlaceInSlot(this, _itemLayer, GetSlot(item.RootGridIndex.x, item.RootGridIndex.y));
                }
            }
        });
        
        _root.RegisterCallback<PointerMoveEvent>(OnPointerMove);
        _root.RegisterCallback<PointerUpEvent>(OnPointerUp);

        _accBox = AccessioningController.Instance.GetBox();

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
        _mousePos = pos;
        _isDragging = true;
        _draggedItem = item;
        _accBox.pickingMode = PickingMode.Position;

        if (_draggedItem.CurrentState == ItemState.InInventory)
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

        _ghostIcon.SetToMousePosition(_draggedItem.Pivot, _mousePos);
        _ghostIcon.SetVisual(_draggedItem);   
        
    }

    /// <summary>
    /// Handles dragging across the screen
    /// </summary>
    public void OnPointerMove(PointerMoveEvent evt)
    {
        if (!_isDragging) return;

        _mousePos = evt.position;
        _ghostIcon.SetToMousePosition(_draggedItem.Pivot, _mousePos);

        Rect r = _accBox.worldBound;
        _isOverBox = r.Contains(_mousePos);

        if (_isOverBox)
        {
            _accBox.AddToClassList("accessioning-box--active");
        }
        else
        {
            _accBox.RemoveFromClassList("accessioning-box--active");
        }
        
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

        Slot hoveredSlot = null;

        foreach (Slot s in _slotList) 
        {
            Rect r = s.worldBound;
            if (r.Contains(_mousePos))
            {
                hoveredSlot = s;
            }
        }

        if (_isOverBox)
        {
            _draggedItem.SetState(ItemState.InAccessioning);
            _draggedItem.PlaceInBox(_accBox, _mousePos);
            _accBox.RemoveFromClassList("accessioning-box--active");

            GameManager.Instance.StoredItems.Remove(_draggedItem);
        }
        else if (CanPlace(hoveredSlot))
        {
            _draggedItem.SetState(ItemState.InInventory);
            _draggedItem.PlaceInSlot(this, _itemLayer, hoveredSlot);

            if (!GameManager.Instance.StoredItems.Contains(_draggedItem))
            {
                GameManager.Instance.StoredItems.Add(_draggedItem);
            }
        }
        else // Couldn't place
        {
            _draggedItem.RevertRotation();

            if (_draggedItem.CurrentState == ItemState.InInventory)
            {
                _draggedItem.PlaceInSlot(this, _itemLayer, _draggedItem.Pivot.GridSlot);
            }
        }

        _accBox.pickingMode = PickingMode.Ignore;
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
        _ghostIcon.SetToMousePosition(_draggedItem.Pivot, _mousePos);
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
        List<Item> items = _itemLayer.Children().OfType<Item>()
            .OrderBy(x => x.RootGridIndex.x)
            .OrderBy(y => y.RootGridIndex.y).ToList();

        // Avoids z-indexing issues (being unable to click on certain items)
        for (int i = 0; i < items.Count; i++)
        {
            items[i].BringToFront();
        }

        foreach (Item item in items)
        {
            _itemLayer.Add(item);
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
