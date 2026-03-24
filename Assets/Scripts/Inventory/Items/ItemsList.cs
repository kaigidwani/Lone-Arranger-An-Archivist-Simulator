using UnityEngine;

public class ItemsList : MonoBehaviour
{
    // === Fields ===

    [SerializeField] private PlaceableItemSO[] _itemPool;


    // === Properties ===

    public PlaceableItemSO[] ItemPool { get { return _itemPool; } }


    // === Methods ===

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
