using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Scriptable Objects/Item")]
public class PlaceableItemSO : ScriptableObject
{
    // Fields

    [SerializeField] private string[] _activeTiles;

    // Properties

    public string Name;
    public string Description;

    /// <summary>
    /// (X = Rows, Y = Columns)
    /// </summary>
    public Vector2 Dimensions;

    public Sprite Sprite;

    [HideInInspector] public int[][] Shape;
    
    private void OnValidate()
    {
        // Creates a new array in the inspector based on the dimensions of the item
        if (_activeTiles.Length != Dimensions.x)
        {
            _activeTiles = new string[(int)Dimensions.x];
        }

        // Converts the each row's string array of active tiles to a jagged array of integers
        // for easy parsing later on
        Shape = new int[(int)Dimensions.x][];

        for (int row = 0; row < _activeTiles.Length; row++)
        {
            string[] rowString = _activeTiles[row].Split(',');
            Shape[row] = new int[rowString.Length];

            for (int col = 0; col < rowString.Length; col++)
            {
                Shape[row][col] = int.Parse(rowString[col]);
            }

        }
    }

    
}
