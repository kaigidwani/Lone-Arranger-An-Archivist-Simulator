using UnityEngine;
using UnityEngine.UIElements;

public class MainMenuController : MonoBehaviour
{
    // Fields
    [SerializeField] private SceneController _sceneController;
    private VisualElement _root;

    private Button _startBttn;
    private Button _optionsBttn;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        _root = GetComponent<UIDocument>().rootVisualElement;

        _startBttn = _root.Query<Button>("Start");
        _optionsBttn = _root.Query<Button>("Options");

        _startBttn.RegisterCallback<ClickEvent>(OnStartGame);
    }

    private void OnDisable()
    {
        _startBttn.UnregisterCallback<ClickEvent>(OnStartGame);
    }

    private void OnStartGame(ClickEvent evt)
    {
        Debug.Log("game started!");
        _sceneController.ChangeScene(Scene.Inventory);
    }
}
