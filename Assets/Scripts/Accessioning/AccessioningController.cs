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

    private List<Item> _itemList;

    private VisualElement _root;
    private VisualElement _wrapper;
    private Accessioning _box;

    private Button _startDayBttn;

    [SerializeField] private List<Transition> _transitionsList;
    [SerializeField] private TransitionStyle _transition;
    private Transition _transitionInfo;

    private void OnEnable()
    {
        _root = GetComponent<UIDocument>().rootVisualElement;
        _wrapper = _root.Q(className: WRAPPER_CLS);
        _box = _root.Q<Accessioning>();

        _startDayBttn = _root.Q<Button>("Start");
        _startDayBttn.RegisterCallback<ClickEvent>(StartDay);

        _transitionInfo = _transitionsList.Find(t => t.Style == _transition);
        _wrapper.AddToClassList(_transitionInfo.ClassName);

        _itemList = new List<Item>();

        SceneController.OnSceneLoad += DisplayAccessioning;

    }


    public void OnDisable()
    {
        SceneController.OnSceneLoad -= DisplayAccessioning;
        SceneController.OnSceneExit -= HideAccessioning;

        _startDayBttn.UnregisterCallback<ClickEvent>(StartDay);
    }

    public void StartDay(ClickEvent evt)
    {
        SceneController.OnSceneExit += HideAccessioning;
        _sceneController.ChangeScene(Scene.MainMenu);
    }

    public async void DisplayAccessioning(SceneController controller)
    {
        _box.GetDimensions();
        SpawnItems();

        _wrapper.AddToClassList(WRAPPER_ACTIVE_CLS);

        await UniTask.Delay(_transitionInfo.DurationMS);
          
    }

    public void HideAccessioning(SceneController controller)
    {
        _wrapper.RemoveFromClassList(WRAPPER_ACTIVE_CLS);
    }

    /// <summary>
    /// Adds a number of randomly generated items inside the accessioning box
    /// </summary>
    /// <param name="count">Number of items to spawn</param>
    public void SpawnItems(int count = 5)
    {
        for (int i = 0; i < count; i++)
        {
            Item item = new Item();
            item.Spawn(_box);

            _box.Add(item);

        }
    }
}
