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
        pickingMode = PickingMode.Ignore;

        // Make sure styling is applied
        if (!ClassListContains("ghost-icon"))
        {
            AddToClassList("ghost-icon");
        }

        DebugLabel = new Label("");
        DebugLabel.pickingMode = PickingMode.Ignore;
        DebugLabel.AddToClassList("debug-text");
        Add(DebugLabel);

        _transitionDuration = BASE_TRANSITION_DURATION;
    }

    /// <summary>
    /// Sets the ghost icon's visual to mirror an item
    /// </summary>
    /// <param name="item">The item to set the icon to</param>
    public void SetVisual(Item item)
    {
        MatchItemStyle(item);
        UpdatePosition(item.Pivot);

        //_transitionDuration *= item.SO.Weight;

        style.visibility = Visibility.Visible;
        style.transitionDuration = new List<TimeValue> { new TimeValue(_transitionDuration, TimeUnit.Second) };
    }

    /// <summary>
    /// Removes any icon that is currently being displayed while dragging
    /// </summary>
    public void ResetVisual()
    {
        style.transitionDuration = new List<TimeValue> { new TimeValue(0, TimeUnit.Second) };

        _transitionDuration = BASE_TRANSITION_DURATION;

        style.visibility = Visibility.Hidden;
        DebugLabel.style.visibility = Visibility.Hidden;
    }

    /// <summary>
    /// Moves the ghost icon to the given mouse position
    /// </summary>
    /// <param name="pivot">Which tile is currently being dragged</param>
    public void UpdatePosition(ItemTile pivot)
    {
        if (pivot != null)
        {
            SetToMousePosition(pivot);

            // Space to add some cool effects or something idk
        }
    }

    /// <summary>
    /// Rotates the visual of the ghost icon
    /// </summary>
    /// <param name="dir">The direction to rotate</param>
    /// <param name="pivot">Where to rotate from</param>
    public void Rotate(int dir, ItemTile pivot)
    {
        Rotate rot = new Rotate(new Angle(style.rotate.value.angle.value + 90 * dir, AngleUnit.Degree));
        style.rotate = rot;

        UpdatePosition(pivot); // Maintain mouse position on center of pivot

        // Keep debug label orientation
        DebugLabel.style.rotate = new Rotate(new Angle(360.0f - Mathf.Abs(style.rotate.value.angle.value), AngleUnit.Degree));
    }

    /// <summary>
    /// Updates the style of the ghost icon to match the currently dragged item's
    /// </summary>
    /// <param name="item">The item being dragged</param>
    private void MatchItemStyle(Item item)
    {
        style.width = item.resolvedStyle.width;
        style.height = item.resolvedStyle.height;
        style.backgroundImage = item.SO.Sprite.texture;

        float angle = item.style.rotate.value.angle.value;
        Rotate rot = new Rotate(new Angle(angle, AngleUnit.Degree));
        style.rotate = rot;
    }

    /// <summary>
    /// Centers the mouse on the given tile
    /// </summary>
    private void SetToMousePosition(ItemTile pivot)
    {
        DebugLabel.text = $"Pivot: ({pivot.Index.x}, {pivot.Index.y})";

        Vector2 mouseScreen = Mouse.current.position.ReadValue();
        Vector2 mousePanel = UIHelpers.WorldToLocalUIPosition(panel, mouseScreen);

        float tileWidth = InventoryController.Instance.ItemTileSize.x;
        float tileHeight = InventoryController.Instance.ItemTileSize.y;

        // Find offset from pivot
        float rowOffset = tileHeight * pivot.Index.x;
        float colOffset = tileWidth * pivot.Index.y;

        float drawOffset = 0;
        Item item = pivot.ParentItem;
        if (pivot.ParentItem.Rotation % 180 != 0)
        {
            drawOffset = (item.WidthInTiles - item.HeightInTiles) * InventoryController.Instance.ItemTileSize.x / 2;
        }

        style.left = (mousePanel.x - colOffset - tileWidth * 0.5f) + drawOffset;
        style.top = (mousePanel.y - rowOffset - tileHeight * 0.5f) - drawOffset;
    }
}
