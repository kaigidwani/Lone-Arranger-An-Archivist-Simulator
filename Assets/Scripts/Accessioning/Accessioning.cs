using System;
using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class Accessioning : VisualElement
{
    // Fields
    
    private float _paddingLeft;
    private float _paddingRight;
    private float _paddingTop;
    private float _paddingBottom;
    private Vector2 _boxSize;

    // Properties

    /// <summary>
    /// Minimimum position of this element (x=left, y=top)
    /// </summary>
    public Vector2 Min
    {
        get 
        {
            return new Vector2(_paddingLeft, _paddingTop);
        }
    }

    /// <summary>
    /// Maximum position of this element (top-right)
    /// </summary>
    public Vector2 Max
    {
        get
        {
            return new Vector2(
                Min.x + _boxSize.x,
                Min.y + _boxSize.y);
        }
    }

    public void GetDimensions()
    {
        _paddingLeft = resolvedStyle.paddingLeft;
        _paddingRight = resolvedStyle.paddingLeft;
        _paddingTop = resolvedStyle.paddingLeft;
        _paddingBottom = resolvedStyle.paddingLeft;

        _boxSize = new Vector2(
            resolvedStyle.width - resolvedStyle.borderLeftWidth - resolvedStyle.borderRightWidth - _paddingLeft - _paddingRight,
            resolvedStyle.height - resolvedStyle.borderTopWidth - resolvedStyle.borderBottomWidth - _paddingTop - _paddingBottom);
    }

    public bool TryGetDimensions()
    {
        return resolvedStyle.width > 0;
    }
}
