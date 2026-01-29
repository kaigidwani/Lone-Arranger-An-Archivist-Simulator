using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class Accessioning : VisualElement
{
    // Fields
    
    private Vector2 _paddingLeftRight;
    private Vector2 _paddingTopBottom;
    private Vector2 _boxSize;

    // Properties

    public Vector2 Bounds
    {
        get
        {
            float x = _paddingLeftRight.x + _boxSize.x;
            float y = _paddingTopBottom.x + _boxSize.y;
            return new Vector2(x, y);
        }
    }

    public Vector2 Position
    {
        get { return new Vector2(_paddingLeftRight.x, _paddingTopBottom.x); }
    }

    public Accessioning()
    {
        RegisterCallback<GeometryChangedEvent>(evt =>
        {
            _paddingLeftRight = new Vector2(resolvedStyle.paddingLeft, resolvedStyle.paddingRight);
            _paddingTopBottom = new Vector2(resolvedStyle.paddingTop, resolvedStyle.paddingBottom);

            _boxSize = new Vector2(
                resolvedStyle.width - _paddingLeftRight.x - _paddingLeftRight.y,
                resolvedStyle.height - _paddingTopBottom.x - _paddingTopBottom.y);
        });
    }

    /// <summary>
    /// Adds an item to a randomly generated position within the accessioning box on click
    /// </summary>
    /// <param name="e">Click Event</param>
    public void SpawnItems(int count = 5)
    {
        Debug.Log("adding items");

        for (int i = 0; i < count; i++)
        {
            ItemInfo type = null;
            if (InventoryController.Instance.ItemPool.Length > 0)
            {
                type = InventoryController.Instance.ItemPool[Random.Range(0, InventoryController.Instance.ItemPool.Length - 1)];
                Debug.Log(type.Name);
            }

            Item item = new Item();
            item.Spawn(this, type);

            InventoryController.Instance.Items.Add(item);
            Add(item);
        }

        Debug.Log("box full");
    }
}
