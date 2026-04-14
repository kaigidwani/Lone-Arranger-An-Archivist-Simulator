using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PatronManager : MonoBehaviour
{
    // === Fields ===

    [SerializeField] private int _minReqItemAmount = 1;
    [SerializeField] private int _maxReqItemAmount = 4;

    private List<VisualElement> _patronsList;
    private VisualElement _patronLayer;

    [SerializeField] private GameObject requestsContainer;
    [SerializeField] private GameObject requestPrefab;


    // === Properties ===

    public static PatronManager Instance;

    /// <summary>
    /// The lower inclusive value of the randomized amount of items in a patron's request
    /// </summary>
    public int MinReqItemAmount { get {return _minReqItemAmount; } set { _minReqItemAmount = value; } }

    /// <summary>
    /// The upper inclusive value of the randomized amount of items in a patron's request
    /// </summary>
    public int MaxReqItemAmount { get { return _maxReqItemAmount; } set { _maxReqItemAmount = value; } }


    // === Methods ===

    private void Awake()
    {
        Instance = this;
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _patronsList = new List<VisualElement>();
        _patronLayer = RoomController.Instance.ReadingRoom.Q("PatronLayer");
    }

    public void StartSpawning()
    {
        SpawnPatron();
    }

    private async void SpawnPatron()
    {
        Patron patron = new Patron();
        _patronsList.Add(patron);
        _patronLayer.Add(patron);

        await patron.Spawn();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
