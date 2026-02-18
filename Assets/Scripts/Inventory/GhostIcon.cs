using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using static UnityEditor.Progress;
using static UnityEditor.Rendering.FilterWindow;
using static UnityEngine.GraphicsBuffer;

[UxmlElement]
public partial class GhostIcon : VisualElement
{
    private VisualElement _icon;
    private Item _draggedItem;

    public GhostIcon()
    {
        AddToClassList("ghost-icon");
    }

    public void ResetVisual()
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

    public void SetIcon(Item item)
    {
        _draggedItem = item;
        style.width = item.resolvedStyle.width;
        style.height = item.resolvedStyle.height;

        _icon = new VisualElement();
        _icon.AddToClassList("ghost-icon-visual");
        _icon.style.width = item.resolvedStyle.width;
        _icon.style.height = item.resolvedStyle.height;
        _icon.style.backgroundImage = item.BaseSprite.texture;
        
        // Change ghost icon's color to match item's
        foreach (string className in item.GetClasses())
        {
            if (className.StartsWith("item-"))
            {
                _icon.AddToClassList(className);
            }
        }

        Add(_icon);
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

        float pivotOffsetX = tileWidth * _draggedItem.Pivot.Index.x;
        float pivotOffsetY = tileHeight * _draggedItem.Pivot.Index.y;
        Debug.Log(_draggedItem.Pivot.Index);

        style.left = mousePanel.x - pivotOffsetX - tileWidth * 0.5f;
        style.top = mousePanel.y - pivotOffsetY - tileHeight * 0.5f;

    }
}
