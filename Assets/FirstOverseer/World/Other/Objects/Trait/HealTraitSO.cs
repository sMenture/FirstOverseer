using FirstOverseer.World.Objects.Traits;
using System;
using UnityEngine;

namespace FirstOverseer.World.Objects
{
    [CreateAssetMenu(menuName = "World/Objects/Traits/" + nameof(HealTraitSO))]
    public class HealTraitSO : ObjectTraitSO
    {
        [field: SerializeField] public string Id { get; private set; }
        [field: SerializeField] public string NameKey { get; private set; }
        [field: SerializeField] public float MaxHealth { get; private set; }

        public override ObjectTraitInstance CreateInstance() => new HealTraitInstance(this);
    }

    [Serializable]
    public class HealTraitInstance : ObjectTraitInstance
    {
        private readonly HealTraitSO Data;

        private float _currentHealth;
        public event Action<float> OnHealthChanged;

        public HealTraitInstance(HealTraitSO data) => Data = data;

        protected override void OnItialize()
        {
            _currentHealth = Data.MaxHealth;
        }

        public override void OnEnter()
        {
            Owner.OnTakeDamage += HandleTakeDamage;
            Owner.OnHeal += HandleHeal;
        }

        public override void OnExit()
        {
            Owner.OnTakeDamage -= HandleTakeDamage;
            Owner.OnHeal -= HandleHeal;
        }

        public void HandleTakeDamage(float value)
        {
            if (value > 0)
                return;

            _currentHealth = Mathf.Clamp(_currentHealth -  value, 0, Data.MaxHealth);

            OnHealthChanged?.Invoke(_currentHealth);
        } 

        public void HandleHeal(float value)
        {
            if (value < 0)
                return;

            _currentHealth = Mathf.Clamp(_currentHealth + value, 0, Data.MaxHealth);

            OnHealthChanged?.Invoke(_currentHealth);
        } 
    }
}