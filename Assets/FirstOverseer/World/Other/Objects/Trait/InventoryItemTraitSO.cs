using UnityEngine;

namespace FirstOverseer.World.Objects.Traits
{
    [CreateAssetMenu(menuName = "World/Objects/Traits/" + nameof(InventoryItemTraitSO))]
    public class InventoryItemTraitSO : ObjectTraitSO
    {
        [field: SerializeField] public Vector2Int Size { get; private set; } = new Vector2Int(1, 1);

        public override ObjectTraitInstance CreateInstance() => new InventoryItemTraitInstance(this);
    }

    public class InventoryItemTraitInstance : ObjectTraitInstance
    {
        public InventoryItemTraitSO Data { get; private set; }
        public Vector2Int Position { get; set; }

        public InventoryItemTraitInstance(InventoryItemTraitSO data)
        {
            Data = data;
        }
    }
}