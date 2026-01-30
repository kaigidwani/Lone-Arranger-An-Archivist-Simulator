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

    private bool _hasLoaded;

    // Properties

    /// <summary>
    /// Minimimum (top-left) position of this element
    /// </summary>
    public Vector2 Min
    {
        get 
        {
            return new Vector2(_paddingLeftRight.x, _paddingTopBottom.x);
        }
    }

    /// <summary>
    /// Maximum (top-right) position of this element
    /// </summary>
    public Vector2 Max
    {
        get
        {
            return new Vector2(
                _paddingLeftRight.x + _boxSize.x,
                _paddingTopBottom.x + _boxSize.y);
        }
    }

    public Accessioning()
    {
        _hasLoaded = false;

        RegisterCallback<GeometryChangedEvent>(evt =>
        {
            _paddingLeftRight = new Vector2(resolvedStyle.paddingLeft, resolvedStyle.paddingRight);
            _paddingTopBottom = new Vector2(resolvedStyle.paddingTop, resolvedStyle.paddingBottom);

            _boxSize = new Vector2(
                resolvedStyle.width - _paddingLeftRight.x - _paddingLeftRight.y,
                resolvedStyle.height - _paddingTopBottom.x - _paddingTopBottom.y);

            if (!_hasLoaded) // Guarantees that items spawn once after event fires
            {
                _hasLoaded = true;
                SpawnItems();
            }
        });
    }

    /// <summary>
    /// Adds a number of randomly generated items inside this box
    /// </summary>
    /// <param name="count">Number of items to spawn</param>
    public void SpawnItems(int count = 5)
    {
        Debug.Log("adding items");

        for (int i = 0; i < count; i++)
        {
            Item item = new Item();
            item.Spawn(this);

            InventoryController.Instance.Items.Add(item);
            Add(item);
        }

        Debug.Log("box full");
    }
}
