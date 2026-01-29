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
    private Accessioning _accessioning;

    // Properties
    public static InventoryController Instance;
    public List<Slot> Slots;
    public List<Item> Items;
    public ItemInfo[] ItemPool;

    static bool _isDragging;
    static Item _draggedItem;

    public event Action<Item> OnDrop;

    public int[] InventorySize;

    private void Awake()
    {
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
        _accessioning = _root.Q<Accessioning>();

        Slots = new List<Slot>();
        Items = new List<Item>();

        Slots = _root.Q("Inventory").Query<Slot>().ToList();

        _ghostIcon = _root.Q("GhostIcon");
        _ghostIcon.RegisterCallback<PointerMoveEvent>(OnPointerMove);
        _ghostIcon.RegisterCallback<PointerUpEvent>(OnPointerUp);

        Debug.Log(_ghostIcon);

        _accessioning.SpawnItems();

    }

    void CheckSlotColor(VisualElement slot, VisualElement item, string color)
    {
        if (slot.ClassListContains($"inventory-slot-{color}"))
        {
            item.AddToClassList($"item-{color}");
        }
    }

    static void SetGhostIconPosition(Vector2 pos)
    {
        _ghostIcon.style.top = pos.y - _ghostIcon.layout.height / 2;
        _ghostIcon.style.left = pos.x - _ghostIcon.layout.height / 2;
    }

    public void OnPointerDown(Vector2 pos, Item item)
    {
        _isDragging = true;
        _draggedItem = item;

        _draggedItem.style.visibility = Visibility.Hidden;

        _ghostIcon.style.backgroundImage = item.BaseSprite.texture;
        _ghostIcon.style.width = item.resolvedStyle.width;
        _ghostIcon.style.height = item.resolvedStyle.height;
        
        _ghostIcon.style.visibility = Visibility.Visible;
        SetGhostIconPosition(pos);
    }

    void OnPointerMove(PointerMoveEvent evt)
    {
        if (!_isDragging) return;

        Debug.Log("moving");
        SetGhostIconPosition(evt.position);
    }

    void OnPointerUp(PointerUpEvent evt)
    {
        if (!_isDragging) return;

        OnDrop?.Invoke(_draggedItem);

        _isDragging = false;

        Vector2 mousePos = evt.position;

        Slot hoveredSlot = GetSlotUnderMouse(mousePos);

        if (hoveredSlot != null && CanPlaceItem(hoveredSlot, _draggedItem))
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
    /// 
    /// </summary>
    /// <param name="pos">Position of the mouse</param>
    /// <returns></returns>
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
    /// 
    /// </summary>
    /// <param name="slot"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    bool CanPlaceItem(Slot slot, Item item)
    {
        int i = Slots.IndexOf(slot);

        int row = i / InventorySize[1];
        int col = i % InventorySize[1];

        for (int y = 0; y < item.Dimensions.y; y++)
        {
            for (int x = 0; x < item.Dimensions.x; x++)
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
    /// 
    /// </summary>
    /// <param name="item"></param>
    /// <param name="slot">The top-left slot that the user places the item in</param>
    void PlaceItem(Item item, Slot slot)
    {
        int i = Slots.IndexOf(slot);
        int row = i / InventorySize[1];
        int col = i % InventorySize[1];

        for (int y = 0; y < item.Dimensions.y; y++)
        {
            for (int x = 0; x < item.Dimensions.x; x++)
            {
                int index = (row + y) * InventorySize[1] + (col + x);

                Slots[index].ItemRef = item;
            }
        }

        slot.SetItem(item);
    }

    /*void ReturnItem(Item item)
    {
        _ghostIcon.style.visibility = Visibility.Hidden;
        item.style.visibility = Visibility.Visible;
    }*/
}
