using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.ProbeAdjustmentVolume;

[CreateAssetMenu(fileName = "New Item", menuName = "Scriptable Objects/Item")]
public class PlaceableItemSO : ScriptableObject
{
    // Fields

    [SerializeField] private Vector2Int _baseDimensions;
    [SerializeField] private string[] _baseActiveTiles;

    // Properties

    public string Name;
    public string Description;
    public Sprite Sprite;
    public int Rotation = 0;

    public int Width { get; private set; }

    public int Height { get; private set; }

    [HideInInspector] public int[][] Shape;
    
    private void OnValidate()
    {
        Rotation = 0;

        // Creates a new array in the inspector based on the dimensions of the item
        if (_baseActiveTiles.Length != _baseDimensions.y)
        {
            _baseActiveTiles = new string[_baseDimensions.y];
        }

        Width = _baseDimensions.x;
        Height = _baseDimensions.y;
        Shape = ParseActiveTiles(_baseActiveTiles, Width, Height);
    }

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

    private string[] RebuildActiveTiles(int[][] shape)
    {
        string[] rowList = new string[shape.Length];

        for (int x = 0; x < shape.Length; x++)
        {
            rowList[x] = string.Join(',', shape[x]);
        }

        return rowList;
    }

    public void RotateShape(int dir)
    {
        int[][] currentShape = Shape;
        int[][] rotatedShape = new int[Width][];
        
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                if (dir >= 0)
                {
                    if (rotatedShape[x] == null)
                    {
                        rotatedShape[x] = new int[Height];
                    }

                    rotatedShape[x][Height - 1 - y] = currentShape[y][x];
                }
                else
                {
                    if (rotatedShape[Width - 1 - x] == null)
                    {
                        rotatedShape[Width - 1 - x] = new int[Height];
                    }

                    rotatedShape[Width - 1 - x][y] = currentShape[y][x];
                }
            }
        }

        Width = rotatedShape[0].Length;
        Height = rotatedShape.Length;

        string[] newActiveTiles = RebuildActiveTiles(rotatedShape);
        Shape = ParseActiveTiles(newActiveTiles, Width, Height);
    }
}
