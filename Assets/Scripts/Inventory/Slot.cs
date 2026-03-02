using UnityEngine;
using UnityEngine.UIElements;

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
    public string Color { get; private set; }

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
        DebugLabel.AddToClassList("debug-text");

        Add(DebugLabel);
    }

    /// <summary>
    /// Finds the color of this slot via its classlist
    /// </summary>
    /// <returns>A string containing the color of this slot</returns>
    private string GetColor()
    {
        foreach (string className in GetClasses())
        {
            if (className.StartsWith("inventory-slot-"))
            {
                return className.Replace("inventory-slot-", "");
            }
        }

        return "";

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
