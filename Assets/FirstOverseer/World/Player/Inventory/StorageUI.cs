using FirstOverseer.Global.Localization;
using FirstOverseer.World.Equipment;
using FirstOverseer.World.Inventory;
using FirstOverseer.World.Objects;
using FirstOverseer.World.Player.Inventory.Pool;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace FirstOverseer.World.Player.Inventory
{
    public class StorageUI : MonoBehaviour
    {
        [SerializeField] private Canvas _inventoryCanvas;
        [SerializeField] private RectTransform _body;
        [SerializeField] private RectTransform _gridContainer;
        [SerializeField] private RectTransform _equipmentContainer;

        [Header("Pools")]
        [SerializeField] private StorageCategoryUIPool _categoryPool;
        [SerializeField] private StorageCellUIPool _cellPool;
        [SerializeField] private StorageItemUIPool _itemPool;
        [SerializeField] private StorageEquipmentUIPool _equipmentPool;

        [Header("Other")]
        [SerializeField] private LocalizationSO _localization;
        [SerializeField] private EquipmentManager _equipmentManager;

        private readonly List<StorageCategoryUI> _activeCategories = new List<StorageCategoryUI>();
        private readonly List<EquipmentCellUI> _activeEquipmentCells = new List<EquipmentCellUI>();

        public IReadOnlyList<StorageCategoryUI> ActiveCategories => _activeCategories;
        public IReadOnlyList<EquipmentCellUI> ActiveEquipmentCells => _activeEquipmentCells;

        public event Action OnOpen;
        public event Action OnClose;

        public void RenderStorages(List<StorageContainer> containers)
        {
            OnOpen?.Invoke();
            _inventoryCanvas.enabled = true;

            ClearStorages();
            RenderEquipmentSlots();

            for (int i = 0; i < containers.Count; i++)
            {
                var container = containers[i];

                var categoryUI = _categoryPool.GiveElement();
                categoryUI.transform.SetParent(_gridContainer, false);
                categoryUI.transform.SetSiblingIndex(i);
                categoryUI.Render(container, _cellPool, _itemPool);

                if (_localization != null)
                    categoryUI.SetName(_localization.GetText(container.NameKey));

                _activeCategories.Add(categoryUI);
            }

            Canvas.ForceUpdateCanvases();
            UpdateBodySize();
        }

        private void RenderEquipmentSlots()
        {
            ClearEquipmentSlots();

            if (_equipmentManager == null)
                return;

            var equipmentTypes = (Objects.Traits.Equipment.EquipmentType[])Enum.GetValues(typeof(Objects.Traits.Equipment.EquipmentType));

            foreach (var type in equipmentTypes)
            {
                var cell = _equipmentPool.GiveElement();
                cell.RectTransform.SetParent(_equipmentContainer, false);
                cell.Initialize(type);

                _activeEquipmentCells.Add(cell);

                var itemInstance = _equipmentManager.GetEquippedObject(type);
                if (itemInstance != null)
                {
                    var itemUI = _itemPool.GiveElement();
                    itemUI.ApplyVisual(itemInstance);
                    cell.SetItem(itemUI);
                }
            }
        }

        private void UpdateBodySize()
        {
            float gridWidth = _gridContainer.rect.width;

            if (gridWidth >= 10)
            {
                float newWidth = gridWidth * 1.07f;
                _body.sizeDelta = new Vector2(newWidth, _body.sizeDelta.y);
            }
        }

        public void Hide()
        {
            OnClose?.Invoke();
            ClearStorages();
            ClearEquipmentSlots();

            _inventoryCanvas.enabled = false;
        }

        private void ClearStorages()
        {
            foreach (var category in _activeCategories)
            {
                category.ClearCellPool(_cellPool);
                _categoryPool.ReturnToPool(category);
            }

            _activeCategories.Clear();
        }

        private void ClearEquipmentSlots()
        {
            foreach (var cell in _activeEquipmentCells)
            {
                if (cell.CurrentItemUI != null)
                {
                    _itemPool.ReturnToPool(cell.CurrentItemUI);
                    cell.ClearItem();
                }
                _equipmentPool.ReturnToPool(cell);
            }

            _activeEquipmentCells.Clear();
        }

        public bool TryGetCellUnderCursor(Vector2 screenPosition, out StorageContainer container, out int col, out int row)
        {
            container = null;
            col = -1;
            row = -1;

            foreach (var categoryUI in _activeCategories)
            {
                if (categoryUI.TryGetCellAtPosition(screenPosition, out col, out row, out _))
                {
                    container = categoryUI.Container;
                    return true;
                }
            }

            return false;
        }

        public bool TryGetEquipmentCellUnderCursor(Vector2 screenPosition, out EquipmentCellUI equipmentCell)
        {
            equipmentCell = null;

            foreach (var cell in _activeEquipmentCells)
            {
                if (RectTransformUtility.RectangleContainsScreenPoint(cell.RectTransform, screenPosition, null))
                {
                    equipmentCell = cell;
                    return true;
                }
            }

            return false;
        }

        public bool TryGetItemUI(StorageContainer container, int col, int row, out StorageItemUI itemUI)
        {
            foreach (var categoryUI in _activeCategories)
            {
                if (categoryUI.Container == container)
                    return categoryUI.TryGetItemAtCell(col, row, out itemUI);
            }

            itemUI = null;
            return false;
        }
    }
}