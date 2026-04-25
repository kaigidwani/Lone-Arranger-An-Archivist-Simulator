using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEditorInternal.Profiling.Memory.Experimental;

public class Patron : VisualElement
{
    // === Fields ===

    private List<(SlotColor Color, PlaceableItemSO Item)> _request;

    private VisualElement _requestContainer;
    private VisualElement _shadow;
    private VisualElement _textBox;
    private VisualElement _horizontalLine;
    private Label _requestHeader;
    private Label _requestDescription;
    private VisualElement _itemList;
    private Button _closeButton;
    private bool isComplete = false;

    // === Properties ===

    public VisualElement RequestElem { get { return _requestContainer; } }

    // === Methods ===

    public Patron()
    {
        AddToClassList("patron");
        AddToClassList("hidden-left");

        BuildPatronRequest();
    }

    private void BuildPatronRequest()
    {
        _requestContainer = new VisualElement();
        _requestContainer.AddToClassList("request");
        _requestContainer.AddToClassList("hidden");
        _requestContainer.pickingMode = PickingMode.Ignore;

        _shadow = new VisualElement();
        _shadow.AddToClassList("shadow");
        _requestContainer.Add(_shadow);

        _textBox = new VisualElement();
        _textBox.AddToClassList("request-text-box");
        _textBox.RegisterCallback<MouseDownEvent>(OnRequestClick);
        _textBox.RegisterCallback<MouseEnterEvent>(OnRequestHover);
        _textBox.RegisterCallback<MouseLeaveEvent>(OnRequestHoverExit);
        _requestContainer.Add(_textBox);

        _requestHeader = new Label();
        _requestHeader.AddToClassList("request-label");
        _requestHeader.text = "...";
        _textBox.Add(_requestHeader);

        _requestDescription = new Label();
        _requestDescription.AddToClassList("request-label");
        _requestDescription.AddToClassList("hidden");

        _horizontalLine = new VisualElement();
        _horizontalLine.AddToClassList("horizontal-line");
        _textBox.Add(_horizontalLine);

        _itemList = new VisualElement();
        _itemList.AddToClassList("item-list");

        _closeButton = new Button();
        _closeButton.AddToClassList("close-button");
        _closeButton.AddToClassList("hidden");
        _closeButton.text = "Got it!";
        _requestContainer.Add(_closeButton);
    }

    private void OnRequestHover(MouseEnterEvent evt)
    {
        _textBox.AddToClassList("text-box--hover");
        _shadow.AddToClassList("shadow--hover");
    }

    private void OnRequestHoverExit(MouseLeaveEvent evt)
    {
        _textBox.RemoveFromClassList("text-box--hover");
        _shadow.RemoveFromClassList("shadow--hover");
    }

    private async void OnRequestClick(MouseDownEvent evt)
    {
        await EnlargeTextBox("I'm looking for...", isComplete);
        await DisplayRequest();
        await UniTask.WaitForSeconds(1);

        _closeButton.RemoveFromClassList("hidden");
        _closeButton.clicked += HideRequest;
    }

    private async UniTask DisplayRequest()
    {
        AddToClassList("selected");

        _itemList.RemoveFromClassList("hidden");
        _textBox.Add(_itemList);

        if (_request == null)
        {
            GenerateRequest();
        }
        else 
        {
            // Reset item list to display animation
            List<VisualElement> childrenToRemove = new List<VisualElement>();
            foreach (VisualElement child in _itemList.Children())
            {
                childrenToRemove.Add(child);
            }

            for (int i = 0; i < childrenToRemove.Count; i++)
            {
                childrenToRemove[i].RemoveFromHierarchy();
            }
        }

        for (int i = 0; i < _request.Count; i++)
        {
            VisualElement itemContainer = new VisualElement();
            itemContainer.AddToClassList("item-display-bg");

            switch (_request[i].Color)
            {
                case SlotColor.Red:
                    itemContainer.AddToClassList("item-display-bg-red");
                    break;

                case SlotColor.Green:
                    itemContainer.AddToClassList("item-display-bg-green");
                    break;

                case SlotColor.Blue:
                    itemContainer.AddToClassList("item-display-bg-blue");
                    break;

                default:
                    itemContainer.AddToClassList("item-display-bg-none");
                    break;
            }

            itemContainer.AddToClassList("hidden");

            PlaceableItemSO so = _request[i].Item;
            VisualElement itemVisual = new VisualElement();
            itemVisual.AddToClassList("item-visual");
            itemVisual.style.backgroundImage = so.Sprite.texture;

            StyleLength width = 0;
            StyleLength height = 0;
            if (so.BaseWidth > so.BaseHeight)
            {
                width = Length.Percent(75);
                height = Length.Percent(((float)so.BaseHeight / so.BaseWidth) * 75);
            }
            else
            {
                width = Length.Percent(((float)so.BaseWidth / so.BaseHeight) * 75);
                height = Length.Percent(75);
            }

            itemVisual.style.width = width;
            itemVisual.style.height = height;

            // Making tiles so that empty parts of shape will not be colored
            StyleLength tileWidth = Length.Percent(width.value.value / so.BaseWidth);
            StyleLength tileHeight = Length.Percent(height.value.value / so.BaseHeight);
            Debug.Log(tileWidth + ", " + tileHeight);

            itemContainer.Add(itemVisual);
            _itemList.Add(itemContainer);

            await UniTask.Delay(450);
            itemContainer.RemoveFromClassList("hidden");
        }
    }

