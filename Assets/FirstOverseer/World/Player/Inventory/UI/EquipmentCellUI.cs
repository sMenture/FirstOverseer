using FirstOverseer.World.Objects.Traits.Equipment;
using UnityEngine;

namespace FirstOverseer.World.Player.Inventory
{
    public class EquipmentCellUI : MonoBehaviour
    {
        [field: SerializeField] public EquipmentType SlotType { get; private set; }
        [field: SerializeField] public RectTransform RectTransform { get; private set; }

        public StorageItemUI CurrentItemUI { get; private set; }

        public void Initialize(EquipmentType type)
        {
            SlotType = type;
        }

        public void SetItem(StorageItemUI itemUI)
        {
            CurrentItemUI = itemUI;
            CurrentItemUI.RectTransform.SetParent(RectTransform);
            CurrentItemUI.RectTransform.anchoredPosition = new Vector2(
                RectTransform.sizeDelta.x / 2f,
                -RectTransform.sizeDelta.y / 2f
            );
        }

        public void ClearItem()
        {
            CurrentItemUI = null;
        }
    }
}