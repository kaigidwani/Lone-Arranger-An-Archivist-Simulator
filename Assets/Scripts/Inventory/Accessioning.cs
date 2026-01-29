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

        //SpawnItems();

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
            }

            Item item = new Item();

            item.AddToClassList("item");

            item.schedule.Execute(() =>
            {

                float x = Random.Range(_paddingLeftRight.x, _boxSize.x - item.resolvedStyle.width);
                float y = Random.Range(_paddingTopBottom.x, _boxSize.y - item.resolvedStyle.height);

                // USS properties
                item.style.left = x;
                item.style.top = y;
                item.style.opacity = 100;
            });

            InventoryController.Instance.Items.Add(item);
            Add(item);
            item.OnStartDrag += InventoryController.Instance.OnPointerDown;
        }

        Debug.Log("box full");
    }
}
