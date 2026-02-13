using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.Progress;

[UxmlElement]
public partial class GhostIcon : VisualElement
{
    private VisualElement _icon;

    public GhostIcon()
    {
        AddToClassList("ghost-icon");
    }

    public void RefreshVisual()
    {
        if (_icon != null)
        {
            _icon.RemoveFromHierarchy();
            _icon = null;
        }
    }

    public void SetIcon(Item item)
    {
        style.width = item.resolvedStyle.width;
        style.height = item.resolvedStyle.height;
        style.top = item.resolvedStyle.top;
        style.left = item.resolvedStyle.left;

        _icon = new VisualElement();
        _icon.AddToClassList("ghost-icon-visual");
        _icon.style.width = item.resolvedStyle.width;
        _icon.style.height = item.resolvedStyle.height;
        //_icon.style.top = item.resolvedStyle.top;
        //_icon.style.left = item.resolvedStyle.left;
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
    /// <param name="pos">The position to draw the icon</param>
    public void SetPosition(Vector2 pos)
    {
        _icon.style.left = pos.x;
        _icon.style.top = pos.y;
    }
}
