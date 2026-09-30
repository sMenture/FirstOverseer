using FirstOverseer.Global.Player.Input;
using FirstOverseer.World.Inventory;
using FirstOverseer.World.Objects;
using FirstOverseer.World.Objects.Traits;
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

        [Header("Settings")]
        [SerializeField] private InventoryThemeSO _inventoryThemmeConfig;
        [SerializeField] private InputReader _inputReader;

        private Vector2 _cursorPosition;
        private Vector2 _dragOffset;

        private Vector2Int _originalCoordinate;
        private Vector2Int _grabOffset;

        private StorageContainer _sourceContainer;
        private StorageItemUI _draggedItemUI;
        private InventoryObjectInstance _draggedItemData;

        private StorageContainer _lastValidContainer;
        private Vector2Int _lastValidPosition;
        private Vector3 _lastValidPreviewWorldPos;

        private void OnEnable()
        {
            _storageUI.OnOpen += HandleInventoryOpen;
            _storageUI.OnClose += HandleInventoryClose;
        }

        private void OnDisable()
        {
            _storageUI.OnOpen -= HandleInventoryOpen;
            _storageUI.OnClose -= HandleInventoryClose;
        }

        private void HandleInventoryOpen()
        {
            _inputReader.MouseDown += TryStartDrag;
        }

        private void HandleInventoryClose()
        {
            _inputReader.MouseDown -= TryStartDrag;
            _inputReader.MouseUp -= StopDrag;
        }

        private void TryStartDrag()
        {
            _inputReader.Point += UpdateCursor;

            if (_storageUI.TryGetCellUnderCursor(_cursorPosition, out var container, out int col, out int row))
            {
                if (container != null && _storageUI.TryGetItemUI(container, col, row, out var itemUI))
                {
                    _sourceContainer = container;
                    _draggedItemUI = itemUI;
                    _draggedItemData = container.GetItemAt(col, row);

                    var trait = _draggedItemData.InventoryTrait;
                    _originalCoordinate = trait.Position;

                    _grabOffset = new Vector2Int(col - trait.Position.x, row - trait.Position.y);
                    _dragOffset = (Vector2)_draggedItemUI.RectTransform.position - _cursorPosition;

                    StartDraggingUIItem();

                    _previewUI.Enable();
                    _previewUI.SetSize(_draggedItemUI.RectTransform.rect.size);

                    _lastValidContainer = container;
                    _lastValidPosition = trait.Position;
                    _lastValidPreviewWorldPos = _draggedItemUI.RectTransform.position;
                }
            }
        }

        private void StartDraggingUIItem()
        {
            _iventoryScrollRect.enabled = false;

            _draggedItemUI.ApplyDraggingVisual();
            _draggedItemUI.RectTransform.SetParent(_panelItem);

            _inputReader.Point += UpdateDraggedItemPosition;
            _inputReader.MouseUp += StopDrag;
        }

        private void StopDrag()
        {
            bool placed = false;

            if (_lastValidContainer != null)
            {
                placed = _inventory.TryTransferItem(
                    _draggedItemData,
                    _sourceContainer,
                    _lastValidContainer,
                    _lastValidPosition.x,
                    _lastValidPosition.y
                );
            }

            if (placed == false)
                _sourceContainer.PlaceItem(_draggedItemData, _originalCoordinate.x, _originalCoordinate.y);

            CleanupDrag();
        }

        private void CleanupDrag()
        {
            if (_draggedItemUI != null)
                _draggedItemUI.RestoreVisual();

            _inputReader.Point -= UpdateCursor;
            _inputReader.Point -= UpdateDraggedItemPosition;
            _inputReader.MouseUp -= StopDrag;

            _iventoryScrollRect.enabled = true;

            _draggedItemUI = null;
            _draggedItemData = null;
            _sourceContainer = null;

            _previewUI.Disable();
        }

        private void UpdateCursor(Vector2 pos) => _cursorPosition = pos;

        private void UpdateDraggedItemPosition(Vector2 pos)
        {
            if (_draggedItemUI != null)
                _draggedItemUI.RectTransform.position = pos + _dragOffset;

            UpdatePreview(pos);
        }

        private void UpdatePreview(Vector2 cursorPos)
        {
            if (_draggedItemData == null || _draggedItemUI == null)
                return;

            Vector3 followPos = _draggedItemUI.RectTransform.position;

            if (_storageUI.TryGetCellUnderCursor(cursorPos, out var container, out int col, out int row))
            {
                int placeX = col - _grabOffset.x;
                int placeY = row - _grabOffset.y;

                var trait = _draggedItemData.InventoryTrait;
                Vector2Int size = trait.Data.Size;

                if (placeX < 0 || placeY < 0 || placeX + size.x > container.Width || placeY + size.y > container.Height)
                {
                    _previewUI.Disable();
                    return;
                }

                bool canPlace = container.CanPlaceAt(_draggedItemData, placeX, placeY);

                StorageCategoryUI categoryUI = null;
                foreach (var cat in _storageUI.ActiveCategories)
                    if (cat.Container == container)
                    {
                        categoryUI = cat;
                        break;
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

                        _previewUI.Enable();
                        _previewUI.SetColor(_inventoryThemmeConfig.AccentColor);
                        _previewUI.ShowPreview(middlePosition, true);

                        _lastValidContainer = container;
                        _lastValidPosition = new Vector2Int(placeX, placeY);
                        _lastValidPreviewWorldPos = middlePosition;

                        return;
                    }
                }

                _previewUI.Enable();
                _previewUI.SetColor(_inventoryThemmeConfig.WarningColor);
                _previewUI.ShowPreview(followPos, false);
                return;
            }

            _previewUI.Disable();
        }
    }
}
