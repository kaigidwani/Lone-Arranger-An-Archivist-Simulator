using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Linq;

public class InventoryController : MonoBehaviour
{
    public static InventoryController Instance;

    public List<VisualElement> Slots;
    public ItemInfo[] ItemPool;

    private VisualElement root;
    [SerializeField] private List<Button> debugButtons;
    

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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        debugButtons = root.Q("Debug").Query<Button>().ToList();
        Slots = root.Q("Inventory").Query("Slot").ToList();
        Debug.Log(debugButtons.Count);

        debugButtons[0].RegisterCallback<ClickEvent>(SpawnItem);

    }

    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Purely Debug Functions

    void SpawnItem(ClickEvent e)
    {
        Debug.Log("button clicked!");
        Debug.Log("slot count: " + Slots.Count);
        for (int i = 0; i < Slots.Count; i++)
        {
            if (Slots[i].childCount == 0)
            {
                VisualElement child = new VisualElement();
                child.AddToClassList("item");

                CheckSlotColor(Slots[i], child, "red");
                CheckSlotColor(Slots[i], child, "blue");
                CheckSlotColor(Slots[i], child, "green");

                Slots[i].Add(child);
                return;
            }
        }

        Debug.Log("inventory full");
    }

    void CheckSlotColor(VisualElement slot, VisualElement item, string color)
    {
        if (slot.ClassListContains($"inventory-slot-{color}"))
        {
            item.AddToClassList($"item-{color}");
        }
    }
}
