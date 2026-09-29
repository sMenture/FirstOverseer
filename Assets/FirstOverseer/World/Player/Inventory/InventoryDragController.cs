using FirstOverseer.Global.Player.Input;
using FirstOverseer.World.Equipment;
using FirstOverseer.World.Inventory;
using FirstOverseer.World.Objects;
using FirstOverseer.World.Objects.Traits;
using FirstOverseer.World.Objects.Traits.Equipment;
using UnityEngine;
using UnityEngine.UI;

namespace FirstOverseer.World.Player.Inventory
{
    public class InventoryDragController : MonoBehaviour
    {
        [SerializeField] private ScrollRect _iventoryScrollRect;
        [SerializeField] private RectTransform _panelItem;
        [SerializeField] private StorageUI _storageUI;
        [SerializeField] private PlayerInventory _inventory;
        [SerializeField] private StoragePlacementPreviewUI _previewUI;
        [SerializeField] private EquipmentManager _equipmentManager;

        [Header("Settings")]
        [SerializeField] private InventoryThemeSO _inventoryThemmeConfig;
        [SerializeField] private InputReader _inputReader;

        private Vector2 _cursorPosition;
        private Vector2 _dragOffset;

        private Vector2Int _originalCoordinate;
        private Vector2Int _grabOffset;

        private StorageContainer _sourceContainer;
        private EquipmentType? _sourceEquipmentType;
        private StorageItemUI _draggedItemUI;
        private InventoryObjectInstance _draggedItemData;

        private StorageContainer _lastValidContainer;
        private Vector2Int _lastValidPosition;
        private EquipmentCellUI _lastValidEquipmentCell;
        private Vector3 _lastValidPreviewWorldPos;

        private EquipmentCellUI _validTargetEquipmentCell;
        private EquipmentCellUI _invalidHoveredEquipmentCell;

        private void Awake()
        {
            if (_equipmentManager == null)
                _equipmentManager = FindObjectOfType<EquipmentManager>();
        }

        private void OnEnable()
        {
            if (_storageUI != null)
            {
                _storageUI.OnOpen += HandleInventoryOpen;
                _storageUI.OnClose += HandleInventoryClose;
            }
        }

        private void OnDisable()
        {
            if (_storageUI != null)
            {
                _storageUI.OnOpen -= HandleInventoryOpen;
                _storageUI.OnClose -= HandleInventoryClose;
            }
        }

        private void HandleInventoryOpen()
        {
            if (_inputReader != null)
                _inputReader.MouseDown += TryStartDrag;
        }

        private void HandleInventoryClose()
        {
            if (_inputReader != null)
            {
                _inputReader.MouseDown -= TryStartDrag;
                _inputReader.MouseUp -= StopDrag;
            }
        }

        private void TryStartDrag()
        {
            if (_inputReader != null)
                _inputReader.Point += UpdateCursor;

            if (_storageUI == null)
                return;

            if (_storageUI.TryGetCellUnderCursor(_cursorPosition, out var container, out int col, out int row))
            {
                if (container != null && _storageUI.TryGetItemUI(container, col, row, out var itemUI))
                {
                    var itemData = container.GetItemAt(col, row);
                    if (itemData == null || itemData.InventoryTrait == null)
                        return;

                    _sourceContainer = container;
                    _sourceEquipmentType = null;

                    _draggedItemUI = itemUI;
                    _draggedItemData = itemData;

                    var trait = _draggedItemData.InventoryTrait;
                    _originalCoordinate = trait.Position;
                    _grabOffset = new Vector2Int(col - trait.Position.x, row - trait.Position.y);

                    _draggedItemUI.RestoreStandardState();
                    _dragOffset = (Vector2)_draggedItemUI.RectTransform.position - _cursorPosition;

                    StartDraggingUIItem();

                    _lastValidContainer = container;
                    _lastValidPosition = trait.Position;
                    _lastValidPreviewWorldPos = _draggedItemUI.RectTransform.position;

                    HighlightTargetEquipmentSlot();
                }
            }
            else if (_storageUI.TryGetEquipmentCellUnderCursor(_cursorPosition, out var equipmentCell))
            {
                if (equipmentCell != null && equipmentCell.CurrentItemUI != null && _equipmentManager != null)
                {
                    var equippedData = _equipmentManager.GetEquippedObject(equipmentCell.SlotType);
                    if (equippedData == null)
                        return;

                    _sourceContainer = null;
                    _sourceEquipmentType = equipmentCell.SlotType;
                    _grabOffset = Vector2Int.zero;

                    _draggedItemUI = equipmentCell.CurrentItemUI;
                    _draggedItemData = equippedData;

                    _draggedItemUI.RestoreStandardState();
                    _dragOffset = (Vector2)_draggedItemUI.RectTransform.position - _cursorPosition;

                    StartDraggingUIItem();

                    _lastValidEquipmentCell = equipmentCell;

                    HighlightTargetEquipmentSlot();
                }
            }
        }

