using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    // Fields

    // Properties

    public static GameManager Instance;

    public PlaceableItemSO[] ItemPool;
    public List<Item> StoredItems;

    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        
    }

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StoredItems = new List<Item>();
    }
}
