using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Linq;
using System;
using Random = UnityEngine.Random;
using Unity.VisualScripting;
using static UnityEngine.Rendering.DebugUI.Table;

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
        _ghostIcon.style.left = pos.x - _ghostIcon.resolvedStyle.width / 2;
        _ghostIcon.style.top = pos.y - _ghostIcon.resolvedStyle.height / 2;
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

        _draggedItem.style.visibility = Visibility.Hidden;

        // Copy the item's properties
        _ghostIcon.style.backgroundImage = item.BaseSprite.texture;
        _ghostIcon.style.width = item.resolvedStyle.width;
        _ghostIcon.style.height = item.resolvedStyle.height;
        
        _ghostIcon.style.visibility = Visibility.Visible;

        _ghostIcon.schedule.Execute(() =>
        {
            SetGhostIconPosition(pos);
        });
        
    }

    /// <summary>
    /// Handles dragging across the screen
    /// </summary>
    void OnPointerMove(PointerMoveEvent evt)
    {
        if (!_isDragging) return;

        SetGhostIconPosition(evt.position);
    }

    /// <summary>
    /// Handles dropping items on the screen
    /// </summary>
    void OnPointerUp(PointerUpEvent evt)
    {
        if (!_isDragging) return;

        OnDrop?.Invoke(_draggedItem);

        _isDragging = false;

        Vector2 mousePos = evt.position;

        Slot hoveredSlot = GetSlotUnderMouse(mousePos);

        if (hoveredSlot != null && CanPlace(hoveredSlot))
        {
            PlaceItem(_draggedItem, hoveredSlot);
        }

        /*Vector2 localPos = _draggedItem.parent.WorldToLocal(evt.position);

        // Place item at cursor
        _draggedItem.style.left = localPos.x - _draggedItem.resolvedStyle.width/2;
        _draggedItem.style.top = localPos.y - _draggedItem.resolvedStyle.height/2;*/
        
        _draggedItem.style.visibility = Visibility.Visible;
        _ghostIcon.style.visibility = Visibility.Hidden;
    } 

    /// <summary>
    /// Finds a slot, if any, that the user is hovering over
    /// </summary>
    /// <param name="pos">Position of the mouse</param>
    /// <returns>The slot that the user is hovering over</returns>
    Slot GetSlotUnderMouse(Vector2 pos)
    {
        foreach (Slot slot in Slots)
        {
            /*Rect r = slot.worldBound;
            if (r.Contains(pos))
            {
                return slot;
            }*/
        }

        return null;
    }

    /// <summary>
    /// Determines whether the slot the user places the item is valid
    /// </summary>
    /// <param name="slot">The slot the user places the item in</param>
    /// <returns>Whether the item can be placed in the given slot</returns>
    bool CanPlace(Slot slot)
    {
        int i = Slots.IndexOf(slot);

        int row = i / InventorySize[1];
        int col = i % InventorySize[1];

        for (int y = 0; y < _draggedItem.Dimensions.y; y++)
        {
            for (int x = 0; x < _draggedItem.Dimensions.x; x++)
            {
                int checkIndex = (i + y) * InventorySize[1] + (col + x);

                if (checkIndex < 0 || checkIndex >= Slots.Count)
                {
                    return false;
                }

                if (!Slots[checkIndex].IsFree)
                {
                    return false;
                }
            }
        }

        return true;
    }

    /// <summary>
    /// Places the dragged item into a slot
    /// </summary>
    /// <param name="slot">The top-left slot that the user places the item in</param>
    void PlaceItem(Slot slot)
    {
        int i = Slots.IndexOf(slot);
        int row = i / InventorySize[1];
        int col = i % InventorySize[1];

        for (int y = 0; y < _draggedItem.Dimensions.y; y++)
        {
            for (int x = 0; x < _draggedItem.Dimensions.x; x++)
            {
                int index = (row + y) * InventorySize[1] + (col + x);

                Slots[index].ItemRef = _draggedItem;
            }
        }

        slot.SetItem(_draggedItem);
    }

    /*void ReturnItem(Item item)
    {
        _ghostIcon.style.visibility = Visibility.Hidden;
        item.style.visibility = Visibility.Visible;
    }*/
}