        private void HighlightTargetEquipmentSlot()
        {
            var equipTrait = _draggedItemData.GetTrait<EquipmentTraitInstance>();
            if (equipTrait != null && _storageUI != null && _inventoryThemmeConfig != null)
            {
                foreach (var cell in _storageUI.ActiveEquipmentCells)
                {
                    if (cell.SlotType == equipTrait.Data.EquipmentType)
                    {
                        _validTargetEquipmentCell = cell;
                        _validTargetEquipmentCell.SetHighlight(_inventoryThemmeConfig.AccentColor);
                        break;
                    }
                }
            }
        }

        private void StartDraggingUIItem()
        {
            if (_iventoryScrollRect != null)
                _iventoryScrollRect.enabled = false;

            if (_draggedItemUI != null)
            {
                _draggedItemUI.ApplyDraggingVisual();

                if (_panelItem != null)
                    _draggedItemUI.RectTransform.SetParent(_panelItem);
            }

            if (_inputReader != null)
            {
                _inputReader.Point += UpdateDraggedItemPosition;
                _inputReader.MouseUp += StopDrag;
            }
        }

        private void StopDrag()
        {
            bool placed = false;

            if (_lastValidContainer != null)
            {
                if (_sourceContainer != null)
                {
                    placed = _inventory.TryTransferItem(
                        _draggedItemData,
                        _sourceContainer,
                        _lastValidContainer,
                        _lastValidPosition.x,
                        _lastValidPosition.y
                    );
                }
                else if (_sourceEquipmentType.HasValue)
                {
                    if (_lastValidContainer.CanPlaceAt(_draggedItemData, _lastValidPosition.x, _lastValidPosition.y))
                    {
                        _equipmentManager.TryUnequip(_sourceEquipmentType.Value);
                        _lastValidContainer.PlaceItem(_draggedItemData, _lastValidPosition.x, _lastValidPosition.y);
                        placed = true;
                    }
                }
            }
            else if (_lastValidEquipmentCell != null)
            {
                if (_sourceContainer != null)
                {
                    var equippedItem = _equipmentManager.GetEquippedObject(_lastValidEquipmentCell.SlotType);

                    _sourceContainer.RemoveItem(_draggedItemData);

                    if (equippedItem != null)
                    {
                        if (_sourceContainer.CanPlaceAt(equippedItem, _originalCoordinate.x, _originalCoordinate.y))
                        {
                            _equipmentManager.TryEquip(_draggedItemData);
                            _sourceContainer.PlaceItem(equippedItem, _originalCoordinate.x, _originalCoordinate.y);
                            placed = true;
                        }
                        else if (_sourceContainer.TryAutoAddItem(equippedItem))
                        {
                            _equipmentManager.TryEquip(_draggedItemData);
                            placed = true;
                        }
                        else
                        {
                            _sourceContainer.PlaceItem(_draggedItemData, _originalCoordinate.x, _originalCoordinate.y);
                        }
                    }
                    else
                    {
                        if (_equipmentManager.TryEquip(_draggedItemData))
                            placed = true;
                        else
                            _sourceContainer.PlaceItem(_draggedItemData, _originalCoordinate.x, _originalCoordinate.y);
                    }
                }
                else if (_sourceEquipmentType.HasValue)
                {
                    if (_sourceEquipmentType.Value == _lastValidEquipmentCell.SlotType)
                        placed = false;
                }
            }

            if (placed == false)
            {
                if (_sourceContainer != null)
                    _sourceContainer.PlaceItem(_draggedItemData, _originalCoordinate.x, _originalCoordinate.y);
            }

            CleanupDrag();

            if (_storageUI != null && _inventory != null)
                _storageUI.RenderStorages(_inventory.GetAllActiveStorages());
        }

