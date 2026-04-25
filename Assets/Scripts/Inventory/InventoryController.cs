using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI.Table;
using Cursor = UnityEngine.Cursor;


public class InventoryController : MonoBehaviour
{
    // Fields
    private Vector2 _mousePos;

    private VisualElement _root;
    private float _itemScale = 1;

    private static GhostIcon _ghostIcon;
    private static Accessioning _accBox;

    private static bool _isOverAccessioning;
    private static bool _isDragging;
    private static Item _draggedItem;

    // Properties

    public Grid Inventory;
    public Grid DonationBox;

    [HideInInspector] public bool ShowDebug;

    /// <summary>
    /// Final width and height of each individual slot
    /// </summary>
    public Vector2 SlotSize
    {
        get {
            
            return new Vector2(
                Inventory.GetSlot(0, 0).resolvedStyle.width,
                Inventory.GetSlot(0, 0).resolvedStyle.height

            );
        }
    }

    private void OnEnable()
    {
        _root = GetComponent<UIDocument>().rootVisualElement;
        _ghostIcon = _root.Q<GhostIcon>();
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
        _root.RegisterCallback<PointerMoveEvent>(OnPointerMove);
        _root.RegisterCallback<PointerUpEvent>(OnPointerUp);

        _accBox = AccessioningController.Instance.GetBox();

        ShowDebug = false;
        SetDebug();

        Inventory = _root.Q<Grid>("Inventory");
        DonationBox = _root.Q<Grid>("DonationBox");
        SetupInventories();
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

        if (_draggedItem.CurrentState == ItemState.InInventory || _draggedItem.CurrentState == ItemState.InDonationBox)
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
        _isOverAccessioning = r.Contains(_mousePos);

        if (_isOverAccessioning)
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

        if (_isOverAccessioning)
        {
            _draggedItem.SetState(ItemState.InAccessioning);
            _draggedItem.ReturnToAccessioning(_accBox, _mousePos);
            _accBox.RemoveFromClassList("accessioning-box--active");

            GameManager.Instance.StoredItems.Remove(_draggedItem);
        }

        Slot hoveredSlot = null;
        Grid hoveredGrid = null;

        foreach (Slot s in Inventory.SlotList)
        {
            Rect r = s.worldBound;
            if (r.Contains(_mousePos))
            {
                hoveredSlot = s;
                hoveredGrid = Inventory;
                break;
            }
        }

        if (hoveredSlot == null && DonationBox != null)
        {
            foreach (Slot s in DonationBox.SlotList)
            {
                Rect r = s.worldBound;
                if (r.Contains(_mousePos))
                {
                    hoveredSlot = s;
                    hoveredGrid = DonationBox;
                    break;
                }
            }
        }

        _draggedItem.style.visibility = Visibility.Visible;
        
        if (CanPlace(hoveredGrid, hoveredSlot))
        {
            if (hoveredGrid == Inventory)
            {
                _draggedItem.SetState(ItemState.InInventory);

                if (!GameManager.Instance.StoredItems.Contains(_draggedItem))
                {
                    GameManager.Instance.StoredItems.Add(_draggedItem);
                }
            }
            else
            {
                _draggedItem.SetState(ItemState.InDonationBox);
            }

            _draggedItem.PlaceInSlot(this, hoveredGrid.ItemLayer, hoveredSlot);
        }
        else // Couldn't place
        {
            _draggedItem.RevertRotation();

            if (_draggedItem.CurrentState == ItemState.InInventory)
            {
                _draggedItem.PlaceInSlot(this, Inventory.ItemLayer, _draggedItem.Pivot.GridSlot);
            }

            if (_draggedItem.CurrentState == ItemState.InDonationBox)
            {
                _draggedItem.PlaceInSlot(this, DonationBox.ItemLayer, _draggedItem.Pivot.GridSlot);
            }
        }

        Debug.Log(_draggedItem.CurrentState);

        _accBox.pickingMode = PickingMode.Ignore;
        _ghostIcon.ResetVisual();
        _draggedItem.ResetPivot();
        ReorderItems(Inventory);

        if (DonationBox != null)
        {
            ReorderItems(DonationBox);
        }
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
    private bool CanPlace(Grid grid, Slot startSlot)
    {
        if (startSlot == null)
        {
            return false;
        }

        foreach (ItemTile tile in _draggedItem.Tiles)
        {
            int gridRow = startSlot.GridIndex.x + (tile.Index.x - _draggedItem.Pivot.Index.x);
            int gridCol = startSlot.GridIndex.y + (tile.Index.y - _draggedItem.Pivot.Index.y);

            //Debug.Log($"tile supposedly at {gridRow}, {gridCol}");

            // Bounds logic
            if (gridRow < 0 || gridRow >= grid.Height ||
                gridCol < 0 || gridCol >= grid.Width)
            {
                return false;
            }

            // Occupancy logic
            if (!grid.GetSlot(gridRow, gridCol).IsFree)
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// All placed items are reordered by their grid position
    /// </summary>
    private void ReorderItems(Grid grid)
    {
        List<Item> items = grid.ItemLayer.Children().OfType<Item>()
            .OrderBy(x => x.RootGridIndex.x)
            .OrderBy(y => y.RootGridIndex.y).ToList();

        // Avoids z-indexing issues (being unable to click on certain items)
        for (int i = 0; i < items.Count; i++)
        {
            items[i].BringToFront();
        }

        foreach (Item item in items)
        {
            grid.ItemLayer.Add(item);
        }
    }

    private async void SetupInventories()
    {
        await UniTask.WaitUntil(Inventory.TryGetDimensions);
        Inventory.Setup(6, 6);

        if (DonationBox != null)
        {
            await UniTask.WaitUntil(DonationBox.TryGetDimensions);
            DonationBox.Setup(3, 5);
        }

        // Adding items to inventory if already had some
        if (GameManager.Instance.StoredItems.Count > 0)
        {
            foreach (Item item in GameManager.Instance.StoredItems)
            {
                item.PlaceInSlot(this, Inventory.ItemLayer, Inventory.GetSlot(item.RootGridIndex.x, item.RootGridIndex.y));
            }
        }
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
