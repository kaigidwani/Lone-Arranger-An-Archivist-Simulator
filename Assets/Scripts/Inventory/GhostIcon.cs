using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

[UxmlElement]
public partial class GhostIcon : VisualElement
{
    // Fields

    private VisualElement _icon;
    private Item _draggedItem;

    public GhostIcon()
    {
        // Make sure styling is applied
        if (!ClassListContains("ghost-icon"))
        {
            AddToClassList("ghost-icon");
        }
    }

    /// <summary>
    /// Sets the ghost icon's visual to mirror an item
    /// </summary>
    /// <param name="item">The item to set the icon to</param>
    public void SetIcon(Item item)
    {
        _draggedItem = item;
        style.width = item.resolvedStyle.width;
        style.height = item.resolvedStyle.height;

        _icon = new VisualElement();
        _icon.AddToClassList("ghost-icon-visual");
        _icon.style.width = item.resolvedStyle.width;
        _icon.style.height = item.resolvedStyle.height;
        _icon.style.backgroundImage = item.Type.Sprite.texture;

        Add(_icon);
    }

    /// <summary>
    /// Removes any icon that is currently being displayed while dragging
    /// </summary>
    public void ResetIcon()
    {
        if (_icon != null)
        {
            _icon.RemoveFromHierarchy();
            _icon = null;
        }

        if (_draggedItem != null)
        {
            _draggedItem = null;
        }

        style.left = 0;
        style.top = 0;
        style.width = 0;
        style.height = 0;
    }

    /// <summary>
    /// Centers the ghost icon on the given position
    /// </summary>
    public void SetToMousePosition()
    {
        if (_draggedItem == null || _draggedItem.Pivot == null)
        {
            return;
        }

        Vector2 mouseScreen = Mouse.current.position.ReadValue();
        Vector2 mousePanel = UIHelpers.WorldToLocalUIPosition(panel, mouseScreen);

        float tileWidth = _draggedItem.Pivot.resolvedStyle.width;
        float tileHeight = _draggedItem.Pivot.resolvedStyle.height;

        // Find offset from pivot
        float rowOffset = tileHeight * _draggedItem.Pivot.Index.x;
        float colOffset = tileWidth * _draggedItem.Pivot.Index.y;   

        style.left = mousePanel.x - colOffset - tileWidth * 0.5f;
        style.top = mousePanel.y - rowOffset - tileHeight * 0.5f;

    }
}
