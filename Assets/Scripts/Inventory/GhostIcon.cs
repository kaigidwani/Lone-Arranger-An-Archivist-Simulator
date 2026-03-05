using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

[UxmlElement]
public partial class GhostIcon : VisualElement
{
    // Fields
    private static float BASE_TRANSITION_DURATION = 0.2f;
    private float _transitionDuration;

    // Properties
    public Label DebugLabel;

    public GhostIcon()
    {
        // Make sure styling is applied
        if (!ClassListContains("ghost-icon"))
        {
            AddToClassList("ghost-icon");
        }

        DebugLabel = new Label("");
        DebugLabel.AddToClassList("debug-text");
        Add(DebugLabel);

        _transitionDuration = BASE_TRANSITION_DURATION;
    }

    /// <summary>
    /// Sets the ghost icon's visual to mirror an item
    /// </summary>
    /// <param name="item">The item to set the icon to</param>
    public void SetItem(Item item)
    {
        MatchItemStyle(item);
        MatchItemRotation(item);

        _transitionDuration *= item.SO.Weight;
    }

    public void MatchItemStyle(Item item)
    {
        style.width = item.resolvedStyle.width;
        style.height = item.resolvedStyle.height;
        style.backgroundImage = item.SO.Sprite.texture;
    }

    public void MatchItemRotation(Item item)
    {
        if (Mathf.Abs(item.style.rotate.value.angle.value - style.rotate.value.angle.value) > 90)
        {
            style.transitionDuration = new List<TimeValue> { new TimeValue(0, TimeUnit.Second) };
        }

        style.rotate = item.style.rotate;

        // Keep debug label orientation
        DebugLabel.style.rotate = new Rotate(new Angle(360.0f - style.rotate.value.angle.value, AngleUnit.Degree));

        SetToMousePosition(item.Pivot);
    }

    /// <summary>
    /// Removes any icon that is currently being displayed while dragging
    /// </summary>
    public void ResetIcon()
    {
        style.visibility = Visibility.Hidden;
        style.left = 0;
        style.top = 0;
        style.width = 0;
        style.height = 0;
        style.transitionDuration = new List<TimeValue> { new TimeValue(0, TimeUnit.Second) };

        DebugLabel.style.visibility = Visibility.Hidden;
        _transitionDuration = BASE_TRANSITION_DURATION;
    }

    /// <summary>
    /// Centers the mouse on the given tile
    /// </summary>
    public void SetToMousePosition(ItemTile origin)
    {
        if (origin == null)
        {
            return;
        }
        
        DebugLabel.text = $"Pivot: ({origin.Index.x}, {origin.Index.y})";

        Vector2 mouseScreen = Mouse.current.position.ReadValue();
        Vector2 mousePanel = UIHelpers.WorldToLocalUIPosition(panel, mouseScreen);

        float tileWidth = InventoryController.Instance.ItemTileSize.x;
        float tileHeight = InventoryController.Instance.ItemTileSize.y;

        // Find offset from pivot
        float rowOffset = tileHeight * origin.Index.x;
        float colOffset = tileWidth * origin.Index.y;

        float drawOffset = 0;
        PlaceableItemSO so = origin.ParentItem.SO;
        if (so.Rotation % 180 != 0)
        {
            drawOffset = (so.Width - so.Height) * InventoryController.Instance.ItemTileSize.x / 2;
        }

        style.left = (mousePanel.x - colOffset - tileWidth * 0.5f) + drawOffset;
        style.top = (mousePanel.y - rowOffset - tileHeight * 0.5f) - drawOffset;

        style.visibility = Visibility.Visible;
        
        style.transitionDuration = new List<TimeValue> { new TimeValue(_transitionDuration, TimeUnit.Second) };

        if (InventoryController.Instance.ShowDebug)
        {
            DebugLabel.style.visibility = Visibility.Visible;
        }
        
    }
}
