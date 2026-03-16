using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.ProbeAdjustmentVolume;

[CreateAssetMenu(fileName = "New Item", menuName = "Scriptable Objects/Item")]
public class PlaceableItemSO : ScriptableObject
{
    // Fields

    [SerializeField] private int _baseWidth;
    [SerializeField] private int _baseHeight;
    [SerializeField] private string[] _activeTiles;

    // Properties

    public string Name;
    public string Description;
    public Sprite Sprite;
    public float Weight = 1;

    public int BaseWidth { get { return _baseWidth; } }

    public int BaseHeight { get { return _baseHeight; } }


    [HideInInspector] public int[][] BaseShape;

    /// <summary>
    /// Rotates a specific item's shape
    /// </summary>
    /// <param name="item">The item to rotate</param>
    /// <param name="dir">The direction to rotate in (positive = clockwise)</param>
    public void RotateItemShape(Item item, int dir)
    {
        // Swap width and height
        int newWidth = item.HeightInTiles;
        int newHeight = item.WidthInTiles;
        
        int[][] rotatedShape = new int[newHeight][];
        
        for (int x = 0; x < newHeight; x++)
        {
            for (int y = 0; y < newWidth; y++)
            {
                if (dir >= 0) // clockwise
                {
                    if (rotatedShape[x] == null)
                    {
                        rotatedShape[x] = new int[newWidth];
                    }

                    rotatedShape[x][newWidth - 1 - y] = item.Shape[y][x];
                }
                else // counter-clockwise
                {
                    if (rotatedShape[newHeight - 1 - x] == null)
                    {
                        rotatedShape[newHeight - 1 - x] = new int[newWidth];
                    }

                    rotatedShape[newHeight - 1 - x][y] = item.Shape[y][x];
                }
            }
        }

        // update item's properties
        item.WidthInTiles = newWidth;
        item.HeightInTiles = newHeight;
        item.Shape = rotatedShape;
    }

    private void OnValidate()
    {
        // Creates a new array in the inspector based on the dimensions of the item
        if (_activeTiles.Length != _baseHeight)
        {
            _activeTiles = new string[_baseHeight];
        }
    }

    public void GenerateShape()
    {
        // Converts the each row's string array of active tiles to a jagged array of integers
        BaseShape = ParseActiveTiles(_activeTiles, _baseWidth, _baseHeight);
    }

    /// <summary>
    /// Creates a set of base active tiles to act as the shape of this item
    /// </summary>
    /// <param name="tiles">2D string array with 1s and 0s, seperated by commas</param>
    /// <param name="width">Width of this item</param>
    /// <param name="height">Height of this item</param>
    /// <returns>The shape of this item as a 2D int arry</returns>
    private int[][] ParseActiveTiles(string[] tiles, int width, int height)
    {
        int[][] result = new int[height][];

        for (int x = 0; x < height; x++)
        {
            result[x] = new int[width];
            string[] cols = tiles[x].Split(',');

            for (int y = 0; y < width; y++)
            {
                result[x][y] = int.Parse(cols[y]);
            }
        }

        return result;
    }
}
