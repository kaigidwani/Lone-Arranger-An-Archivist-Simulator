using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Item", menuName = "Scriptable Objects/Item")]
public class ItemInfo : ScriptableObject
{
    // Fields
    [SerializeField] private string[] _activeTiles;

    // Properties
    public string Name;
    public string Description;
    public Vector2 Dimensions;
    public Sprite Sprite;

    [HideInInspector]
    public int[][] Shape;
    
    private void OnValidate()
    {
        // Creates a new array in the inspector based on the dimensions of the item
        if (_activeTiles.Length != Dimensions.y)
        {
            _activeTiles = new string[(int)Dimensions.y];
        }

        // Converts the each row's string array of active tiles to a jagged array of integers
        // for easy parsing later on
        Shape = new int[(int)Dimensions.y][];

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
