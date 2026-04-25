using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
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
        
        _patronLayer.Add(patron);

        // LEFT: 27, BTM: 10

        if (PatronQueue.Peek() == patron)
        {
            patron.AddToClassList("position-1");
        }
        else
        {
            int index = Array.IndexOf(PatronQueue.ToArray(), patron);
            if (index == 1)
            {
                patron.AddToClassList("position-2");
            }
            else
            {
                patron.AddToClassList("position-3");
            }
        }

        await patron.Spawn();

        if (PatronQueue.Count == 1)
        {
            ShowFrontRequest();
        }
    }

    public async void RemovePatron()
    {
        Patron selectedPatron = PatronQueue.Dequeue();
        selectedPatron.RemoveFromClassList("position-1");
        selectedPatron.RemoveFromClassList("position-2");
        selectedPatron.RemoveFromClassList("position-3");

        // Shift everyone in the queue over
        if (PatronQueue.Count > 0)
        {
            foreach (Patron p in PatronQueue)
            {
                p.RemoveFromClassList("position-1");
                p.RemoveFromClassList("position-2");
                p.RemoveFromClassList("position-3");

                int index = Array.IndexOf(PatronQueue.ToArray(), p);
                if (index > 1)
                {
                    p.AddToClassList("position-3");
                    continue;
                }

                if (index == 1)
                {
                    p.AddToClassList("position-2");
                }       

                if (index == 0)
                {
                    p.AddToClassList("position-1");
                }
            }

            ShowFrontRequest();
        }

        await UniTask.Delay(400);
        selectedPatron.RemoveFromHierarchy();
        selectedPatron.RequestElem.RemoveFromHierarchy();
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void ShowFrontRequest()
    {
        Patron patronInFront = PatronQueue.Peek();
        _requestLayer.Add(patronInFront.RequestElem);
        patronInFront.ShowThoughtBubble();
    }
}
