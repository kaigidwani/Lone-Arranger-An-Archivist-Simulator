using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public enum TransitionStyle
{
    BasicFade,
    
}

[Serializable]
public struct Transition
{
    public TransitionStyle Style;
    public int DurationMS;
    public string ClassName;
}

public class TransitionManager : MonoBehaviour
{
    public static TransitionManager Instance;

    private const string WRAPPER_CLS = "overlay";
    private const string WRAPPER_ACTIVE_CLS = "overlay--active";

    private VisualElement _root;
    private VisualElement _wrapper;

    [SerializeField] private List<Transition> _transitionsList;
    [SerializeField] private TransitionStyle _transition;

    private void Awake()
    {
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

    private void OnEnable()
    {
        _root = GetComponent<UIDocument>().rootVisualElement;
        _wrapper = _root.Q(className: WRAPPER_CLS);

        Transition transitionInfo = _transitionsList.Find(t => t.Style == _transition);
        _wrapper.AddToClassList(transitionInfo.ClassName);
    }

    public async UniTask DisplayOverlay()
    {
        _wrapper.AddToClassList(WRAPPER_ACTIVE_CLS);

        await UniTask.Delay(500);
    }

    public async UniTask HideOverlay()
    {
        _wrapper.RemoveFromClassList(WRAPPER_ACTIVE_CLS);

        await UniTask.Delay(500);
    }
}
