using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class AccessioningController : MonoBehaviour
{
    // Fields
    [SerializeField] private SceneController _sceneController;

    private const string WRAPPER_CLS = "accessioning-column";
    private const string WRAPPER_ACTIVE_CLS = "accessioning-column--active";

    [SerializeField] private int _maxItems = 5;
    private int _currNumItems;
    private List<Item> _itemList;
    
    private VisualElement _root;
    private VisualElement _wrapper;

    private Accessioning _box;
    private Label _donationCountLabel;

    private Button _startDayBttn;

    private Label _dayLabel;
    private Label _satisfactionLabel;


    [SerializeField] private List<Transition> _transitionsList;
    [SerializeField] private TransitionStyle _transition;
    private Transition _transitionInfo;

    // Properties

    public static AccessioningController Instance;

    private void OnEnable()
    {
        _root = GetComponent<UIDocument>().rootVisualElement;
        _wrapper = _root.Q(className: WRAPPER_CLS);

        _box = _root.Q<Accessioning>();
        _donationCountLabel = _root.Q<Label>("DonationCountLabel");

        _startDayBttn = _root.Q<Button>("Start");
        _startDayBttn.clicked += OnStartDayClick;

        _dayLabel = _root.Q<Label>("DayCounterLabel");
        _satisfactionLabel = _root.Q<Label>("SatisfactionCounterLabel");

        _transitionInfo = _transitionsList.Find(t => t.Style == _transition);
        _wrapper.AddToClassList(_transitionInfo.ClassName);

        _itemList = new List<Item>();

    }

    public void OnDisable()
    {
        _startDayBttn.clicked -= OnStartDayClick;
    }

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    private async void Start()
    {
        if (DayManager.Instance != null)
        {
            _dayLabel.text = $"Day {DayManager.Instance.CurrentDay}";
        }
        else
        {
            _dayLabel.text = $"Day 1";
        }

        await DisplayAccessioning();

    }

    #region Events

    public async void OnStartDayClick()
    {
        await HideAccessioning();

        // Play VFX/SFX;

        await _sceneController.ChangeScene(Scene.ReadingRoom);
    }

    #endregion

    public async UniTask DisplayAccessioning()
    {
        await UniTask.WaitUntil(_box.TryGetDimensions);

        await UniTask.Delay(750);

        _box.GetDimensions();
        SpawnItems();

        _wrapper.AddToClassList(WRAPPER_ACTIVE_CLS);
        await UniTask.Delay(_transitionInfo.DurationMS);
    }

    public async UniTask HideAccessioning()
    {
        _wrapper.RemoveFromClassList(WRAPPER_ACTIVE_CLS);

        await UniTask.Delay(_transitionInfo.DurationMS);
    }

    /// <summary>
    /// Adds a number of randomly generated items inside the accessioning box
    /// </summary>
    /// <param name="count">Number of items to spawn</param>
    public void SpawnItems()
    {
        for (int i = 0; i < _maxItems; i++)
        {
            Item item = new Item();
            item.Spawn(_box);

            _box.Add(item);
        }

        SetDonationCount();
    }

    private void SetDonationCount()
    {
        _donationCountLabel.text = $"{_currNumItems}/{_maxItems} Donations Taken";
    }

    public void TakeDonation()
    {
        _currNumItems++;
        SetDonationCount();
    }

    public void ReturnDonation()
    {
        _currNumItems--;
        SetDonationCount();
    }

    public Accessioning GetBox()
    {
        return _box;
    }
}
