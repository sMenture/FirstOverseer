using FirstOverseer.World.Inventory;
using FirstOverseer.World.Objects;
using FirstOverseer.World.Objects.Traits;
using FirstOverseer.World.Player.Inventory.Pool;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FirstOverseer.World.Player.Inventory
{
    public class StorageCategoryUI : MonoBehaviour
    {
        [SerializeField] private Transform _container;
        [SerializeField] private GridLayoutGroup _grid;
        [SerializeField] private TextMeshProUGUI _titleText;
        [SerializeField] private RectTransform _itemOverlay;

        private readonly List<StorageCellUI> _activeCells = new List<StorageCellUI>();
        private readonly List<StorageItemUI> _activeItems = new List<StorageItemUI>();
        private readonly Dictionary<InventoryObjectInstance, StorageItemUI> _itemToUIMap = new Dictionary<InventoryObjectInstance, StorageItemUI>();

        private int _currentWidth;
        private StorageItemUIPool _itemPool;

        public StorageContainer Container { get; private set; }

        private void OnDestroy() => UnsubscribeEvents();

        public void Render(StorageContainer container, StorageCellUIPool cellPool, StorageItemUIPool itemPool)
        {
            UnsubscribeEvents();

            Container = container;
            _itemPool = itemPool;

            ClearCellPool(cellPool);
            _currentWidth = container.Width;

            if (_grid != null)
            {
                _grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
                _grid.constraintCount = container.Width;
            }

            int totalCells = container.Width * container.Height;
            Transform parent = _container != null ? _container : transform;

            for (int i = 0; i < totalCells; i++)
            {
                var cell = cellPool.GiveElement();
                cell.transform.SetParent(parent, false);
                _activeCells.Add(cell);
            }

            RenderItems();

            Container.OnAddItem += HandleItemAdded;
            Container.OnRemoveItem += HandleItemRemoved;
        }

        public void PlaceItem(StorageItemUI item, int startX, int startY, int itemWidth, int itemHeight)
        {
            int endX = startX + itemWidth - 1;
            int endY = startY + itemHeight - 1;

            int topLeftIndex = startY * _currentWidth + startX;
            int bottomRightIndex = endY * _currentWidth + endX;

            if (topLeftIndex < 0 || bottomRightIndex >= _activeCells.Count)
                return;

            Vector3 posTopLeft = _activeCells[topLeftIndex].RectTransform.localPosition;
            Vector3 posBottomRight = _activeCells[bottomRightIndex].RectTransform.localPosition;

            Vector3 middlePosition = (posTopLeft + posBottomRight) / 2f;

            Vector2 cellSize = _grid.cellSize;
            Vector2 spacing = _grid.spacing;

            float width = itemWidth * cellSize.x + (itemWidth - 1) * spacing.x;
            float height = itemHeight * cellSize.y + (itemHeight - 1) * spacing.y;

            item.RectTransform.SetParent(_itemOverlay, false);
            item.RectTransform.anchoredPosition = middlePosition;
            item.RectTransform.sizeDelta = new Vector2(width, height);
        }

        private void UnsubscribeEvents()
        {
            if (Container != null)
            {
                Container.OnAddItem -= HandleItemAdded;
                Container.OnRemoveItem -= HandleItemRemoved;
            }
        }

        private void RenderItems()
        {
            ClearItemPool(_itemPool);
            _itemToUIMap.Clear();

            Canvas.ForceUpdateCanvases();
            _grid.CalculateLayoutInputHorizontal();
            _grid.CalculateLayoutInputVertical();
            _grid.SetLayoutHorizontal();
            _grid.SetLayoutVertical();

            foreach (var itemInstance in Container.StoredItems)
                HandleItemAdded(itemInstance);
        }

        private void HandleItemAdded(InventoryObjectInstance itemInstance)
        {
            if (_itemToUIMap.ContainsKey(itemInstance))
                return;

            var trait = itemInstance.GetTrait<InventoryItemTraitInstance>();
            if (trait == null)
                return;

            var itemUI = _itemPool.GiveElement();
            _activeItems.Add(itemUI);
            _itemToUIMap[itemInstance] = itemUI;

            itemUI.ApplyVisual(itemInstance);

            PlaceItem(itemUI, trait.Position.x, trait.Position.y, trait.Data.Size.x, trait.Data.Size.y);
        }

        private void HandleItemRemoved(InventoryObjectInstance itemInstance)
        {
            if (_itemToUIMap.TryGetValue(itemInstance, out var itemUI))
            {
                _activeItems.Remove(itemUI);
                _itemToUIMap.Remove(itemInstance);

                _itemPool.ReturnToPool(itemUI);
            }
        }

        public bool TryGetItemAtCell(int col, int row, out StorageItemUI itemUI)
        {
            itemUI = null;

            foreach (var kvp in _itemToUIMap)
            {
                var item = kvp.Key;
                var trait = item.InventoryTrait;

                Vector2Int pos = trait.Position;
                Vector2Int size = trait.Data.Size;

                if (col >= pos.x && col < pos.x + size.x && row >= pos.y && row < pos.y + size.y)
                {
                    itemUI = kvp.Value;
                    return true;
                }
            }

            return false;
        }

        public bool TryGetCellAtPosition(Vector2 screenPosition, out int col, out int row, out int cellIndex)
        {
            col = -1;
            row = -1;
            cellIndex = -1;

            if (_currentWidth <= 0 || _activeCells.Count == 0)
                return false;

            for (int i = 0; i < _activeCells.Count; i++)
            {
                var cell = _activeCells[i];

                if (RectTransformUtility.RectangleContainsScreenPoint(cell.RectTransform, screenPosition, null))
                {
                    cellIndex = i;
                    col = i % _currentWidth;
                    row = i / _currentWidth;
                    return true;
                }
            }

            return false;
        }

        public void ClearCellPool(StorageCellUIPool pool)
        {
            foreach (var cell in _activeCells)
                pool.ReturnToPool(cell);

            _activeCells.Clear();
        }

        public void ClearItemPool(StorageItemUIPool pool)
        {
            foreach (var item in _activeItems)
                pool.ReturnToPool(item);

            _activeItems.Clear();
            _itemToUIMap.Clear();
        }

        public RectTransform GetCellRectTransform(int index) => index < 0 || index >= _activeCells.Count ? null : _activeCells[index].RectTransform;

        public void SetName(string name) => _titleText.text = name;
    }
}
