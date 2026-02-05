using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class Accessioning : VisualElement
{
    // Fields
    
    private float _paddingLeft;
    private float _paddingRight;
    private float _paddingTop;
    private float _paddingBottom;
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
            return new Vector2(_paddingLeft, _paddingTop);
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
                Min.x + _boxSize.x,
                Min.y + _boxSize.y);
        }
    }

    public Accessioning()
    {
        if (_paddingLeft == 0)
        {
            _hasLoaded = false;
        }
        
        RegisterCallback<GeometryChangedEvent>(evt =>
        {
            _paddingLeft = resolvedStyle.paddingLeft;
            _paddingRight = resolvedStyle.paddingLeft;
            _paddingTop = resolvedStyle.paddingLeft;
            _paddingBottom = resolvedStyle.paddingLeft;

            _boxSize = new Vector2(
                resolvedStyle.width - resolvedStyle.borderLeftWidth - resolvedStyle.borderRightWidth - _paddingLeft - _paddingRight,
                resolvedStyle.height - resolvedStyle.borderTopWidth - resolvedStyle.borderBottomWidth - _paddingTop - _paddingBottom);

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
    public void SpawnItems(int count = 10)
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
