using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using static UnityEditor.Progress;

[UxmlElement]
public partial class GhostIcon : VisualElement
{
    // Fields

    private VisualElement _icon;


    // Properties
    public Label DebugLabel;

    public GhostIcon()
    {
        // Make sure styling is applied
        if (!ClassListContains("ghost-icon"))
        {
            AddToClassList("ghost-icon");
        }

        _icon = new VisualElement();
        _icon.AddToClassList("ghost-icon-visual");
        Add(_icon);

        DebugLabel = new Label("");
        DebugLabel.AddToClassList("debug-text");
        Add(DebugLabel);
    }

    /// <summary>
    /// Sets the ghost icon's visual to mirror an item
    /// </summary>
    /// <param name="item">The item to set the icon to</param>
    public void SetItem(Item item)
    {
        MatchItemStyle(item);
        MatchItemRotation(item);
    }

    public void MatchItemStyle(Item item)
    {
        style.width = item.resolvedStyle.width;
        style.height = item.resolvedStyle.height;

        _icon.style.width = item.resolvedStyle.width;
        _icon.style.height = item.resolvedStyle.height;
        _icon.style.backgroundImage = item.Type.Sprite.texture;
    }

    public void MatchItemRotation(Item item)
    {
        style.rotate = item.style.rotate;
        SetToMousePosition(item.Pivot);
    }

    /// <summary>
    /// Removes any icon that is currently being displayed while dragging
    /// </summary>
    public void ResetIcon()
    {
        style.left = 0;
        style.top = 0;
        style.width = 0;
        style.height = 0;
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

        style.left = mousePanel.x - colOffset - tileWidth * 0.5f;
        style.top = mousePanel.y - rowOffset - tileHeight * 0.5f;

    }
}
