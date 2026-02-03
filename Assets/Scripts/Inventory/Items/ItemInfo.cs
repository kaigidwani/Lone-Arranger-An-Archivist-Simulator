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
    public List<int[]> Shape
    {
        get
        {
            List<int[]> shape = new List<int[]>();
            Debug.Log($"{Name}:{_activeTiles.Length}");

            for (int row = 0; row < _activeTiles.Length; row++)
            {
                string[] rowString = _activeTiles[row].Split(',');
                Debug.Log($"{Name}: running on row");
                    
                for (int col = 0; col < rowString.Length; col++)
                {
                    Debug.Log($"{Name}: running on coll");
                    shape[row][col] = int.Parse(rowString[col]);
                }

            }

            return shape;
        }
    }
    
    private void OnValidate()
    {
        if (_activeTiles.Length != Dimensions.y)
        {
            _activeTiles = new string[(int)Dimensions.y];
        }

        Debug.Log($"{Name}: " + Shape);
        
    }
}
