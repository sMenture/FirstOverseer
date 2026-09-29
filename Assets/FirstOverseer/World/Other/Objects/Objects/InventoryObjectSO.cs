using FirstOverseer.World.Objects.Traits;
using UnityEngine;

namespace FirstOverseer.World.Objects
{
    [CreateAssetMenu(menuName = "World/Objects/" + nameof(InventoryObjectInstance))]
    public class InventoryObjectSO : WorldObjectSO
    {
    }

    public class InventoryObjectInstance : WorldObjectInstance
    {
        public InventoryItemTraitInstance InventoryTrait { get; private set; }

        public InventoryObjectInstance(WorldObjectSO data) : base(data)
        {
            InventoryTrait = GetTrait<InventoryItemTraitInstance>();
        }
    }
}
