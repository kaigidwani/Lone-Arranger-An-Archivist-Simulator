using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemsListSO", menuName = "Scriptable Objects/ItemsListSO")]
public class ItemsListSO : ScriptableObject
{
    // === Fields ===

    /// <summary>
    /// This is populated in editor with all the PlacableItemSOs avaialable
    /// in the game. It should be sorted in the order that they are unlocked
    /// if that feature is being used.
    /// </summary>
    [SerializeField] private List<PlaceableItemSO> _allItemTypes;

    /// <summary>
    /// This is populated in code using the Add method. This is instantialized
    /// at the start in the game manager singleton.
    /// </summary>
    [SerializeField] private List<PlaceableItemSO> _currentItemTypes;

    /// <summary>
    /// This is the amount of items that will be populated into currentItemTypes
    /// at the start of the game.
    /// </summary>
    [SerializeField] private int _starterItemsAmount;

    /// <summary>
    /// Current index of the currentItemTypes list.
    /// </summary>
    [SerializeField] private int _currentIndex = 0;


    // === Properties ===

    /// <summary>
    /// Gets the list of current item types available for random generation
    /// </summary>
    public List<PlaceableItemSO> CurrentItemTypes {  get { return _currentItemTypes; } }

    /// <summary>
    /// Gets the current index of the currentItemTypes list
    /// </summary>
    public int CurrentIndex { get { return _currentIndex; } }


    // === Methods ===

    /// <summary>
    /// Adds the next item types to the list of currentItemTypes for random generation.
    /// </summary>
    /// <param name="amountOfItemsToAdd">Optional amount of items to try to add. Default is 1</param>
    public void Add(int amountOfItemsToAdd = 1)
    {
        // If the new number of types would not exceed all the itemTypes available, causing an out of bounds error
        if (_allItemTypes.Count >= _currentItemTypes.Count + amountOfItemsToAdd)
        {
            // Loop for as many items are being added
            for (int i = 0; i <= amountOfItemsToAdd; i++)
            {
                // Add the item type from the list of all item types to the list of current item types
                _currentItemTypes.Add(_allItemTypes[_currentIndex]);

                // Increment the current index
                _currentIndex++;
            }
        }
        else // Otherwise the game is trying to read out of bounds data and print an error
        {
            Console.WriteLine("Trying to add more item types than exist.");
            Console.WriteLine("_allItemTypes.Count: " + _allItemTypes.Count + "\n" +
                              "_currentItemTypes.Count: " + _currentItemTypes.Count + "\n" +  
                              "_currentInfex: " + _currentIndex);
        }
    }
        
}
