using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

[UxmlElement]
public partial class Accessioning : VisualElement
{
    // Properties

    /// <summary>
    /// Minimimum position of this element (x=left, y=top)
    /// </summary>
    public Vector2 Min { get; private set; }

    /// <summary>
    /// Maximum position of this element (top-right)
    /// </summary>
    public Vector2 Max { get; private set; }

    public Accessioning()
    {
        pickingMode = PickingMode.Ignore;
    }

    public void GetDimensions()
    {
        Min = new Vector2(resolvedStyle.borderLeftWidth, resolvedStyle.borderTopWidth);
        Max = new Vector2(worldBound.width - resolvedStyle.borderRightWidth,
            worldBound.height - resolvedStyle.borderBottomWidth);
        Debug.Log("Min:" + Min);
        Debug.Log("Max:" + Max);
    }

    public bool TryGetDimensions()
    {
        return worldBound.width > 0;
    }

    public Vector2 GetRandomPoint(float offsetX, float offsetY)
    {
        float x = Random.Range(Min.x, Max.x - offsetX);
        float y = Random.Range(Min.y, Max.y - offsetY);
        return new Vector2(x, y);
    }
}
