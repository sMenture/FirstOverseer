using System;
using System.Collections.Generic;
using FirstOverseer.World.Objects;
using FirstOverseer.World.Objects.Traits;
using FirstOverseer.World.Objects.Traits.Equipment;
using UnityEngine;

namespace FirstOverseer.World.Equipment
{
    public class EquipmentManager : MonoBehaviour
    {
        public List<WorldObjectSO> _test;

        public Dictionary<EquipmentType, EquipmentTraitInstance> EquippedTraits { get; private set; } = new();
        public Dictionary<EquipmentTraitInstance, InventoryItemTraitInstance> EquippedInventoryItem { get; private set; } = new();

        public event Action<EquipmentType, InventoryObjectInstance> OnItemEquipped;
        public event Action<EquipmentType> OnItemUnequipped;

        private void Awake()
        {
            foreach (var item in _test)
            {
                var newItem = item.CreateInstance() as InventoryObjectInstance;

                if (newItem != null)
                    TryEquip(newItem);
            }
        }

        public bool TryEquip(InventoryObjectInstance item)
        {
            var equipTrait = item.GetTrait<EquipmentTraitInstance>();
            if (equipTrait == null)
                return false;

            var inventoryTrait = item.GetTrait<InventoryItemTraitInstance>();
            if (inventoryTrait == null)
                return false;

            var type = equipTrait.Data.EquipmentType;

            if (EquippedTraits.ContainsKey(type))
                TryUnequip(type);

            EquippedTraits[type] = equipTrait;
            EquippedInventoryItem[equipTrait] = inventoryTrait;

            OnItemEquipped?.Invoke(type, item);
            return true;
        }

        public bool TryUnequip(EquipmentType type)
        {
            if (EquippedTraits.TryGetValue(type, out var equipTrait))
            {
                equipTrait.OnExit();

                EquippedTraits.Remove(type);
                EquippedInventoryItem.Remove(equipTrait);

                OnItemUnequipped?.Invoke(type);
                return true;
            }

            return false;
        }

        public EquipmentTraitInstance GetEquipped(EquipmentType type)
        {
            if (EquippedTraits.TryGetValue(type, out var trait))
                return trait;

            return null;
        }

        public InventoryObjectInstance GetEquippedObject(EquipmentType type)
        {
            if (EquippedTraits.TryGetValue(type, out var equipTrait))
                if (EquippedInventoryItem.TryGetValue(equipTrait, out var inventoryTrait))
                    return inventoryTrait.Owner as InventoryObjectInstance;

            return null;
        }

    }
}