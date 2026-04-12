using System.Collections.Generic;
using UnityEngine;


public struct PatronRequestItem
{
    public SlotColor Color;
    public string ItemName;
}

public class PatronRequest : MonoBehaviour
{
    // === Fields ===

    [SerializeField] private List<Item> _itemsList;
    [SerializeField] private int _reqNum;
    [SerializeField] private Patron _patronRef;
    [SerializeField] private int _itemAmount;
    [SerializeField] private bool _isComplete;


    // === Properties ===

    /// <summary>
    /// Get the items list of a request
    /// </summary>
    public List<Item> ItemsList { get { return _itemsList; } }

    /// <summary>
    /// Get the request's number
    /// </summary>
    public int ReqNum { get { return _reqNum; } }

    /// <summary>
    /// Gets or sets the bool that this request is completed
    /// </summary>
    public bool IsComplete { get { return _isComplete; } set { value = _isComplete; } }


    // === Methods ===

    /// <summary>
    /// Creates a new patron request.
    /// </summary>
    /// <param name="reqNum">Integer index of this request. Used for ID and numerating the UI elements.</param>
    /// <param name="itemAmount">Integer amount of items in the request.</param>
    public PatronRequest(int reqNum, int itemAmount)
    {
        _reqNum = reqNum;
        _itemAmount = itemAmount;

        // TODO: Generate the list of items
        // Maybe make a helper method for this
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void GenerateItemLists()
    {
        // Create 2 lists, one of the item types, one of the item colors
        // Use itemsList prefab to acess the diffrent types
        // Colors are string and will be either "red" "green" "blue" or "" (for blank).


        return;
    }
}
