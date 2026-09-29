using FirstOverseer.World.Objects;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace FirstOverseer.World.Inventory
{
    [Serializable]
    public class StorageContainer
    {
        public string NameKey { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }

        private InventoryObjectInstance[,] _grid;
        private List<InventoryObjectInstance> _storedItems = new List<InventoryObjectInstance>();

        public IReadOnlyList<InventoryObjectInstance> StoredItems => _storedItems;

        public event Action<InventoryObjectInstance> OnRemoveItem;
        public event Action<InventoryObjectInstance> OnAddItem;

        public StorageContainer(string title, int width, int height)
        {
            NameKey = title;
            Width = width;
            Height = height;
            _grid = new InventoryObjectInstance[width, height];
        }

        public bool CanPlaceAt(InventoryObjectInstance item, int startX, int startY)
        {
            var inventoryTrait = item.InventoryTrait;

            if (inventoryTrait == null)
                return false;

            Vector2Int size = inventoryTrait.Data.Size;

            if (startX < 0 || startY < 0 || startX + size.x > Width || startY + size.y > Height)
                return false;

            for (int x = startX; x < startX + size.x; x++)
                for (int y = startY; y < startY + size.y; y++)
                    if (_grid[x, y] != null && _grid[x, y] != item)
                        return false;

            return true;
        }

        public bool TryAutoAddItem(InventoryObjectInstance item)
        {
            if (item.InventoryTrait == null)
                return false;

            for (int y = 0; y < Height; y++)
                for (int x = 0; x < Width; x++)
                    if (CanPlaceAt(item, x, y))
                    {
                        PlaceItem(item, x, y);
                        return true;
                    }

            return false;
        }

        public void PlaceItem(InventoryObjectInstance item, int startX, int startY)
        {
            var inventoryTrait = item.InventoryTrait;
            if (inventoryTrait == null)
                return;

            RemoveItem(item);

            Vector2Int size = inventoryTrait.Data.Size;
            inventoryTrait.Position = new Vector2Int(startX, startY);

            for (int x = startX; x < startX + size.x; x++)
                for (int y = startY; y < startY + size.y; y++)
                    _grid[x, y] = item;

            if (_storedItems.Contains(item) == false)
                _storedItems.Add(item);

            OnAddItem?.Invoke(item);
        }

        public bool RemoveItem(InventoryObjectInstance item)
        {
            if (_storedItems.Remove(item) == false)
                return false;

            var trait = item.InventoryTrait;

            if (trait != null)
            {
                Vector2Int pos = trait.Position;
                Vector2Int size = trait.Data.Size;

                for (int x = pos.x; x < pos.x + size.x; x++)
                    for (int y = pos.y; y < pos.y + size.y; y++)
                        _grid[x, y] = null;
            }

            OnRemoveItem?.Invoke(item);
            return true;
        }

        public InventoryObjectInstance GetItemAt(int x, int y)
        {
            if (x < 0 || y < 0 || x >= Width || y >= Height)
                return null;

            return _grid[x, y];
        }
    }
}
