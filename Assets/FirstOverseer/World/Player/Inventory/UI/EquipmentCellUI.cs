using FirstOverseer.World.Objects.Traits.Equipment;
using UnityEngine;
using UnityEngine.UI;

namespace FirstOverseer.World.Player.Inventory
{
    public class EquipmentCellUI : MonoBehaviour
    {
        [field: SerializeField] public EquipmentType SlotType { get; private set; }
        [field: SerializeField] public RectTransform RectTransform { get; private set; }
        [SerializeField] private Image _background;

        public StorageItemUI CurrentItemUI { get; private set; }
        private Color _defaultColor;

        private void Awake()
        {
            if (_background != null)
                _defaultColor = _background.color;
        }

        public void Initialize(EquipmentType type)
        {
            SlotType = type;
        }

        public void SetItem(StorageItemUI itemUI)
        {
            CurrentItemUI = itemUI;
            CurrentItemUI.RectTransform.SetParent(RectTransform);
            CurrentItemUI.ApplyEquippedState();
        }

        public void ClearItem()
        {
            CurrentItemUI = null;
        }

        public void SetHighlight(Color color)
        {
            if (_background != null)
                _background.color = color;
        }

        public void ClearHighlight()
        {
            if (_background != null)
                _background.color = _defaultColor;
        }
    }
}