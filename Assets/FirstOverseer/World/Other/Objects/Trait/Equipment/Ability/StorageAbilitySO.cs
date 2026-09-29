using System;
using UnityEngine;

namespace FirstOverseer.World.Objects.Traits.Equipment.Ability
{
    using FirstOverseer.World.Inventory;
    using FirstOverseer.World.Objects;

    [CreateAssetMenu(menuName = "World/Equipment/Ability/" + nameof(StorageAbilitySO))]
    public class StorageAbilitySO : EquipmentAbilitySO
    {
        [field: SerializeField] public int Width { get; private set; } = 4;
        [field: SerializeField] public int Height { get; private set; } = 4;
        [field: SerializeField] public string StorageNameKey { get; private set; } = "crystal_storage";
        [field: SerializeField] public WorldObjectSO[] TestItems { get; private set; }

        public override EquipmentAbilityInstance CreateInstance() => new StorageAbilityInstance(this);
    }

    [Serializable]
    public class StorageAbilityInstance : EquipmentAbilityInstance
    {
        public readonly StorageAbilitySO Data;

        public StorageContainer Container { get; private set; }

        public StorageAbilityInstance(StorageAbilitySO data)
        {
            Data = data;
        }

        protected override void OnInitialize()
        {
            Container = new StorageContainer(Data.StorageNameKey, Data.Width, Data.Height);

            foreach (var item in Data.TestItems)
            {
                var instance = item.CreateInstance() as InventoryObjectInstance;

                if (instance != null)
                    Container.TryAutoAddItem(instance);
            }
        }
    }
}
