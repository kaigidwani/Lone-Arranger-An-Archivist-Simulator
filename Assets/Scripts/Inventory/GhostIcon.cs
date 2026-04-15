using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class GhostIcon : VisualElement
{
    // Fields
    private static float BASE_TRANSITION_DURATION = 0.2f;
    private float _transitionDuration;

    // Properties
    public Label DebugLabel;

    public Vector2 Position
    {
        get { return new Vector2(worldBound.x, worldBound.y); }
    }

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
    /// Centers the mouse on the given tile
    /// </summary>
    public void SetToMousePosition(ItemTile pivot, Vector2 mousePos)
    {
        if (pivot == null)
        {
            return;
        }

        DebugLabel.text = $"Pivot: ({pivot.Index.x}, {pivot.Index.y})";

        Vector2 newPos = UIHelpers.SetItemPivotToMouse(pivot, mousePos);
        style.left = newPos.x;
        style.top = newPos.y;
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

    
}
