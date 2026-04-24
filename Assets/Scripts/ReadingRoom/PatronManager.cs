using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PatronManager : MonoBehaviour
{
    // === Fields ===

    [SerializeField] private int _minReqItemAmount = 1;
    [SerializeField] private int _maxReqItemAmount = 4;

    
    private VisualElement _patronLayer;
    private VisualElement _requestLayer;

    [SerializeField] private GameObject requestsContainer;
    [SerializeField] private GameObject requestPrefab;


    // === Properties ===

    public static PatronManager Instance;

    public Queue<Patron> PatronQueue;

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
        PatronQueue = new Queue<Patron>();
        _patronLayer = RoomController.Instance.ReadingRoom.Q("PatronLayer");
        _requestLayer = RoomController.Instance.ReadingRoom.Q("RequestLayer");
    }

    public async void SpawnPatron()
    {
        Patron patron = new Patron();
        PatronQueue.Enqueue(patron);
        _requestLayer.Add(patron.RequestElem);
        _patronLayer.Add(patron);

        await patron.Spawn();
    }

    public void RemovePatron()
    {
        Patron selectedPatron = PatronQueue.Dequeue();
        selectedPatron.RemoveFromHierarchy();
        selectedPatron.RequestElem.RemoveFromHierarchy();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
