using UnityEngine;
using UnityEngine.UIElements;

public enum SlotColor
{
    None,
    Red,
    Green,
    Blue,
}

[UxmlElement]
public partial class Slot : VisualElement
{
    // Fields

    // Properties
    public Label DebugLabel { get; set; }

    /// <summary>
    /// The position of this slot on the inventory grid
    /// (X = Row, Y = Column)
    /// </summary>
    public Vector2Int GridIndex { get; set; }

    /// <summary>
    /// Color of this slot as a string
    /// </summary>
    public SlotColor Color { get; private set; }

    /// <summary>
    /// Which tile is currently contained in this slot
    /// </summary>
    public ItemTile TileRef { get; private set; }

    public bool IsFree { get { return TileRef == null; } }

    public Slot()
    {
        RegisterCallbackOnce<GeometryChangedEvent>(evt =>
        {
            Color = GetColor();
        });

        DebugLabel = new Label("Empty");
        DebugLabel.visible = false;
        DebugLabel.AddToClassList("debug-text");

        Add(DebugLabel);
    }

    /// <summary>
    /// Finds the color of this slot via its classlist
    /// </summary>
    /// <returns>A string containing the color of this slot</returns>
    private SlotColor GetColor()
    {
        foreach (string className in GetClasses())
        {
            if (className.StartsWith("inventory-slot-"))
            {
                string colorStr = className.Replace("inventory-slot-", "");

                switch (colorStr)
                {
                    case "red":
                        return SlotColor.Red;

                    case "green":
                        return SlotColor.Green;

                    case "blue":
                        return SlotColor.Blue;

                    case "default":
                        return SlotColor.None;

                }
            }
        }

        return SlotColor.None;

    }

    /// <summary>
    /// Adds a reference to an item to this inventory slot
    /// </summary>
    /// <param name="tile">An item that should respond to this slot</param>
    public void SetTile(ItemTile tile)
    {
        TileRef = tile;
        DebugLabel.text = TileRef.name;
    }

    public void ClearTile()
    {
        TileRef = null;
        DebugLabel.text = "Empty";
    }
}
