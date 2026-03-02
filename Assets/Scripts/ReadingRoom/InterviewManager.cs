using UnityEditor.PackageManager.Requests;
using System.Collections.Generic;
using UnityEngine;

public class InterviewManager : MonoBehaviour
{
    // === Fields ===

    [SerializeField] private int _patronsAmount;
    [SerializeField] private int _minReqItemAmount;
    [SerializeField] private int _maxReqItemAmount;

    [SerializeField] private List<PatronRequest> _requestsList;
    [SerializeField] private List<Patron> _patronsList;


    // === Properties ===

    /// <summary>
    /// Amount of patrons in a day
    /// </summary>
    public int PatronsAmount { get { return _patronsAmount; } set { _patronsAmount = value; } }

    /// <summary>
    /// The lower inclusive value of the randomized amount of items in a patron's request
    /// </summary>
    public int MinReqItemAmount { get {return _minReqItemAmount; } set { _minReqItemAmount = value; } }

    /// <summary>
    /// The upper inclusive value of the randomized amount of items in a patron's request
    /// </summary>
    public int MaxReqItemAmount { get { return _maxReqItemAmount; } set { _maxReqItemAmount = value; } }


    // === Methods ===

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Construct request and patron lists

        // Loop for the amount of patrons we will have
        for (int i = 0; i < PatronsAmount; i++)
        {
            // Get amount of items for this list
            int itemAmount = Random.Range(_minReqItemAmount, _maxReqItemAmount + 1);

            // Create a new request and add it to the list
            _requestsList.Add(new PatronRequest(i, itemAmount));

            // Create a new patron and add it to the list
            _patronsList.Add(new Patron(_requestsList[i]));

            // Now that the patron has been created,
            // set the request's patron reference to the patron
            _requestsList[i].PatronRef = _patronsList[i];
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
