using UnityEngine.UIElements;
using UnityEngine;
using System.Collections.Generic;

public enum RoomType
{
    Archive,
    ReadingRoom
}

public class RoomController : MonoBehaviour
{
    // Fields
    private const string HIDDEN_LEFT_CLS = "hidden-left";
    private const string HIDDEN_RIGHT_CLS = "hidden-right";

    private VisualElement _root;

    private VisualElement _archive;
    private VisualElement _readingRoom;

    private Button _toArchiveBttn;
    private Button _toReadingRoomBttn;

    private Button _sendBttn;

    private RoomType _currRoomType;

    private Patron _selectedPatron;

    // Properties

    public VisualElement ReadingRoom { get { return _readingRoom; } }

    public VisualElement ArchiveRoom { get { return _archive; } }

    public static RoomController Instance;

    private void Awake()
    {
        _currRoomType = RoomType.ReadingRoom;
        Instance = this;
    }

    private void OnEnable()
    {
        _root = GetComponent<UIDocument>().rootVisualElement;

        _archive = _root.Q("Archive");
        _toReadingRoomBttn = _archive.Q<Button>("NextRoomBttn");
        _toReadingRoomBttn.clicked += SwitchRoom;

        _readingRoom = _root.Q("ReadingRoom");
        _toArchiveBttn = _readingRoom.Q<Button>("NextRoomBttn");
        _toArchiveBttn.clicked += SwitchRoom;

        _sendBttn = _archive.Q<Button>("SendBttn");
        _sendBttn.clicked += FulfillRequest; 
    }

    private void FulfillRequest()
    {
        SwitchRoom();

        _selectedPatron = _root.Q<Patron>(className: "selected");

        _selectedPatron.CompleteRequest();

        List<Item> itemsDonated = new List<Item>();
        foreach (Item item in GameManager.Instance.StoredItems)
        {
            if (item.CurrentState == ItemState.InDonationBox)
            {
                itemsDonated.Add(item);
            }
        }

        // To Do: Calculate Results

        for (int i = 0; i < itemsDonated.Count; i++)
        {
            itemsDonated[i].RemoveFromHierarchy();
            GameManager.Instance.StoredItems.Remove(itemsDonated[i]);
        }
    }

    private void Start()
    {
        StartCoroutine(DayManager.Instance.StartDay());
    }

    public void SwitchRoom()
    {
        switch (_currRoomType)
        {
            case RoomType.Archive:
                _readingRoom.RemoveFromClassList(HIDDEN_LEFT_CLS);
                _archive.AddToClassList(HIDDEN_RIGHT_CLS);

                _currRoomType = RoomType.ReadingRoom;
                break;

            case RoomType.ReadingRoom:
                _archive.RemoveFromClassList(HIDDEN_RIGHT_CLS);
                _readingRoom.AddToClassList(HIDDEN_LEFT_CLS);

                _currRoomType = RoomType.Archive;
                break;
        }
    }
}
