using Cysharp.Threading.Tasks;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class DayManager : MonoBehaviour
{
    // Fields
    [SerializeField] private SceneController _sceneController;

    private VisualElement _root;
    private VisualElement _resultsOverlay;
    private VisualElement _resultsContainer;
    private VisualElement _hr;
    private Label _headerLabel; 
    private Label _patronsHelpedLabel; 
    private VisualElement _patronsHelpedContainer; 
    private Label _satisfactionLabel;
    private VisualElement _satisfactionContainer;
    private Label _demoFinishLabel;
    private Button _mainMenuBttn;

    private int _numPatronsHelped = 0;

    private bool _isDayComplete;


    [SerializeField] private float _baseDayLengthMin = 88.5f;
    [SerializeField] private float _baseDayLengthMax = 148.5f;

    [SerializeField] private float _spawnDelayMin = 20f;
    [SerializeField] private float _spawnDelayMax = 30f;

    [SerializeField] private float _dayLengthMultiplier = 1.5f;

    private float _dayLength;
    private float _dayProgress;

    // Properties 

    public static DayManager Instance;

    public int CurrentDay { get; private set; } = 1;

    private void OnEnable()
    {
        _root = GetComponent<UIDocument>().rootVisualElement;
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

    private void Start()
    {
        _resultsOverlay = _root.Q("Overlay");

        _resultsContainer = _root.Q("ResultsContainer");

        _hr = _root.Q("HR");

        _headerLabel = _root.Q<Label>("Header");

        _demoFinishLabel = _root.Q<Label>("Thanks");

        _patronsHelpedContainer = _root.Q("PatronsHelped");
        _patronsHelpedLabel = _patronsHelpedContainer.Q<Label>("Amount");

        _satisfactionContainer = _root.Q("SatisfactionGained");
        _satisfactionLabel = _satisfactionContainer.Q<Label>("Amount");

        _mainMenuBttn = _root.Q<Button>("Menu");
        _mainMenuBttn.clicked += OnMenuButtonClick;
    }

    private void Update()
    {
        if (!_isDayComplete)
        {
            _dayProgress += Time.deltaTime;
            Debug.Log($"time left in day = {_dayLength - _dayProgress}");

        }
    }

    public IEnumerator StartDay()
    {
        _isDayComplete = false;
        _dayLength = Random.Range(10 + (CurrentDay * _dayLengthMultiplier),
            20 + (CurrentDay * _dayLengthMultiplier));

        while (true)
        {
            if (_dayLength - _dayProgress <= 0)
            {
                _isDayComplete = true;
                break;
            }

            // Spawn patrons until end of day
            PatronManager.Instance.SpawnPatron();
            _numPatronsHelped++;

            float delay = Random.Range(_spawnDelayMin, _spawnDelayMax);
            float elapsed = 0f;

            while (elapsed < delay)
            {
                if (_dayLength - _dayProgress <= 0)
                {
                    _isDayComplete = true;
                    break;
                }

                elapsed += Time.deltaTime;
                yield return null;
            }
        }

        // Wait for player to help patrons
        while (PatronManager.Instance.PatronQueue.Count > 0)
        {
            yield return null;
        }

        EndDay();
        
    }

    public async void EndDay()
    {
        //SaveSystem.SaveInventory();
        await ShowResults();

        CurrentDay++;
        _numPatronsHelped = 0;

    }

    private async UniTask ShowResults()
    {
        _resultsOverlay.RemoveFromClassList("hidden");

        await UniTask.Delay(400);

        _resultsContainer.RemoveFromClassList("hidden-top");

        await UniTask.Delay(600);

        _headerLabel.RemoveFromClassList("hidden");
        _hr.AddToClassList("horizontal-line--active");

        await UniTask.Delay(500);

        _patronsHelpedContainer.RemoveFromClassList("hidden");

        await UniTask.Delay(300);

        _patronsHelpedLabel.text = $"{_numPatronsHelped}";
        _patronsHelpedLabel.RemoveFromClassList("hidden");

        await UniTask.Delay(500);

        _satisfactionContainer.RemoveFromClassList("hidden");

        await UniTask.Delay(300);

        _satisfactionLabel.text = $"{_numPatronsHelped}";
        _satisfactionLabel.RemoveFromClassList("hidden");

        await UniTask.Delay(800);
        _demoFinishLabel.RemoveFromClassList("hidden");

        await UniTask.Delay(2000);
        _mainMenuBttn.RemoveFromClassList("hidden");
    }

    private async UniTask HideResults()
    {
        _resultsOverlay.AddToClassList("hidden");

        await UniTask.Delay(400);

        _resultsContainer.AddToClassList("hidden-top");

        _headerLabel.AddToClassList("hidden");
        _hr.RemoveFromClassList("horizontal-line--active");

        _patronsHelpedContainer.AddToClassList("hidden");

        _patronsHelpedLabel.text = $"";

        _patronsHelpedLabel.AddToClassList("hidden");

        _satisfactionContainer.AddToClassList("hidden");

        _satisfactionLabel.text = $"";

        _satisfactionLabel.AddToClassList("hidden");

        _demoFinishLabel.AddToClassList("hidden");

        _mainMenuBttn.AddToClassList("hidden");
    }

    private async void OnMenuButtonClick()
    {
        // Demo Things
        CurrentDay = 1;

        GameManager.Instance.ResetInventory();

        await HideResults();
        await _sceneController.ChangeScene(Scene.MainMenu);
    }
}