    private async UniTask EnlargeTextBox(string header, bool complete)
    {
        if (complete)
        {
            _requestContainer.AddToClassList("request--complete");
        }
        else
        {
            _requestContainer.AddToClassList("request--active");
        }
        
        _requestContainer.pickingMode = PickingMode.Ignore;

        _textBox.RemoveFromClassList("text-box--hover");
        _shadow.RemoveFromClassList("shadow--hover");
        _textBox.UnregisterCallback<MouseDownEvent>(OnRequestClick);
        _textBox.UnregisterCallback<MouseEnterEvent>(OnRequestHover);
        _textBox.UnregisterCallback<MouseLeaveEvent>(OnRequestHoverExit);

        _requestHeader.AddToClassList("hidden");

        await UniTask.WaitForSeconds(1);

        _requestHeader.text = header;
        _requestHeader.RemoveFromClassList("hidden");
        _requestHeader.AddToClassList("request-label--active");

        await UniTask.Delay(100);
        _horizontalLine.AddToClassList("horizontal-line--active");

        await UniTask.Delay(100);
    }

    private async void HideRequest()
    {
        _requestContainer.RemoveFromClassList("request--active");

        _requestHeader.AddToClassList("hidden");
        _requestHeader.RemoveFromClassList("request-label--active");
        _horizontalLine.RemoveFromClassList("horizontal-line--active");

        _itemList.AddToClassList("hidden");
        _textBox.Remove(_itemList);

        _closeButton.AddToClassList("hidden");

        await UniTask.Delay(700);

        _requestHeader.RemoveFromClassList("hidden");
        _requestHeader.text = "...";
        
        _closeButton.clicked -= HideRequest;
        _textBox.RegisterCallback<MouseDownEvent>(OnRequestClick);
        _textBox.RegisterCallback<MouseEnterEvent>(OnRequestHover);
        _textBox.RegisterCallback<MouseLeaveEvent>(OnRequestHoverExit);
    }

    public async void ShowThoughtBubble()
    {
        await UniTask.Delay(800);

        _requestContainer.RemoveFromClassList("hidden");

        await UniTask.Delay(200);

        _requestContainer.pickingMode = PickingMode.Position;
    }

    public async UniTask Spawn()
    {
        await UniTask.WaitForSeconds(1);

        RemoveFromClassList("hidden-left");
    }

    public async void CompleteRequest(List<Item> itemsDonated)
    {
        isComplete = true;

        if (_requestContainer.ClassListContains("request--active"))
        {
            HideRequest();
        }

        GameManager.Instance.Satisfaction += Random.Range(20, 50);

        await EnlargeTextBox("Thank you.", isComplete); // customize for good or bad score (maybe)

        _textBox.Add(_requestDescription);
        _requestDescription.text = "This is exactly what I was looking for!";
        _requestDescription.RemoveFromClassList("hidden");

        await UniTask.Delay(1200);
        _requestContainer.AddToClassList("hidden");

        await UniTask.Delay(750);
        AddToClassList("hidden-left");

        await UniTask.Delay(1000);

        PatronManager.Instance.RemovePatron();
    }

    private void GenerateRequest()
    {
        _request = new List<(SlotColor Color, PlaceableItemSO Item)>();
        int itemCount = Random.Range(1, 4); // should scale with day count

        for (int i = 0; i < itemCount; i++)
        {
            SlotColor itemColor = UIHelpers.GetRandomColor();
            PlaceableItemSO item = UIHelpers.GetRandomItem();

            _request.Add((itemColor, item));
            Debug.Log($"gimme a {itemColor} {item}");
        }
    }

    
}
 