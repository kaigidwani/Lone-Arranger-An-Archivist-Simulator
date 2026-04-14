using UnityEngine;

public class DayManager : MonoBehaviour
{
    public static DayManager Instance;

    public int CurrentDay { get; private set; } = 1;

    private void Awake()
    {
        Instance = this;
    }

    public void StartDay()
    {
        Debug.Log($"Starting Day {CurrentDay}");

        PatronManager.Instance.StartSpawning();
    }

    public void EndDay()
    {
        //SaveSystem.SaveInventory();

        CurrentDay++;
    }
}
