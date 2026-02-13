using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.Progress;


public class InventoryController : MonoBehaviour
{
    // Fields

    [SerializeField] private List<Button> _debugButtons;
    private VisualElement _root;
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
        List<Slot> slotList = _root.Query<Slot>().ToList();
        Grid = new Slot[Height][];
        foreach (Slot slot in slotList)
        {
            if (Grid[row] == null)
            {
                Grid[row] = new Slot[Width];
            }

            Debug.Log($"row: {row}, col: {col}");
            Grid[row][col] = slot;
            
            col++;
            if (col == Width)
            {
                row++;
                col = 0;
            }
        }

        _ghostIcon = _root.Q<GhostIcon>();
        Debug.Log(_ghostIcon);
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

        _draggedItem.style.visibility = Visibility.Hidden;

        foreach (VisualElement tile in _draggedItem.Children())
        {
            Rect r = tile.worldBound;
            if (r.Contains(pos))
            {
                tile.AddToClassList("pivot");
                _draggedItem.Pivot = tile;

            }
        }

        Vector2 itemPivotCenter = _draggedItem.Pivot.worldBound.center - _draggedItem.worldBound.position;

        _ghostIcon.SetIcon(item);
        _ghostIcon.SetPosition(pos - itemPivotCenter);
    }

    /// <summary>
    /// Handles dragging across the screen
    /// </summary>
    void OnPointerMove(PointerMoveEvent evt)
    {
        if (!_isDragging) return;

        //Debug.Log(_draggedItem.Pivot);
        _ghostIcon.SetPosition(evt.position);
    }

    /// <summary>
    /// Handles dropping items on the screen
    /// </summary>
    void OnPointerUp(PointerUpEvent evt)
    {
        if (!_isDragging) return;

        _isDragging = false;

        if (_draggedItem.Pivot != null)
        {
            _draggedItem.RemoveFromClassList("pivot");
            _draggedItem.Pivot = null;
        }
        
        _draggedItem.style.visibility = Visibility.Visible;

        OnDrop?.Invoke(_draggedItem);
    
        // Find slot under mouse
        Vector2 mousePos = evt.position;

        Slot hoveredSlot = _root.Q<Slot>("hover");
        if (hoveredSlot != null && CanPlace(hoveredSlot))
        {
            _draggedItem.Place(hoveredSlot);
        }
        else if (_draggedItem.CurrentSlot != null)
        {
            _draggedItem.Place(_draggedItem.CurrentSlot);
        }

        _ghostIcon.RefreshVisual();
    } 

    /// <summary>
    /// Determines whether the slot the user places the item is valid
    /// </summary>
    /// <param name="slot">The slot the user places the item in</param>
    /// <returns>Whether the item can be placed in the given slot</returns>
    private bool CanPlace(Slot slot)
    {
        return slot.IsFree;
    }

    
}
