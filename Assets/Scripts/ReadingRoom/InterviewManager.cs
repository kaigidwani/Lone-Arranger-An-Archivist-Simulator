using UnityEditor.PackageManager.Requests;
using System.Collections.Generic;
using UnityEngine;

public class InterviewManager : MonoBehaviour
{
    // === Fields ===

    [SerializeField] private int patronsAmount;
    [SerializeField] private int minReqItemAmount;
    [SerializeField] private int maxReqItemAmount;

    [SerializeField] private List<PatronRequest> requestsList;
    [SerializeField] private List<Patron> patronsList;


    // === Properties ===

    /// <summary>
    /// Amount of patrons in a day
    /// </summary>
    public int PatronsAmount { get { return patronsAmount; } set { patronsAmount = value; } }

    /// <summary>
    /// The lower inclusive value of the randomized amount of items in a patron's request
    /// </summary>
    public int MinReqItemAmount { get {return minReqItemAmount; } set { minReqItemAmount = value; } }

    /// <summary>
    /// The upper inclusive value of the randomized amount of items in a patron's request
    /// </summary>
    public int MaxReqItemAmount { get { return maxReqItemAmount; } set { maxReqItemAmount = value; } }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Construct request list

        // Construct patrons list
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
