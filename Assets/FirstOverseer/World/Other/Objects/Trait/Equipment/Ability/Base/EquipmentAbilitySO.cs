using System;
using UnityEngine;

namespace FirstOverseer.World.Objects.Traits.Equipment.Ability
{
    public abstract class EquipmentAbilitySO : ScriptableObject
    {
        [field: SerializeField] public string Id { get; private set; }
        [field: SerializeField] public string NameKey { get; private set; }

        public abstract EquipmentAbilityInstance CreateInstance();
    }

    [Serializable]
    public abstract class EquipmentAbilityInstance
    {
        public EquipmentTraitInstance Owner { get; private set; }

        public void Initialize(EquipmentTraitInstance owner)
        {
            Owner = owner;

            OnInitialize();
        }

        public void Enable() => OnEnable();
        public void Exit() => OnExit();

        protected virtual void OnInitialize() { }
        protected virtual void OnEnable() { }
        protected virtual void OnExit() { }
    }
}
