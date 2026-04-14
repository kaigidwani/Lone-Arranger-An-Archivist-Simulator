using System;
using UnityEngine;

public static class UIHelpers
{
    public static Vector2 SetItemPivotToMouse(ItemTile pivot, Vector2 mousePos)
    {
        float tileWidth = InventoryController.Instance.ItemTileSize.x;
        float tileHeight = InventoryController.Instance.ItemTileSize.y;

        // Find offset from pivot
        float rowOffset = tileHeight * pivot.Index.x;
        float colOffset = tileWidth * pivot.Index.y;

        float drawOffset = 0;
        Item item = pivot.ParentItem;
        if (pivot.ParentItem.Rotation % 180 != 0)
        {
            drawOffset = (item.WidthInTiles - item.HeightInTiles) * tileWidth / 2;
        }

        return new Vector2((mousePos.x - colOffset - tileWidth * 0.5f) + drawOffset,
            (mousePos.y - rowOffset - tileHeight * 0.5f) - drawOffset);
    }

    public static SlotColor GetRandomColor()
    {
        int rand = UnityEngine.Random.Range(0, Enum.GetNames(typeof(SlotColor)).Length);

        return (SlotColor)rand;
    }

    public static PlaceableItemSO GetRandomItem()
    {
        int rand = UnityEngine.Random.Range(0, InventoryController.Instance.ItemPool.Length);

        return InventoryController.Instance.ItemPool[rand];
    }
}
 