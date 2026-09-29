using FirstOverseer.World.Objects.Traits;
using System;
using UnityEngine;

namespace FirstOverseer.World.Objects
{
    [CreateAssetMenu(menuName = "World/Objects/Traits/" + nameof(HealTraitSO))]
    public class DieByHealthTraitSO : ObjectTraitSO
    {
        public override ObjectTraitInstance CreateInstance() => new DieByHealthTraitInstance();
    }

    [Serializable]
    public class DieByHealthTraitInstance : ObjectTraitInstance
    {
        private HealTraitInstance _healTrait;

        public event Action OnDie;

        public override void OnEnter()
        {
            _healTrait = Owner.GetTrait<HealTraitInstance>();

            if (_healTrait == null)
                throw new ArgumentNullException(nameof(_healTrait));

            _healTrait.OnHealthChanged += HandleDie;
        }

        public override void OnExit()
        {
            if (_healTrait == null)
                throw new ArgumentNullException(nameof(_healTrait));

            _healTrait.OnHealthChanged -= HandleDie;
        }

        private void HandleDie(float currentHealth)
        {
            if (currentHealth <= 0)
                OnDie?.Invoke();
        }
    }
}