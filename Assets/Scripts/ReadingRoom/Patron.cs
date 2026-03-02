using UnityEditor.PackageManager.Requests;
using UnityEngine;

public class Patron : MonoBehaviour
{
    // === Fields ===

    [SerializeField] private PatronRequest _requestRef;


    // === Properties ===

    /// <summary>
    /// Gets or sets the request reference of this patron
    /// </summary>
    public PatronRequest RequestRef {  get { return _requestRef; } set { value = _requestRef; } }


    // === Methods ===

    /// <summary>
    /// Creates a new Patron
    /// </summary>
    /// <param name="requestRef">The request the patron will hold a reference to</param>
    public Patron(PatronRequest requestRef)
    {
        _requestRef = requestRef;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // TODO:
        // - Check if the player has taken the request yet
        // - Then: Check if the request has been completed yet
    }
}
