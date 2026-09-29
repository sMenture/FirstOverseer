using FirstOverseer.World.Objects;
using FirstOverseer.World.Objects.Traits;
using UnityEngine;
using UnityEngine.UI;

namespace FirstOverseer.World.Player.Inventory
{
    public class StorageItemUI : MonoBehaviour
    {
        [field: SerializeField] public RectTransform RectTransform { get; private set; }

        [SerializeField] private Image _background;
        [SerializeField] private Image _icon;

        [Header("Size Settings")]
        [SerializeField] private Vector2 _baseCellSize = new Vector2(100f, 100f);
        [SerializeField] private Vector2 _cellSpacing = new Vector2(5f, 5f);

        private Vector2 _standardSize;

        private bool _isStateSaved;
        private Vector2 _defaultAnchorMin;
        private Vector2 _defaultAnchorMax;
        private Vector2 _defaultPivot;
        private Vector2 _defaultSizeDelta;

        private void SaveState()
        {
            if (!_isStateSaved)
            {
                _defaultAnchorMin = RectTransform.anchorMin;
                _defaultAnchorMax = RectTransform.anchorMax;
                _defaultPivot = RectTransform.pivot;
                _defaultSizeDelta = RectTransform.sizeDelta;
                _isStateSaved = true;
            }
        }

        public void ApplyVisual(WorldObjectInstance instance)
        {
            SaveState();

            var rarity = instance.Data.ItemRaritySO;

            _icon.sprite = instance.Data.Sprite;

            if (rarity != null)
                _background.color = rarity.BackgroundColor;
            else
                _background.color = Color.clear;

            var c = _icon.color;
            c.a = 1f;
            _icon.color = c;

            var inventoryTrait = instance.GetTrait<InventoryItemTraitInstance>();
            if (inventoryTrait != null)
            {
                var size = inventoryTrait.Data.Size;
                _standardSize = new Vector2(
                    size.x * _baseCellSize.x + Mathf.Max(0, size.x - 1) * _cellSpacing.x,
                    size.y * _baseCellSize.y + Mathf.Max(0, size.y - 1) * _cellSpacing.y
                );

                RestoreStandardState();
            }
        }

        public void ApplyEquippedState()
        {
            SaveState();
            RectTransform.anchorMin = Vector2.zero;
            RectTransform.anchorMax = Vector2.one;
            RectTransform.offsetMin = Vector2.zero;
            RectTransform.offsetMax = Vector2.zero;
        }

        public void RestoreStandardState()
        {
            if (_isStateSaved)
            {
                RectTransform.anchorMin = _defaultAnchorMin;
                RectTransform.anchorMax = _defaultAnchorMax;
                RectTransform.pivot = _defaultPivot;
            }

            if (_standardSize != Vector2.zero)
                RectTransform.sizeDelta = _standardSize;
            else if (_isStateSaved)
                RectTransform.sizeDelta = _defaultSizeDelta;
        }

        public void ApplyDraggingVisual()
        {
            _background.color = Color.clear;

            var c = _icon.color;
            c.a = 0.3f;
            _icon.color = c;
        }

        public void RestoreVisual()
        {
            var c = _icon.color;
            c.a = 1f;
            _icon.color = c;
        }
    }
}