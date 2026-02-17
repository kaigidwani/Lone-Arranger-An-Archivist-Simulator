using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI;

public static class UIHelpers
{
    public static Vector2 WorldToLocalUIPosition(IPanel rootPanel, Vector2 pos)
    {
        // In UI Toolkit, the Y-axis is inverted
        pos.y = Screen.height - pos.y;

        // Convert screen position to the panel's local position
        Vector2 panelLocalPosition = RuntimePanelUtils.ScreenToPanel(rootPanel, pos);

        return panelLocalPosition;
        
    }
}
