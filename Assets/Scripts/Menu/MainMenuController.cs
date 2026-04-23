using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class MainMenuController : MonoBehaviour
{
    // Fields
    [SerializeField] private SceneController _sceneController;
    private VisualElement _root;

    private Button _startBttn;
    private Button _optionsBttn;
    private Button _quitBttn;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        _root = GetComponent<UIDocument>().rootVisualElement;      
    }

    private void Start()
    {
        // Whjenever you come back to the menu, the menu track plays
        MusicManager.Instance.PlayTrack("Menu");
    }

    private void OnEnable()
    {
        _startBttn = _root.Query<Button>("Start");
        _startBttn.clicked += OnStartButtonClicked;
        _startBttn.RegisterCallback<MouseEnterEvent>(ButtonBehavior.OnMouseEnter);
        _startBttn.RegisterCallback<MouseLeaveEvent>(ButtonBehavior.OnMouseExit);

        _optionsBttn = _root.Query<Button>("Options");
        _optionsBttn.clicked += OnOptionsButtonClicked;
        _optionsBttn.RegisterCallback<MouseEnterEvent>(ButtonBehavior.OnMouseEnter);
        _optionsBttn.RegisterCallback<MouseLeaveEvent>(ButtonBehavior.OnMouseExit);

        _quitBttn = _root.Query<Button>("Quit");
        _quitBttn.clicked += OnQuitButtonClicked;
        _quitBttn.RegisterCallback<MouseEnterEvent>(ButtonBehavior.OnMouseEnter);
        _quitBttn.RegisterCallback<MouseLeaveEvent>(ButtonBehavior.OnMouseExit);
    }

    private void OnDisable()
    {
        _startBttn.clicked -= OnStartButtonClicked;
        _startBttn.UnregisterCallback<MouseEnterEvent>(ButtonBehavior.OnMouseEnter);
        _startBttn.UnregisterCallback<MouseLeaveEvent>(ButtonBehavior.OnMouseExit);

        _optionsBttn.clicked -= OnOptionsButtonClicked;
        _optionsBttn.UnregisterCallback<MouseEnterEvent>(ButtonBehavior.OnMouseEnter);
        _optionsBttn.UnregisterCallback<MouseLeaveEvent>(ButtonBehavior.OnMouseExit);

        _quitBttn.clicked -= OnQuitButtonClicked;
        _quitBttn.RegisterCallback<MouseEnterEvent>(ButtonBehavior.OnMouseEnter);
        _quitBttn.RegisterCallback<MouseLeaveEvent>(ButtonBehavior.OnMouseExit);
    }

    private void OnStartButtonClicked()
    {
        _sceneController.ChangeScene(Scene.Inventory);
        MusicManager.Instance.PlayTrack("Gameplay");
    }

    private void OnOptionsButtonClicked()
    {
        Debug.Log("There are no options yet...");
    }

    private void OnQuitButtonClicked()
    {
        Application.Quit();

#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#endif
    }
}
