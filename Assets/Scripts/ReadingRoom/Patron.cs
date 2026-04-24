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
    private Label _requestLabel;
    private VisualElement _itemList;
    private Button _closeButton;

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

        _requestLabel = new Label();
        _requestLabel.AddToClassList("request-label");
        _requestLabel.text = "...";
        _textBox.Add(_requestLabel);

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
        _requestContainer.AddToClassList("request--active");
        _requestContainer.pickingMode = PickingMode.Ignore;

        _textBox.RemoveFromClassList("text-box--hover");
        _shadow.RemoveFromClassList("shadow--hover");
        _textBox.UnregisterCallback<MouseDownEvent>(OnRequestClick);
        _textBox.UnregisterCallback<MouseEnterEvent>(OnRequestHover);
        _textBox.UnregisterCallback<MouseLeaveEvent>(OnRequestHoverExit);

        _requestLabel.AddToClassList("hidden");

        await DisplayRequest();
        await UniTask.WaitForSeconds(1);

        _closeButton.RemoveFromClassList("hidden");
        _closeButton.clicked += HideRequest;
    }

    private async UniTask DisplayRequest()
    {
        await UniTask.WaitForSeconds(1);

        _requestLabel.text = "I'm looking for...";
        _requestLabel.RemoveFromClassList("hidden");
        _requestLabel.AddToClassList("request-label--active");

        await UniTask.Delay(100);
        _horizontalLine.AddToClassList("horizontal-line--active");

        await UniTask.Delay(100);

        _itemList.RemoveFromClassList("hidden");
        _textBox.Add(_itemList);

        /*if (_request == null)
        {
            GenerateRequest();
        }
        else 
        {

        }*/

        if (_request != null)
        {
            _request = null;

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

        GenerateRequest();
      
        for (int i = 0; i < _request.Count; i++)
        {
            VisualElement itemContainer = new VisualElement();
            itemContainer.AddToClassList("item-display-bg");
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

            /*for (int row = 0; row < so.BaseHeight; row++)
            {
                Debug.Log(so.BaseHeight);
                for (int col = 0; col < so.BaseWidth; col++)
                {

                    Debug.Log(so.BaseWidth);
                    /* Empty parts of the shape don't get "made"
                    if (so.BaseShape[row][col] == 0)
                    {
                        continue;
                    }

                    VisualElement tile = new VisualElement();
                    tile.style.width = tileWidth;
                    tile.style.height = tileHeight;
                    tile.style.left = col * tileWidth.value.value;
                    tile.style.top = row * tileHeight.value.value;

                    itemVisual.Add(tile);

                    tile.style.backgroundColor = so.COLOR_CODES[(int)_request[i].Color];
                }
            }*/

            itemContainer.Add(itemVisual);
            _itemList.Add(itemContainer);

            await UniTask.Delay(450);
            itemContainer.RemoveFromClassList("hidden");
        }
    }

    private async void HideRequest()
    {
        _requestContainer.RemoveFromClassList("request--active");

        _requestLabel.AddToClassList("hidden");
        _requestLabel.RemoveFromClassList("request-label--active");
        _horizontalLine.RemoveFromClassList("horizontal-line--active");

        _itemList.AddToClassList("hidden");
        _textBox.Remove(_itemList);

        _closeButton.AddToClassList("hidden");

        await UniTask.Delay(700);

        _requestLabel.RemoveFromClassList("hidden");
        _requestLabel.text = "...";
        
        _closeButton.clicked -= HideRequest;
        _textBox.RegisterCallback<MouseDownEvent>(OnRequestClick);
        _textBox.RegisterCallback<MouseEnterEvent>(OnRequestHover);
        _textBox.RegisterCallback<MouseLeaveEvent>(OnRequestHoverExit);
    }

    public async UniTask Spawn()
    {
        await UniTask.WaitForSeconds(1);

        RemoveFromClassList("hidden-left");

        await UniTask.Delay(800);

        _requestContainer.RemoveFromClassList("hidden");

        await UniTask.Delay(200);

        _requestContainer.pickingMode = PickingMode.Position;

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
 