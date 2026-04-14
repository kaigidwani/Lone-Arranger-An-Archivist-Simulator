using UnityEngine.UIElements;
using UnityEngine;

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

    private RoomType _currRoomType;

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
        _toReadingRoomBttn.clicked += OnSwitchRoomClick;

        _readingRoom = _root.Q("ReadingRoom");
        _toArchiveBttn = _readingRoom.Q<Button>("NextRoomBttn");
        _toArchiveBttn.clicked += OnSwitchRoomClick;
    }

    private void Start()
    {
        DayManager.Instance.StartDay();
    }

    public void OnSwitchRoomClick()
    {
        Debug.Log("switching rooms");

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
