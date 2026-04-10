using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Fields

    // Properties

    public static GameManager Instance;

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
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
