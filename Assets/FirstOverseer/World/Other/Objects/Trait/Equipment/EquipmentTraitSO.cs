using FirstOverseer.World.Objects.Traits.Equipment;
using FirstOverseer.World.Objects.Traits.Equipment.Ability;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace FirstOverseer.World.Objects.Traits
{
    [CreateAssetMenu(menuName = "World/Objects/Traits/" + nameof(EquipmentTraitSO))]
    public class EquipmentTraitSO : ObjectTraitSO
    {
        [Header("Ability")]
        [field: SerializeField] public List<EquipmentAbilitySO> Abilities { get; private set; }
        [field: SerializeField] public EquipmentType EquipmentType { get; private set; }

        public override ObjectTraitInstance CreateInstance() => new EquipmentTraitInstance(this);
    }

    public class EquipmentTraitInstance : ObjectTraitInstance
    {
        public readonly EquipmentAbilityInstance[] Abilities;
        public readonly EquipmentTraitSO Data;

        public EquipmentTraitInstance(EquipmentTraitSO data)
        {
            Data = data;
            var dataAbilities = Data.Abilities;

            Abilities = new EquipmentAbilityInstance[dataAbilities.Count];

            for (int i = 0; i < dataAbilities.Count; i++)
            {
                var ability = dataAbilities[i];
                Abilities[i] = ability.CreateInstance();
                Abilities[i].Initialize(this);
            }
        }

        public override void OnEnter()
        {
            foreach (var ability in Abilities)
                ability.Enable();

            Debug.Log(nameof(EquipmentTraitSO));
        }
        public override void OnExit()
        {
            foreach (var ability in Abilities)
                ability.Exit();
        }
    }
}
