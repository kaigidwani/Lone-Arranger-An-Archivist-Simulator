using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Linq;
using System;
using Random = UnityEngine.Random;
using Unity.VisualScripting;
using static UnityEngine.Rendering.DebugUI.Table;
using static UnityEditor.Progress;
using UnityEditor.UIElements;

public class InventoryController : MonoBehaviour
{
    // Fields

    [SerializeField] private List<Button> _debugButtons;
    private VisualElement _root;
    private static VisualElement _ghostIcon;

    private static bool _isDragging;
    private static Item _draggedItem;

    // Properties

    public static InventoryController Instance;

    public int[] InventorySize;
    public List<Slot> Slots;
    public Vector2 SlotSize
    {
        get {
            return new Vector2(
                Slots[0].resolvedStyle.width,
                Slots[0].resolvedStyle.height

            );
        }
    }

    public List<Item> Items;
    public ItemInfo[] ItemPool;

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
        InventorySize = new int[2] { 6, 6 };

        _root = GetComponent<UIDocument>().rootVisualElement;
        _root.RegisterCallback<PointerMoveEvent>(OnPointerMove);
        _root.RegisterCallback<PointerUpEvent>(OnPointerUp);

        Items = new List<Item>();
        Slots = _root.Q("Inventory").Query<Slot>().ToList();
        
        _ghostIcon = _root.Q("GhostIcon");
    }

    /// <summary>
    /// Centers the ghost icon on the given position
    /// </summary>
    /// <param name="pos">The position to draw the icon</param>
    static void SetGhostIconPosition(Vector2 pos)
    {
        //Debug.Log("item size" + new Vector2(_draggedItem.ItemSize.x, _draggedItem.ItemSize.y));
        _ghostIcon.style.left = pos.x - _draggedItem.resolvedStyle.width / 2;
        _ghostIcon.style.top = pos.y - _draggedItem.resolvedStyle.height / 2;
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
            _draggedItem.CurrentSlot.ClearItems();
        }

        _draggedItem.style.visibility = Visibility.Hidden;
        _ghostIcon.style.visibility = Visibility.Visible;
 
        RemoveItemColor(_ghostIcon);

        // Copy the item's properties
        _ghostIcon.style.backgroundImage = item.BaseSprite.texture;
        _ghostIcon.style.width = item.resolvedStyle.width;
        _ghostIcon.style.height = item.resolvedStyle.height;
        _ghostIcon.style.top = item.resolvedStyle.top;
        _ghostIcon.style.left = item.resolvedStyle.left;

        foreach (VisualElement tile in item.Children())
        {
            Rect r = tile.worldBound;
            if (r.Contains(pos))
            {
                tile.AddToClassList("pivot");
                _draggedItem.Pivot = tile;
                
            }
        }

        Debug.Log(_draggedItem.Pivot.resolvedStyle.top);
        SetGhostIconPosition(pos - _draggedItem.Pivot.worldBound.position);
    }

    /// <summary>
    /// Handles dragging across the screen
    /// </summary>
    void OnPointerMove(PointerMoveEvent evt)
    {
        if (!_isDragging) return;

        //Debug.Log(_draggedItem.Pivot);
        SetGhostIconPosition((Vector2)evt.position - _draggedItem.Pivot.worldBound.position);
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
        _ghostIcon.style.visibility = Visibility.Hidden;

        OnDrop?.Invoke(_draggedItem);
    
        // Find slot under mouse
        Vector2 mousePos = evt.position;

        Slot hoveredSlot = GetHoveredVisualElement(mousePos);
        if (hoveredSlot != null && CanPlace(hoveredSlot))
        {
            _draggedItem.Place(hoveredSlot);
        }
        else if (_draggedItem.CurrentSlot != null)
        {
            _draggedItem.Place(_draggedItem.CurrentSlot);
        }

        // Change ghost icon's color to match item's
        foreach (string className in _draggedItem.GetClasses())
        {
            if (className.StartsWith("item-"))
            {
                _ghostIcon.AddToClassList(className);
            }
        }
    } 

    /// <summary>
    /// Finds a slot, if any, that the user is hovering over
    /// </summary>
    /// <param name="pos">Position of the mouse</param>
    /// <returns>The slot that the user is hovering over</returns>
    private Slot GetHoveredVisualElement(Vector2 pos)
    {
        foreach (Slot slot in Slots)
        {
            Rect r = slot.worldBound;
            if (r.Contains(pos))
            {
                return slot;
            }
        }

        return null;
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

    //private bool 

    public void RemoveItemColor(VisualElement elem)
    {
        string prevColor = "";
        foreach (string className in elem.GetClasses())
        {
            if (className.StartsWith("item-"))
            {
                prevColor = className;
            }
        }

        elem.RemoveFromClassList(prevColor);
    }
}
