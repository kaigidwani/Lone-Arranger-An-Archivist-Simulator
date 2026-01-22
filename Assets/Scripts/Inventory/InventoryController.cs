using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Linq;

public class InventoryController : MonoBehaviour
{
    // Fields
    [SerializeField] private List<Button> _debugButtons;
    private VisualElement _root;
    private VisualElement _accessioningBox;

    // Properties

    public static InventoryController Instance;
    public List<VisualElement> Slots;
    public ItemInfo[] ItemPool;

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
        _root = GetComponent<UIDocument>().rootVisualElement;
        _debugButtons = _root.Q("Debug").Query<Button>().ToList();
        _accessioningBox = _root.Q("Accessioning");
        Slots = _root.Q("Inventory").Query("Slot").ToList();
        Debug.Log(_debugButtons.Count);

        _debugButtons[0].RegisterCallback<ClickEvent>(SpawnItem);

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
    void CheckSlotColor(VisualElement slot, VisualElement item, string color)
    {
        if (slot.ClassListContains($"inventory-slot-{color}"))
        {
            item.AddToClassList($"item-{color}");
        }
    }

    // Purely Debug Functions

    void SpawnItem(ClickEvent e)
    {
        Debug.Log("button clicked!");
        Debug.Log("slot count: " + Slots.Count);

        Vector2 boxSize = new Vector2(_accessioningBox.resolvedStyle.width - _accessioningBox.resolvedStyle.paddingLeft*2,
            _accessioningBox.resolvedStyle.height - _accessioningBox.resolvedStyle.paddingTop*2);

        Translate randomPos = new Translate(0 + Random.Range(0, boxSize.x), 0 + Random.Range(0, boxSize.y));

        VisualElement child = new VisualElement();
        child.AddToClassList("item");

        child.style.translate = new StyleTranslate(randomPos);
        _accessioningBox.Add(child);

        /*for (int i = 0; i < Slots.Count; i++)
        {
            if (Slots[i].childCount == 0)
            {
               

                CheckSlotColor(Slots[i], child, "red");
                CheckSlotColor(Slots[i], child, "blue");
                CheckSlotColor(Slots[i], child, "green");

                Slots[i].Add(child);
                return;
            }
        }*/



        Debug.Log("inventory full");
    }
}