        private void CleanupDrag()
        {
            if (_draggedItemUI != null)
                _draggedItemUI.RestoreVisual();

            if (_inputReader != null)
            {
                _inputReader.Point -= UpdateCursor;
                _inputReader.Point -= UpdateDraggedItemPosition;
                _inputReader.MouseUp -= StopDrag;
            }

            if (_iventoryScrollRect != null)
                _iventoryScrollRect.enabled = true;

            if (_validTargetEquipmentCell != null)
            {
                _validTargetEquipmentCell.ClearHighlight();
                _validTargetEquipmentCell = null;
            }

            if (_invalidHoveredEquipmentCell != null)
            {
                _invalidHoveredEquipmentCell.ClearHighlight();
                _invalidHoveredEquipmentCell = null;
            }

            _draggedItemUI = null;
            _draggedItemData = null;
            _sourceContainer = null;
            _sourceEquipmentType = null;
            _lastValidEquipmentCell = null;

            if (_previewUI != null)
                _previewUI.Disable();
        }

        private void UpdateCursor(Vector2 pos) => _cursorPosition = pos;

        private void UpdateDraggedItemPosition(Vector2 pos)
        {
            if (_draggedItemUI != null && _draggedItemUI.RectTransform != null)
                _draggedItemUI.RectTransform.position = pos + _dragOffset;

            UpdatePreview(pos);
        }

        private void UpdatePreview(Vector2 cursorPos)
        {
            if (_draggedItemData == null || _draggedItemUI == null)
                return;

            Vector3 followPos = _draggedItemUI.RectTransform.position;
            _lastValidContainer = null;
            _lastValidEquipmentCell = null;

            if (_invalidHoveredEquipmentCell != null)
            {
                _invalidHoveredEquipmentCell.ClearHighlight();
                _invalidHoveredEquipmentCell = null;
            }

            if (_storageUI.TryGetCellUnderCursor(cursorPos, out var container, out int col, out int row))
            {
                int placeX = col - _grabOffset.x;
                int placeY = row - _grabOffset.y;

                var trait = _draggedItemData.InventoryTrait;
                if (trait == null)
                    return;

                Vector2Int size = trait.Data.Size;

                if (placeX < 0 || placeY < 0 || placeX + size.x > container.Width || placeY + size.y > container.Height)
                {
                    if (_previewUI != null)
                        _previewUI.Disable();
                    return;
                }

                bool canPlace = container.CanPlaceAt(_draggedItemData, placeX, placeY);

                StorageCategoryUI categoryUI = null;
                foreach (var cat in _storageUI.ActiveCategories)
                {
                    if (cat.Container == container)
                    {
                        categoryUI = cat;
                        break;
                    }
                }

                if (canPlace && categoryUI != null)
                {
                    int endX = placeX + size.x - 1;
                    int endY = placeY + size.y - 1;

                    int topLeftIndex = placeY * container.Width + placeX;
                    int bottomRightIndex = endY * container.Width + endX;

                    RectTransform topLeftCell = categoryUI.GetCellRectTransform(topLeftIndex);
                    RectTransform bottomRightCell = categoryUI.GetCellRectTransform(bottomRightIndex);

                    if (topLeftCell != null && bottomRightCell != null)
                    {
                        Vector3 posTopLeft = topLeftCell.position;
                        Vector3 posBottomRight = bottomRightCell.position;
                        Vector3 middlePosition = (posTopLeft + posBottomRight) / 2f;

                        if (_previewUI != null)
                        {
                            _previewUI.Enable();
                            _previewUI.SetColor(_inventoryThemmeConfig.AccentColor);
                            _previewUI.SetSize(_draggedItemUI.RectTransform.rect.size);
                            _previewUI.ShowPreview(middlePosition, true);
                        }

                        _lastValidContainer = container;
                        _lastValidPosition = new Vector2Int(placeX, placeY);
                        _lastValidPreviewWorldPos = middlePosition;

                        return;
                    }
                }

                if (_previewUI != null)
                {
                    _previewUI.Enable();
                    _previewUI.SetColor(_inventoryThemmeConfig.WarningColor);
                    _previewUI.SetSize(_draggedItemUI.RectTransform.rect.size);
                    _previewUI.ShowPreview(followPos, false);
                }
                return;
            }
            else if (_storageUI.TryGetEquipmentCellUnderCursor(cursorPos, out var equipmentCell))
            {
                var equipTrait = _draggedItemData.GetTrait<EquipmentTraitInstance>();

                if (equipTrait != null && equipTrait.Data.EquipmentType == equipmentCell.SlotType)
                {
                    _lastValidEquipmentCell = equipmentCell;
                    if (_previewUI != null) _previewUI.Disable();
                    return;
                }
                else
                {
                    _invalidHoveredEquipmentCell = equipmentCell;
                    _invalidHoveredEquipmentCell.SetHighlight(_inventoryThemmeConfig.WarningColor);

                    if (_previewUI != null) _previewUI.Disable();
                    return;
                }
            }

            if (_previewUI != null)
                _previewUI.Disable();
        }
    }
}